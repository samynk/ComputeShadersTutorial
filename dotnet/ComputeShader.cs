using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    using System.IO;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;


    public class ComputeShader : IDisposable
    {
        private readonly string _fileLocation;
        private bool _sourceValid;
        private string _shaderContents = string.Empty;
        private int _sourceLength;

        // OpenGL-specific handles
        private int _shaderID = 0;
        private int _computeProgramID = 0;

        // Reasonable defaults for workgroup sizes
        private int _localSizeX = 16;
        private int _localSizeY = 16;
        private int _localSizeZ = 1;

        /// <summary>
        /// Creates a ComputeShader object, loading GLSL source from the specified file.
        /// </summary>
        /// <param name="fileLocation">Path to the .glsl compute shader file.</param>
        public ComputeShader(string fileLocation)
        {
            _fileLocation = fileLocation;
            if (File.Exists(_fileLocation))
            {
                _shaderContents = File.ReadAllText(_fileLocation);
                _sourceValid = true;
                _sourceLength = _shaderContents.Length;
            }
            else
            {
                Console.WriteLine($"ComputeShader Error: File does not exist: {_fileLocation}");
                _sourceValid = false;
            }
        }

        /// <summary>
        /// Compiles and links the compute shader into a program.
        /// </summary>
        public void Compile()
        {
            if (!_sourceValid)
            {
                Console.WriteLine("ComputeShader Error: Shader source is invalid.");
                return;
            }

            // Create and compile the compute shader
            _shaderID = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(_shaderID, _shaderContents);
            GL.CompileShader(_shaderID);

            // Check compile status
            GL.GetShader(_shaderID, ShaderParameter.CompileStatus, out int compileStatus);
            if (compileStatus == 0)
            {
                string infoLog = GL.GetShaderInfoLog(_shaderID);
                Console.WriteLine("ComputeShader Compile Error:\n" + infoLog);
            }

            // Create program and attach shader
            _computeProgramID = GL.CreateProgram();
            GL.AttachShader(_computeProgramID, _shaderID);
            GL.LinkProgram(_computeProgramID);

            // Check link status
            GL.GetProgram(_computeProgramID, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string infoLog = GL.GetProgramInfoLog(_computeProgramID);
                Console.WriteLine("ComputeShader Link Error:\n" + infoLog);
            }

            // Detach and delete the shader (it's now linked into the program)
            GL.DetachShader(_computeProgramID, _shaderID);
            GL.DeleteShader(_shaderID);
            _shaderID = 0;
        }

        /// <summary>
        /// Binds the compute shader program so you can set uniforms or dispatch compute.
        /// </summary>
        public void Use()
        {
            GL.UseProgram(_computeProgramID);
        }

        /// <summary>
        /// Dispatches the compute shader, using xSize and ySize as the total number of groups.
        /// If your shader uses layout(local_size_x/Y/Z = ...), you might choose xSize/ySize
        /// as # of groups, or compute them from total data size / localSize.
        /// </summary>
        /// <param name="xGroups">Number of work groups in the x-dimension.</param>
        /// <param name="yGroups">Number of work groups in the y-dimension.</param>
        /// <param name="zGroups">Number of work groups in the z-dimension (optional).</param>
        public void Compute(int totalWidth, int totalHeight, int zGroups = 1)
        {
            // Calculate the number of workgroups
            int wgSizeX = (int)Math.Ceiling(totalWidth * 1.0f / _localSizeX);
            int wgSizeY = (int)Math.Ceiling(totalHeight * 1.0f / _localSizeY);
            GL.DispatchCompute(wgSizeX, wgSizeY, zGroups);

            // In many cases, you'll want a memory barrier here, depending on how you use the results next
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        }

        /// <summary>
        /// Gets the uniform location for a parameter name in this compute shader program.
        /// Returns -1 if the uniform is not found.
        /// </summary>
        public int GetParameterLocation(string parameterName)
        {
            return GL.GetUniformLocation(_computeProgramID, parameterName);
        }

        /// <summary>
        /// Sets a boolean uniform. Internally stored as int (0 or 1).
        /// </summary>
        public void SetUniformBool(int paramLoc, bool value)
        {
            if (paramLoc >= 0)
            {
                GL.Uniform1(paramLoc, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Sets an integer uniform.
        /// </summary>
        public void SetUniformInteger(int paramLoc, int value)
        {
            if (paramLoc >= 0)
            {
                GL.Uniform1(paramLoc, value);
            }
        }

        /// <summary>
        /// Sets a vec2 of integers uniform.
        /// </summary>
        public void SetUniformInteger2(int paramLoc, int x, int y)
        {
            if (paramLoc >= 0)
            {
                GL.Uniform2(paramLoc, x, y);
            }
        }

        /// <summary>
        /// Sets a float uniform.
        /// </summary>
        public void SetUniformFloat(int paramLoc, float value)
        {
            if (paramLoc >= 0)
            {
                GL.Uniform1(paramLoc, value);
            }
        }

        /// <summary>
        /// Sets a vec3 of floats uniform.
        /// </summary>
        public void SetUniformFloat3(int paramLoc, float x, float y, float z)
        {
            if (paramLoc >= 0)
            {
                GL.Uniform3(paramLoc, x, y, z);
            }
        }

        /// <summary>
        /// Sets a mat4 uniform from an OpenTK Matrix4.
        /// The transpose parameter is 'false' here by default.
        /// If your shader expects row-major data, set transpose = true.
        /// </summary>
        public void SetUniformMatrix(int paramLoc, ref Matrix4 matrix, bool transpose = false)
        {
            if (paramLoc >= 0)
            {
                GL.UniformMatrix4(paramLoc, transpose, ref matrix);
            }
        }

        /// <summary>
        /// Implement the IDisposable pattern to properly release GPU resources.
        /// </summary>
        public void Dispose()
        {
            // Delete the program
            if (_computeProgramID != 0)
            {
                GL.DeleteProgram(_computeProgramID);
                _computeProgramID = 0;
            }
        }
    }
}
