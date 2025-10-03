using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial.HandsOn
{
    using OpenTK.Mathematics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ComputeShaderTutorial
    {
        class HandsOn03 : ComputeWindow
        {
            ComputeShader m_CameraRays;
            ComputeShader m_VisualizeRays;
            ShaderStorageBufferObject<Vector4> m_Rays;

            int m_ImgDimensionLoc1;
            int m_ImgDimensionLoc2;



            public HandsOn03(String title, int width, int height) : base(title, width, height)
            {
                m_CameraRays = new ComputeShader("Resources/computeshaders/handson/CameraRays.glsl");
                m_VisualizeRays = new ComputeShader("Resources/computeshaders/handson/VisualizeRays.glsl");

                m_Rays = new ShaderStorageBufferObject<Vector4>(0, width, height, 1);
            }

            protected override void Init()
            {
                
            }

            protected override void Compute()
            {
                
            }
        }
    }
}
