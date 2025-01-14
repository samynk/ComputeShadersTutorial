using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{

    internal class Project03 : ComputeWindow
    {
        ShaderStorageBufferObject<int> m_GridData0;
        ShaderStorageBufferObject<int> m_GridData1;
        ComputeShader m_ConvertComputeShader;
        ComputeShader m_GameOfLifeShader;
        int m_ScaleFactorLocation;
        int m_DimensionLocation;
        int m_ConvertInputDimensionLocation;

        uint m_CurrentDataID = 0;
        uint m_FrameCount = 0;

        public Project03(String title, int w, int h)
           : base(title, w, h)
        {
            m_GameOfLifeShader = new ComputeShader("Resources/computeshaders/gameoflife/gameoflife.glsl");
            m_ConvertComputeShader = new ComputeShader("Resources/computeshaders/convert_to_texture.glsl");
            m_GridData0 = new ShaderStorageBufferObject<int>("Resources/patterns/spacefillersynth.rle", 
                0, 
                GetClientWidth(), 
                GetClientHeight(), 
                2);
            m_GridData1 = new ShaderStorageBufferObject<int>( 
                1, 
                GetClientWidth(),
                GetClientHeight(),
                2);
        }

        protected override void Init()
        {
            m_GridData0.Init();
            m_GridData1.Init();

            m_GameOfLifeShader.Compile();
            m_ConvertComputeShader.Compile();

            m_ScaleFactorLocation = m_ConvertComputeShader.GetParameterLocation("scaleFactor");
            m_ConvertInputDimensionLocation = m_ConvertComputeShader.GetParameterLocation("inputDimension");
            m_DimensionLocation = m_GameOfLifeShader.GetParameterLocation("gridDimension");

        }

        protected override void Compute()
        {
            m_GameOfLifeShader.Use();
            int bufferWidth = m_GridData0.GetBufferWidth();
            int bufferHeight = m_GridData0.GetBufferHeight();
            m_GameOfLifeShader.SetUniformInteger2(m_DimensionLocation, bufferWidth, bufferHeight);
            m_GridData0.BindAsCompute(m_CurrentDataID);
            uint nextID = (m_CurrentDataID + 1) % 2;
            m_GridData1.BindAsCompute(nextID);
            m_GameOfLifeShader.Compute(bufferWidth, bufferHeight);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            m_ConvertComputeShader.Use();
            m_ConvertComputeShader.SetUniformInteger(m_ScaleFactorLocation, m_GridData0.GetScaleFactor());
            switch (m_CurrentDataID)
            {
                case 0:
                    m_GridData0.BindAsCompute(0);
                    break;
                case 1:
                    m_GridData1.BindAsCompute(0);
                    break;
            }

            BindAsCompute(0); // binding number 0
            m_ConvertComputeShader.SetUniformInteger2(m_ConvertInputDimensionLocation, bufferWidth, bufferHeight);
            m_ConvertComputeShader.Compute(GetClientWidth(),GetClientHeight());

            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
            m_CurrentDataID = nextID;
        }
    }
}
