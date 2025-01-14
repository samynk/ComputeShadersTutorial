using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    class Project05:ComputeWindow
    {
        ComputeShader m_CameraRays;
        ComputeShader m_VisualizeRays;
        ShaderStorageBufferObject<Vector4> m_Rays;

        int m_ImgDimensionLoc1;
        int m_ImgDimensionLoc2;



        public Project05(String title, int width, int height):base(title,width,height)
        {
            m_CameraRays = new ComputeShader("Resources/computeshaders/raytracer1/CameraRays.glsl");
            m_VisualizeRays = new ComputeShader("Resources/computeshaders/raytracer1/VisualizeRays.glsl");

            m_Rays = new ShaderStorageBufferObject<Vector4>(0, width, height, 1);
        }

        protected override void Init()
        {
            m_CameraRays.Compile();
            m_ImgDimensionLoc1 = m_CameraRays.GetParameterLocation("imgDimension");
            m_VisualizeRays.Compile();
            m_ImgDimensionLoc2 = m_VisualizeRays.GetParameterLocation("imgDimension");
            m_Rays.Init();
        }

        protected override void Compute()
        {

            m_CameraRays.Use();
            m_CameraRays.SetUniformInteger2(m_ImgDimensionLoc1, GetClientWidth(), GetClientHeight());
            m_Rays.BindAsCompute(0);
            m_CameraRays.Compute(GetClientWidth(), GetClientHeight());

            // convert and render the rays as colors.
            m_VisualizeRays.Use();
            m_VisualizeRays.SetUniformInteger2(m_ImgDimensionLoc2, GetClientWidth(), GetClientHeight());
            m_Rays.BindAsCompute(0);
            BindAsCompute(0);
            m_VisualizeRays.Compute(GetClientWidth(), GetClientHeight());
        }
    }
}
