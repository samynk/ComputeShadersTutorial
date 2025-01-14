using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{

    struct Ray
    {
        Vector4 m_Origin;
        Vector4 m_Direction;
    };

    internal class Project07 : ComputeWindow
    {
        ComputeShader m_CameraRays;
        int m_ImgDimensionLoc1;
        int m_CameraPositionLoc;
        int m_CameraMatrixLoc;
        Vector3 m_CameraPosition;

        ComputeShader m_ClearDepthBuffer;
        int m_ImgDimensionLoc2;
        int m_MaxDepthValueLoc;
        float m_MaxDepthValue = 1000.0f;

        ComputeShader m_SphereRayTracer;
        int m_NrOfSpheresLoc;
        int m_ImgDimensionLoc3;

        ShaderStorageBufferObject<Ray> m_Rays;
        ShaderStorageBufferObject<float> m_DepthBuffer;
        ShaderStorageBufferObject<Sphere> m_Spheres;

        Camera m_Camera;
        float m_Phi = 0;
        float m_T = 0;
        public Project07(String title, int w, int h) : base(title, w, h)
        {
            m_CameraRays = new ComputeShader("Resources/computeshaders/raytracer2/CameraRays.glsl");
            m_ClearDepthBuffer = new ComputeShader("Resources/computeShaders/ClearDepthBuffer.glsl");
            m_SphereRayTracer = new ComputeShader("Resources/computeshaders/raytracer2/SphereRayTracer.glsl");
            m_DepthBuffer = new ShaderStorageBufferObject<float>(0, GetClientWidth(), GetClientHeight(), 1);
            m_Rays = new ShaderStorageBufferObject<Ray>(0, GetClientWidth(), GetClientHeight(), 1);
            m_Spheres = new ShaderStorageBufferObject<Sphere>(4);

            m_CameraPosition = new Vector3(0, 0, 0);
            m_Camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), true);
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
            m_CameraPositionLoc = m_CameraRays.GetParameterLocation("cameraPosition");
            m_CameraMatrixLoc = m_CameraRays.GetParameterLocation("cameraMatrix");
            m_ImgDimensionLoc2 = m_ClearDepthBuffer.GetParameterLocation("imgDimension");
            m_MaxDepthValueLoc = m_ClearDepthBuffer.GetParameterLocation("maxDepthValue");
            m_ImgDimensionLoc3 = m_SphereRayTracer.GetParameterLocation("imgDimension");
            m_NrOfSpheresLoc = m_SphereRayTracer.GetParameterLocation("nrOfSpheres");
        }

        protected override void Compute()
        {
            // to do replace with actual time
            m_T += 0.005f;
            m_Phi = float.Pi / 2 + float.Pi/6  * (float)Math.Sin(m_T);

            m_Camera.SetPhi(m_Phi);
            m_Camera.Update();
            //m_CameraPosition.z += 0.001f;
            // generate the rays
            m_CameraRays.Use();
            m_Rays.BindAsCompute(0);
            m_CameraRays.SetUniformInteger2(m_ImgDimensionLoc1, GetClientWidth(), GetClientHeight());
            m_CameraRays.SetUniformFloat3(m_CameraPositionLoc, m_CameraPosition.X, m_CameraPosition.Y, m_CameraPosition.Z);
            Matrix4 camMatrix = m_Camera.GetCameraMatrix();
            m_CameraRays.SetUniformMatrix(m_CameraMatrixLoc, ref camMatrix );
            m_CameraRays.Compute(GetClientWidth(), GetClientHeight());


            // clear the depth buffer and set the entire buffer to the far plane.
            m_ClearDepthBuffer.Use();
            m_DepthBuffer.BindAsCompute(0);
            m_ClearDepthBuffer.SetUniformInteger2(m_ImgDimensionLoc2, GetClientWidth(), GetClientHeight());
            m_ClearDepthBuffer.SetUniformFloat(m_MaxDepthValueLoc, m_MaxDepthValue);
            m_ClearDepthBuffer.Compute(GetClientWidth(), GetClientHeight());

            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
            

            // raytracer
            m_SphereRayTracer.Use();

            m_Rays.BindAsCompute(0);
            m_DepthBuffer.BindAsCompute(1);
            m_Spheres.BindAsCompute(2);

            BindAsCompute(0);
            m_SphereRayTracer.SetUniformInteger(m_NrOfSpheresLoc, m_Spheres.Size());
            m_SphereRayTracer.SetUniformInteger2(m_ImgDimensionLoc3, GetClientWidth(), GetClientHeight());
            m_SphereRayTracer.Compute(GetClientWidth(), GetClientHeight());
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
        }

    }
}
