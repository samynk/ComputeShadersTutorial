using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
   
    class Project10: ComputeConsole
    {
        ShaderStorageBufferObject<float> _buffer1;
        ShaderStorageBufferObject<float> _buffer2;
        ComputeShader maxReduce;

        int _elementCount;
        int _uCountLoc;

        public Project10(String title, int nrOfFloats, float min, float max)
            :base(title)
        {
            _elementCount = nrOfFloats;
            _buffer1 = new ShaderStorageBufferObject<float>(nrOfFloats);
            _buffer2 = new ShaderStorageBufferObject<float>(nrOfFloats);
            var rng = Random.Shared;
            float span = max - min;

            float expectedMax = 0;
            for (int i = 0; i < nrOfFloats; ++i)
            {
                float randomValue = (float)rng.NextDouble() * span + min;
                _buffer1.Set(i, randomValue);
                if (randomValue > expectedMax)
                {
                    expectedMax = randomValue;
                }
            }
            Console.WriteLine("Expected max value: " + expectedMax);
            maxReduce = new ComputeShader("Resources/computeshaders/reduce/max.glsl");
        }

        protected override void Init()
        {
            maxReduce.Compile();
            _uCountLoc = maxReduce.GetParameterLocation("uCount");
            _buffer1.Init();
            _buffer2.Init();
        }

        protected override void Compute()
        {
            float[] values = _buffer1.GetRawData();

            // simpe CPU single threaded for loop.
            long[] cpuSerialMeasurements = new long[20];
            for (int si = 0; si < cpuSerialMeasurements.Length; ++si)
            { 
                var sw = Stopwatch.StartNew();
                float max = values.Aggregate(float.MinValue, Math.Max);
                sw.Stop();
                cpuSerialMeasurements[si] = sw.Elapsed.Microseconds;
            }
            long meanCPUSerial = (long)cpuSerialMeasurements.Average();
            Console.WriteLine($"CPU result serial - Elapsed: {meanCPUSerial:F3} ms");

            // parallel
            long[] cpuParallelMeasurements = new long[20];
            for (int si = 0; si < cpuParallelMeasurements.Length; ++si)
            {
                var sw = Stopwatch.StartNew();
                float max = values.AsParallel().Max();
                sw.Stop();
                cpuParallelMeasurements[si] = sw.Elapsed.Microseconds;
            }
            long meanCPUParallel = (long)cpuParallelMeasurements.Average();
            Console.WriteLine($"CPU result parallel - Elapsed: {meanCPUParallel:F3} ms");

            long[] gpuParallelMeasurements = new long[20];
            for (int si = 0; si < gpuParallelMeasurements.Length; ++si)
            {
                var sw = Stopwatch.StartNew();

                int ThreadsPerGroup = maxReduce.GetLocalSizeX();
                int passCount = (int)Math.Ceiling((float)_elementCount / ThreadsPerGroup);
                int elementCount = _elementCount;
                while (passCount > 1)
                {
                    ComputeReduceStep(passCount, elementCount);
                    // next iteration: use previous output as new input
                    elementCount = passCount;
                    //_buffer2.Download();
                    (_buffer1, _buffer2) = (_buffer2, _buffer1);
                    passCount = (int)Math.Ceiling((float)elementCount / ThreadsPerGroup);
                }
                ComputeReduceStep(passCount, elementCount);
                _buffer2.Download(1);

                sw.Stop();
                gpuParallelMeasurements[si] = sw.Elapsed.Microseconds;
            }

            // 3️⃣ Stop and inspect
            long meanGPU = (long)gpuParallelMeasurements.Average();
            Console.WriteLine($"GPU result: Elapsed: {meanGPU:F3} microseconds");

            Environment.Exit(0);
            
        }

        private void ComputeReduceStep(int passCount, int elementCount)
        {
            maxReduce.Use();
            _buffer1.BindAsCompute(0);
            _buffer2.BindAsCompute(1);
            maxReduce.SetUniformInteger(_uCountLoc, elementCount);
            maxReduce.ComputeWorkgroups(passCount, 1, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        }
    }
}
