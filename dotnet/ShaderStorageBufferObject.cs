using System;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace ComputeShaderTutorial
{


    /// <summary>
    /// Generic Shader Storage Buffer Object for a 2D array of value-type elements.
    /// Meant to store data (like Game of Life cells) in a GPU buffer accessible by compute shaders.
    /// </summary>
    /// <typeparam name="T">Value type (e.g., int, float), must be a struct in C#.</typeparam>
    public sealed class ShaderStorageBufferObject<T> : IDisposable where T : struct
    {
        private int _width;
        private int _height;
        private int _scaleFactor;
        private uint _bindingID;
        private T[] _pInputData;     // Replaces std::vector<T> in C++
        private int _ssboID;         // OpenGL buffer handle

        /// <summary>
        /// Creates an SSBO with a certain 1D size, internally using (size x 1) as dimensions.
        /// </summary>
        public ShaderStorageBufferObject(int size)
            : this(bindingID: 0, width: size, height: 1, scaleFactor: 1)
        {
        }

        /// <summary>
        /// Creates an SSBO from an existing array of T data.
        /// Dimensions are (data.Length x 1).
        /// </summary>
        public ShaderStorageBufferObject(T[] data)
        {
            _width = data.Length;
            _height = 1;
            _scaleFactor = 1;
            _bindingID = 0;
            _ssboID = 0;

            // Copy the array into our local storage
            _pInputData = new T[data.Length];
            Array.Copy(data, _pInputData, data.Length);
        }

        /// <summary>
        /// Creates an SSBO with a 2D width/height, scaled down by 'scaleFactor'.
        /// </summary>
        public ShaderStorageBufferObject(uint bindingID, int width, int height, int scaleFactor)
        {
            _width = width / scaleFactor;
            _height = height / scaleFactor;
            _scaleFactor = scaleFactor;

            _bindingID = bindingID;
            _ssboID = 0;

            _pInputData = new T[_width * _height];
        }

        /// <summary>
        /// Loads a Game of Life RLE file (via RLEReader) into a 2D SSBO.
        /// The buffer is (width/scaleFactor x height/scaleFactor).
        /// </summary>
        public ShaderStorageBufferObject(string file, uint bindingID, int width, int height, int scaleFactor)
        {
            _width = width / scaleFactor;
            _height = height / scaleFactor;
            _scaleFactor = scaleFactor;
            _bindingID = bindingID;
            _ssboID = 0;

            _pInputData = new T[_width * _height];

            // Use the RLEReader here to populate the data array
            // NOTE: This assumes T is 'int' for Game of Life. 
            // If T = int, the cast is straightforward. 
            // If T is something else, you'll need to adapt.
            var reader = new RLEReader(file);
            // We can only call reader.Read(...) if T == int; so consider a type check or separate code path:
            if (typeof(T) == typeof(int))
            {
                // Box/unbox trick: we have _pInputData as T[], which is effectively int[] if T=int
                int[] intArray = new int[_pInputData.Length];
                reader.Read((int)_width, (int)_height, intArray);

                // Copy back into _pInputData
                for (int i = 0; i < intArray.Length; i++)
                {
                    object boxed = intArray[i];
                    _pInputData[i] = (T)boxed;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "RLEReader is only compatible with T=int. Please adjust or provide a suitable reading method."
                );
            }
        }

        /// <summary>
        /// Called once to generate and upload the SSBO data to the GPU.
        /// </summary>
        public void Init()
        {
            // Generate buffer
            _ssboID = GL.GenBuffer();

            // Bind the buffer as a SSBO
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _ssboID);

            // Calculate size in bytes
            int typeSize = Marshal.SizeOf(typeof(T));
            int totalSize = (_width * _height * typeSize);

            // Upload data to GPU with STATIC_DRAW usage
            // OpenTK 4.x has an overload that allows uploading a T[] directly
            GL.BufferData(BufferTarget.ShaderStorageBuffer, totalSize, _pInputData, BufferUsageHint.StaticDraw);

            // Bind the SSBO to the given binding point (by default 0 if you want)
            // We set it to binding=0 here in your example, but you could use _bindingID if needed
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _ssboID);

            // Unbind the buffer
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        public T[] GetRawData()
        {
            return _pInputData;
        }

        public void Download()
        {
            Download(_width * _height);
        }

        public void Download(int nrOfResults)
        {
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _ssboID);
            int typeSize = Marshal.SizeOf(typeof(T));
            int totalSize = (nrOfResults * typeSize);
            GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer,
                                IntPtr.Zero,                           // offset in bytes
                                totalSize,         // size in bytes
                                _pInputData);                              // destination array
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        }

        /// <summary>Width of the buffer’s data in 'T' elements (after scaling).</summary>
        public int GetBufferWidth() => _width;

        /// <summary>Height of the buffer’s data in 'T' elements (after scaling).</summary>
        public int GetBufferHeight() => _height;

        /// <summary>Returns the scale factor used to reduce the original width/height.</summary>
        public int GetScaleFactor() => _scaleFactor;

        /// <summary>
        /// Binds the SSBO at the binding point specified by _bindingID.
        /// </summary>
        public void BindAsCompute()
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, (int)_bindingID, _ssboID);
        }

        /// <summary>
        /// Binds the SSBO at a specified binding point.
        /// </summary>
        public void BindAsCompute(uint bindingID)
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, (int)bindingID, _ssboID);
        }

        /// <summary>
        /// Sets the local array to a checkerboard pattern.
        /// You may call Init() again if you want to re-upload to GPU.
        /// </summary>
        public void SetAsCheckerBoard()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int index = y * _width + x;
                    // Convert (x + y) % 2 to T
                    // If T != int, you must adapt or do a type check.
                    int val = ((int)x + (int)y) % 2;
                    object boxed = val;
                    _pInputData[index] = (T)boxed;
                }
            }
        }

        /// <summary>
        /// Sets a single element in the local array.
        /// Note: If you want to update the GPU, call Init() again or a sub-buffer update method.
        /// </summary>
        public void Set(int index, T value)
        {
            _pInputData[index] = value;
        }

        public T Get(int index)
        {
            return _pInputData[index];
        }

        /// <summary>
        /// Number of elements in the local array (width * height).
        /// </summary>
        public int Size()
        {
            return _pInputData.Length;
        }

        /// <summary>
        /// Release the underlying buffer object from the GPU (similar to the C++ destructor).
        /// </summary>
        public void Dispose()
        {
            if (_ssboID != 0)
            {
                GL.DeleteBuffer(_ssboID);
                _ssboID = 0;
            }
        }

        public T[] getInputData()
        {
            return _pInputData;
        }
    }
}
