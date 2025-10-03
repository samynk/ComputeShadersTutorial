using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial.HandsOn
{
    internal class HandsOn02 : ComputeConsole
    {
        ShaderStorageBufferObject<float>? operand1Data;
        ShaderStorageBufferObject<float>? operand2Data;
        // output
        ShaderStorageBufferObject<float>? result;

        ComputeShader? addFloatsComputeShader;

        public HandsOn02() : base("AddFloats")
        {
        }

        protected override void Init()
        {
        }
        protected override void Compute()
        {
         
        }
    }
}
