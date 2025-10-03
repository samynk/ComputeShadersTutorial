using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    // ComputeConsole is opening an OpenGL context but without showing a window.
    class Project11 : ComputeConsole
    {
        // A one dimensional buffer with floats : used to upload to gpu and download from gpu.
        // ShaderStorageBufferObject: SSBO --> primitive types or structs.
        // input
        ShaderStorageBufferObject<float> inputNumbers;
        // output
        ShaderStorageBufferObject<float> outputNumbers;
        // Wrapper around an OpenGL program
        // 1. Compile the shader
        // 2. Use the shader
        // 3. Access to the uniform parameters (uniform synonym for constant)
        ComputeShader mapFunctionComputeShader;
        
        public Project11(string title, int nrOfFloats) 
            :base(title)
        {
            // Create a new ComputeShader object with the given shader code.
            mapFunctionComputeShader = new ComputeShader("Resources/computeshaders/map/square.glsl");
            // create new SSBO (here one dimension with formally: nrOfFloats x 1 x 1)
            inputNumbers = new ShaderStorageBufferObject<float>(nrOfFloats);
            // Random 
            var rng = Random.Shared;
            float min = 0.0f;
            float max = 1000.0f;
            float span = max - min;

            for (int i = 0; i < nrOfFloats; ++i)
            {
                // generate a new random float in the interval [min,max]
                float randomValue = (float)rng.NextDouble() * span + min;
                // set the float inside of the input SSBO
                // this is still CPU side
                inputNumbers.Set(i, randomValue);
            }
            // create an SSBO with the same dimension as the input buffer.
            outputNumbers = new ShaderStorageBufferObject<float>(nrOfFloats);
        }
        protected override void Init()
        {
            // Create SSBO id and upload the data to the GPU.
            inputNumbers.Init();
            // Same
            outputNumbers.Init();
            // Compile the shader:
            // 1. Shader ID generation
            // 2. Attach the source and compile
            // 3. Program ID generation
            // 4. Link the attached shaders --> generates shader exectuable.
            mapFunctionComputeShader.Compile();
        }

        protected override void Compute()
        {
            // Activate the compute shader
            mapFunctionComputeShader.Use();
            // inputNumbers SSBO goes into slot 0
            inputNumbers.BindAsCompute(0);
            // outputNumbers SSBO goes into slot 1
            outputNumbers.BindAsCompute(1);
            // from square.glsl
            // layout(std430, binding = 0) readonly buffer InputBuffer  { float  inData[];  };
            // layout(std430, binding = 1) writeonly buffer OutputBuffer { float outData[]; }

            // localSize from square.glsl:
            // layout (local_size_x = 512, local_size_y = 1, local_size_z = 1) in;
            // --> local size x is 512
            // suppose we have 2048 floats in the input buffer
            // number of workgroups : 2048 / local size x = 2048 / 512 = 4 workgroups
            // You need to call ComputeWorkgroups with the number of workgroups.
            int ThreadsPerGroup = mapFunctionComputeShader.GetLocalSizeX();
            int workGroupCount = (int)Math.Ceiling((float)inputNumbers.GetBufferWidth() / ThreadsPerGroup);

            // Give the order to process this number of workgroups
            mapFunctionComputeShader.ComputeWorkgroups(workGroupCount);
            // Wait for all workgroups to be finished --> Blocking call.
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
            // download from GPU to CPU --> warning slow operation.
            // for performance reasons, only do this for results you need on the cpu
            // not for intermediate result.
            outputNumbers.Download();
            // testing did everything work ok .
            for ( int index = 0; index <256; ++index )
            {
                float input = inputNumbers.Get(index);
                float expected = input * input;
                float actual = outputNumbers.Get(index);
                bool equal = Math.Abs(expected - actual) < 0.0001f;
                if ( !equal)
                {
                    Console.WriteLine("Oh no!");
                    Console.WriteLine("Expected: " + expected + " but got " + actual);
                    Environment.Exit(-1);
                }
            }
            Console.WriteLine("Great succes! Al my bases are belong to me.");
            Environment.Exit(0);
        }


    }
}
