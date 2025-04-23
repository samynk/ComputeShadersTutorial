using System;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics; // For Vector types if needed

namespace ComputeShaderTutorial
{
    public sealed class SurfaceRenderer : IDisposable
    {
        // A quad (two triangles) covering the entire screen.
        // Each vertex: (pos.x, pos.y, texCoord.x, texCoord.y)
        private static readonly float[] QuadVertices =
        {
            //   Positions     TexCoords
            // Triangle 1
            -1.0f,  1.0f,     0.0f, 0.0f, // bottom left
            -1.0f, -1.0f,     0.0f, 1.0f, // top left
             1.0f,  1.0f,     1.0f, 0.0f, // bottom right

            // Triangle 2
            -1.0f, -1.0f,     0.0f, 1.0f, // top left
             1.0f, -1.0f,     1.0f, 1.0f, // top right
             1.0f,  1.0f,     1.0f, 0.0f  // bottom right
        };

        private readonly string _vertexShaderFile;
        private readonly string _fragmentShaderFile;

        // The final target for the compute shader pipeline, also used as a texture.
        private readonly GLImage _fullScreenImage;

        private int _programID;
        private int _vertexArrayObject;
        private int _vertexBufferObject;
        private int _screenTextureLoc;

        public int _width { get; }
        public int _height { get; }

        /// <summary>
        /// Constructor that takes a binding ID for compute usage, a width/height,
        /// and paths to the vertex/fragment shaders for rendering.
        /// </summary>
        public SurfaceRenderer(int bindingId, int w, int h, string vertexShader, string fragmentShader)
        {
            _vertexShaderFile = vertexShader;
            _fragmentShaderFile = fragmentShader;

            // This is our "final" image that compute writes to,
            // and which we will also sample from in a fullscreen pass:
            _fullScreenImage = new GLImage(bindingId, w, h, TextureAccess.ReadWrite);

            _programID = 0;
            _vertexArrayObject = 0;
            _vertexBufferObject = 0;

            _width = w;
            _height = h;
        }

        /// <summary>
        /// Initializes the renderer: compiles the shaders, initializes the full-screen image,
        /// and sets up the vertex data for a fullscreen quad.
        /// </summary>
        public void Init()
        {
            CreateShaderProgram(_vertexShaderFile, _fragmentShaderFile);
            _fullScreenImage.Init();
            SetupQuad();
        }

        /// <summary>
        /// Binds the internal texture as an image for compute (using the binding from the constructor).
        /// </summary>
        public void BindAsCompute()
        {
            _fullScreenImage.BindAsCompute();
        }

        /// <summary>
        /// Binds the internal texture as an image for compute, using a specified binding slot.
        /// </summary>
        public void BindAsCompute(int bindingSlot)
        {
            _fullScreenImage.BindAsCompute(bindingSlot);
        }

        /// <summary>
        /// Draws a full-screen quad with the internal texture bound as a regular sampler2D.
        /// </summary>
        public void DrawQuadWithTexture()
        {
            GL.UseProgram(_programID);

            // Set which texture unit the sampler2D should use. 
            // We will bind the image to texture unit 0.
            GL.Uniform1(_screenTextureLoc, 0);

            // Bind the full-screen image as a regular texture.
            _fullScreenImage.BindAsTexture();

            // Draw the quad
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Frees the underlying GPU resources: the shader program, VAO, and VBO.
        /// </summary>
        public void Dispose()
        {
            if (_programID != 0)
            {
                GL.DeleteProgram(_programID);
                _programID = 0;
            }

            if (_vertexArrayObject != 0)
            {
                GL.DeleteVertexArray(_vertexArrayObject);
                _vertexArrayObject = 0;
            }

            if (_vertexBufferObject != 0)
            {
                GL.DeleteBuffer(_vertexBufferObject);
                _vertexBufferObject = 0;
            }

            // Dispose the fullScreenImage as well
            _fullScreenImage.Dispose();
        }

        // --------------------------------------------------------------------
        // Private Helpers
        // --------------------------------------------------------------------

        /// <summary>
        /// Compiles and links the vertex/fragment shader program for drawing the full-screen quad.
        /// </summary>
        private void CreateShaderProgram(string vertexShaderFile, string fragmentShaderFile)
        {
            int vertexShaderID = CompileShader(LoadShaderSource(vertexShaderFile), ShaderType.VertexShader);
            int fragmentShaderID = CompileShader(LoadShaderSource(fragmentShaderFile), ShaderType.FragmentShader);

            _programID = GL.CreateProgram();
            GL.AttachShader(_programID, vertexShaderID);
            GL.AttachShader(_programID, fragmentShaderID);
            GL.LinkProgram(_programID);

            // Check for linking errors
            GL.GetProgram(_programID, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(_programID);
                throw new Exception($"ERROR::PROGRAM::LINKING_FAILED\n{infoLog}");
            }

            // Obtain uniform location for the texture
            _screenTextureLoc = GL.GetUniformLocation(_programID, "screenTexture");

            // Shaders are no longer needed after linking
            GL.DetachShader(_programID, vertexShaderID);
            GL.DetachShader(_programID, fragmentShaderID);
            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);
        }

        /// <summary>
        /// Creates a VAO/VBO for a 2D fullscreen quad (two triangles).
        /// </summary>
        private void SetupQuad()
        {
            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, QuadVertices.Length * sizeof(float),
                QuadVertices, BufferUsageHint.StaticDraw);

            // Position attribute
            GL.VertexAttribPointer(
                index: 0,
                size: 2,
                type: VertexAttribPointerType.Float,
                normalized: false,
                stride: 4 * sizeof(float),
                offset: 0
            );
            GL.EnableVertexAttribArray(0);

            // Texture coordinate attribute
            GL.VertexAttribPointer(
                index: 1,
                size: 2,
                type: VertexAttribPointerType.Float,
                normalized: false,
                stride: 4 * sizeof(float),
                offset: 2 * sizeof(float)
            );
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /// <summary>
        /// Compiles either a vertex or fragment shader from a string of GLSL source.
        /// </summary>
        private int CompileShader(string shaderSource, ShaderType shaderType)
        {
            int shaderID = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderID, shaderSource);
            GL.CompileShader(shaderID);

            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderID);
                throw new Exception($"ERROR::SHADER::{shaderType}::COMPILATION_FAILED\n{infoLog}");
            }

            return shaderID;
        }

        /// <summary>
        /// Reads the entire shader file into a string.
        /// </summary>
        private string LoadShaderSource(string shaderFile)
        {
            if (!File.Exists(shaderFile))
            {
                throw new FileNotFoundException("Error: Could not open shader file.", shaderFile);
            }
            return File.ReadAllText(shaderFile);
        }
    }
}
