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
            // corresponds to uniform (constant) in the CamerayRays.glsl
            // uniform ivec2 imgDimension;
            m_VisualizeRays.Compile();
            m_ImgDimensionLoc2 = m_VisualizeRays.GetParameterLocation("imgDimension");
            m_Rays.Init();
        }

        protected override void Compute()
        {
            // activate camerarays compute shader.
            m_CameraRays.Use();
            // set the value of the uniform.
            m_CameraRays.SetUniformInteger2(m_ImgDimensionLoc1, GetClientWidth(), GetClientHeight());
            // bind m_Rays --> SSBO in slot 0 of the compute shader.
            m_Rays.BindAsCompute(0);
            // Compute calculates the workgroups for you.
            m_CameraRays.Compute(GetClientWidth(), GetClientHeight());

            // convert and render the rays as colors.
            m_VisualizeRays.Use();
            // set the constant
            m_VisualizeRays.SetUniformInteger2(m_ImgDimensionLoc2, GetClientWidth(), GetClientHeight());
            // bind the rays in slot 0
            m_Rays.BindAsCompute(0);
            // bind the final texture also in slot 0
            BindAsCompute(0);
            m_VisualizeRays.Compute(GetClientWidth(), GetClientHeight());
        }
    }
}
