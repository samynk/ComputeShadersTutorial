using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    internal class Project02 : ComputeWindow
    {
        ShaderStorageBufferObject<int> m_GridData;
        ComputeShader m_ConvertComputeShader;
        int m_ScaleFactorLocation;
        int m_ConvertInputDimensionLocation;

        private const int scaleFactor = 4;
        public Project02(String title, int w, int h)
            : base(title,w,h)
        {
            m_ConvertComputeShader = new ComputeShader( "Resources/computeshaders/convert_to_texture.glsl" );
	        m_GridData = new ShaderStorageBufferObject<int>( 
                "Resources/patterns/spacefillersynth.rle", 
                0, GetClientWidth(),GetClientHeight(), 2 );
        }

        protected override void Init()
        {
            m_ConvertComputeShader.Compile();
            m_GridData.Init();

            m_ScaleFactorLocation = m_ConvertComputeShader.GetParameterLocation("scaleFactor");
            m_ConvertInputDimensionLocation = m_ConvertComputeShader.GetParameterLocation("inputDimension");
        }

        protected override void Compute()
        {
            // Dispatch compute work and draw results here
            m_ConvertComputeShader.Use();
            m_ConvertComputeShader.SetUniformInteger(m_ScaleFactorLocation, m_GridData.GetScaleFactor());
            m_GridData.BindAsCompute(0);
            m_ConvertComputeShader.SetUniformInteger2(m_ConvertInputDimensionLocation,
                m_GridData.GetBufferWidth(),
                m_GridData.GetBufferHeight());
            BindAsCompute(0); // binding number 0
            m_ConvertComputeShader.Compute(GetClientWidth(), GetClientHeight());
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        }
    }
}
