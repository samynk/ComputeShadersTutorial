using System;
using System.Drawing;            // For Bitmap, etc. (Windows-specific)
using System.Drawing.Imaging;    // For BitmapData
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace ComputeShaderTutorial
{
    /// <summary>
    /// A sealed class analogous to the C++ GLImage, for 2D texture usage in OpenGL compute or rendering.
    /// </summary>
    public sealed class GLImage : IDisposable
    {
        private int _imageID;
        private readonly int _binding;
        private int _width;
        private int _height;
        private readonly TextureAccess _accessType;

        private readonly bool _isEmpty;
        private readonly string _textureLocation;

        private bool _initialized;

        /// <summary>
        /// Constructs a GLImage with specified width/height, for compute usage (no file loaded).
        /// </summary>
        /// <param name="bindingId">Image binding unit, e.g. 0, 1, etc.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="accessType">Read/Write mode for the compute shader (TextureAccess enum).</param>
        public GLImage(int bindingId, int width, int height, TextureAccess accessType)
        {
            _binding = bindingId;
            _width = width;
            _height = height;
            _accessType = accessType;

            _isEmpty = true;
            _textureLocation = string.Empty; // No file
        }

        /// <summary>
        /// Constructs a GLImage by loading from a file (PNG, JPG, etc.).
        /// The width/height come from the file; binding/access are for compute usage.
        /// </summary>
        /// <param name="bindingId">Image binding unit, e.g. 0, 1, etc.</param>
        /// <param name="accessType">Read/Write mode for the compute shader (TextureAccess enum).</param>
        /// <param name="fileLocation">Path to the image file on disk.</param>
        public GLImage(int bindingId, TextureAccess accessType, string fileLocation)
        {
            _binding = bindingId;
            _accessType = accessType;
            _textureLocation = fileLocation;

            _isEmpty = false;

            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException("Image file not found.", fileLocation);
            }

            // We can’t actually create the texture width/height until we load it,
            // but we still want consistent fields. We'll do the actual load in initTexture().
            // For now, just store 0; we’ll set the real size in initTexture().
            _width = 0;
            _height = 0;
        }

        /// <summary>
        /// Creates the OpenGL texture object. Must be called before use.
        /// (Similar to "init()" in your C++ code.)
        /// </summary>
        public void Init()
        {
            if (_initialized)
                return;

            if (_isEmpty)
            {
                InitEmpty();    // Creates a blank 2D texture
            }
            else
            {
                InitTexture();  // Loads from file
            }

            _initialized = true;
        }

        /// <summary>
        /// Binds this texture as an image for compute shader usage at the configured binding slot.
        /// Use this if the binding slot is known from the constructor.
        /// </summary>
        public void BindAsCompute()
        {
            BindAsCompute(_binding);
        }

        /// <summary>
        /// Binds this texture as an image for compute shader usage, specifying the binding slot.
        /// </summary>
        public void BindAsCompute(int bindingSlot)
        {
            GL.BindImageTexture(
                unit: bindingSlot,
                texture: _imageID,
                level: 0,
                layered: false,
                layer: 0,
                access: _accessType,
                format: SizedInternalFormat.Rgba8
            );
        }

        /// <summary>
        /// Regularly binds the texture for, say, a rendering pass (fragment shader usage).
        /// </summary>
        public void BindAsTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, _imageID);
        }

        /// <summary>
        /// Saves the texture to disk as an image file. 
        /// Uses System.Drawing to create a Bitmap and save to the specified path.
        /// </summary>
        public void Write(string fileLocation)
        {
            if (_imageID == 0 || !_initialized)
                return;

            // 1. Bind the texture and read its pixels
            GL.BindTexture(TextureTarget.Texture2D, _imageID);

            // We assume RGBA8
            var pixelData = new byte[_width * _height * 4];
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);

            using Image<Rgba32> image = Image.LoadPixelData<Rgba32>(pixelData, _width, _height);
            image.SaveAsPng(fileLocation);
        }

        /// <summary>
        /// Disposes the underlying OpenGL texture resource.
        /// </summary>
        public void Dispose()
        {
            if (_imageID != 0)
            {
                GL.DeleteTexture(_imageID);
                _imageID = 0;
            }
        }

        // ------------------------
        // Private helpers
        // ------------------------

        /// <summary>
        /// Creates a blank 2D texture of the given width/height using RGBA8.
        /// </summary>
        private void InitEmpty()
        {
            if (_width <= 0 || _height <= 0)
                throw new InvalidOperationException("Invalid width/height for empty GLImage.");

            _imageID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _imageID);

            // Allocate space for RGBA8
            GL.TexImage2D(
                target: TextureTarget.Texture2D,
                level: 0,
                internalformat: PixelInternalFormat.Rgba8,
                width: _width,
                height: _height,
                border: 0,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: IntPtr.Zero
            );

            // Basic filtering (optional)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Unbind
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Loads an image from file and creates a 2D texture with it.
        /// </summary>
        private void InitTexture()
        {
            if (string.IsNullOrEmpty(_textureLocation))
                throw new InvalidOperationException("No texture file location specified.");



            // If we didn’t store the width/height previously, set them
            if (_width == 0 && _height == 0)
            {
                using Image<Rgba32> image = Image.Load<Rgba32>(_textureLocation);
                _width = image.Width;
                _height = image.Height;

                // Convert to raw byte array
                var pixelData = new Rgba32[_width * _height];
                image.CopyPixelDataTo(pixelData);

                // Each Rgba32 is 4 bytes (R, G, B, A)
                // If you want a byte[] specifically, convert:
                var result = new byte[pixelData.Length * 4];
                for (int i = 0; i < pixelData.Length; i++)
                {
                    result[i * 4 + 0] = pixelData[i].R;
                    result[i * 4 + 1] = pixelData[i].G;
                    result[i * 4 + 2] = pixelData[i].B;
                    result[i * 4 + 3] = pixelData[i].A;
                }

                var imageID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, imageID);


                // In typical OpenGL usage, Format32bppArgb means the pixel data is in BGRA order. 
                // For correctness, you might want PixelFormat.Bgra or do a swizzle. 
                // For simplicity, we'll just use PixelFormat.Bgra and PixelType.UnsignedByte.
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba8,
                    _width,
                    _height,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    pixelData
                );

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                // Unbind
                GL.BindTexture(TextureTarget.Texture2D, 0);

                _imageID = imageID;
            }
        }
    }
}