using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial.HandsOn
{
    struct Sphere
    {
        public Sphere(Vector3 loc, float radius, Vector4 color)
        {
            m_Location = loc;
            m_Radius = radius;
            m_Color = color;
        }

        Vector3 m_Location;
        float m_Radius;
        Vector4 m_Color;
    };

    class HandsOn04 : ComputeWindow
    {
        ComputeShader m_CameraRays;
        int m_ImgDimensionLoc1;

        ComputeShader m_ClearDepthBuffer;
        int m_ImgDimensionLoc2;
        int m_MaxDepthValueLoc;
        float m_MaxDepthValue = 1000.0f;

        ComputeShader m_SphereRayTracer;
        int m_NrOfSpheresLoc;
        int m_ImgDimensionLoc3;

        ShaderStorageBufferObject<Vector4> m_Rays;
        ShaderStorageBufferObject<float> m_DepthBuffer;
        ShaderStorageBufferObject<Sphere> m_Spheres;

        public HandsOn04(String title, int w, int h) : base(title, w, h)
        {
            m_CameraRays = new ComputeShader("Resources/computeshaders/handson/CameraRays.glsl");
            m_ClearDepthBuffer = new ComputeShader("Resources/computeShaders/handson/ClearDepthBuffer.glsl");
            m_SphereRayTracer = new ComputeShader("Resources/computeshaders/handson/SphereRayTracer.glsl");
            m_DepthBuffer = new ShaderStorageBufferObject<float>(0, GetClientWidth(), GetClientHeight(), 1);
            m_Rays = new ShaderStorageBufferObject<Vector4>(0, GetClientWidth(), GetClientHeight(), 1);
            m_Spheres = new ShaderStorageBufferObject<Sphere>(4);
        }

        protected override void Init()
        {
            m_CameraRays.Compile();
            m_ClearDepthBuffer.Compile();
            m_SphereRayTracer.Compile();

            m_Rays.Init();
            m_DepthBuffer.Init();


            m_Spheres.Set(0, new Sphere(new Vector3(0, 0, 2), 0.4f, new Vector4(0, 1, 0, 1)));
            m_Spheres.Set(1, new Sphere(new Vector3(-0.45f, -0.25f, 1.9f), 0.25f, new Vector4(1, 1, 0, 1)));
            m_Spheres.Set(2, new Sphere(new Vector3(0.25f, -0.2f, 2), 0.3f, new Vector4(0, 1, 1, 1)));
            m_Spheres.Set(3, new Sphere(new Vector3(0, 0.45f, 2), 0.4f, new Vector4(1, 0, 1, 1)));
            m_Spheres.Init();


            m_ImgDimensionLoc1 = m_CameraRays.GetParameterLocation("imgDimension");
            m_ImgDimensionLoc2 = m_ClearDepthBuffer.GetParameterLocation("imgDimension");
            m_MaxDepthValueLoc = m_ClearDepthBuffer.GetParameterLocation("maxDepthValue");
            m_ImgDimensionLoc3 = m_SphereRayTracer.GetParameterLocation("imgDimension");
            m_NrOfSpheresLoc = m_SphereRayTracer.GetParameterLocation("nrOfSpheres");
        }

        protected override void Compute()
        {
            // generate the rays
            m_CameraRays.Use();
            m_Rays.BindAsCompute(0);
            m_CameraRays.SetUniformInteger2(m_ImgDimensionLoc1, GetClientWidth(), GetClientHeight());
            m_CameraRays.Compute(GetClientWidth(), GetClientHeight());


            // clear the depth buffer and set the entire buffer to the far plane.
            m_ClearDepthBuffer.Use();
            m_DepthBuffer.BindAsCompute(0);
            m_ClearDepthBuffer.SetUniformInteger2(m_ImgDimensionLoc2, GetClientWidth(), GetClientHeight());
            m_ClearDepthBuffer.SetUniformFloat(m_MaxDepthValueLoc, m_MaxDepthValue);
            m_ClearDepthBuffer.Compute(GetClientWidth(), GetClientHeight());

            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            // raytracer
        }
    }
}
