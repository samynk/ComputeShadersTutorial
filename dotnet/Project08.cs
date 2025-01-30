using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    struct TRay
    {
        Vector4 originAndT;    // Combines origin (vec3) and tVal (float), 16 bytes
        Vector4 direction;     // Use vec4 instead of vec3 to maintain alignment, 16 bytes
        Vector4 color;         // Color information, already 16 bytes
        uint rayHits;       // Number of ray hits, 4 bytes
        uint nullRay;
        uint pad1;
        uint pad2;
    };

    internal class Project08 : ComputeWindow
    {
        ComputeShader m_CameraRays;
        int m_ImgDimensionLoc1 = 0;
        int m_CameraPositionLoc = 0;
        int m_CameraMatrixLoc = 0;
        Vector3 m_CameraPosition;

        ComputeShader m_RaysToTexture;
        int m_ImgDimensionLoc2 = 0;

        ComputeShader m_SphereRayTracer;
        int m_NrOfSpheresLoc = 0;
        int m_ImgDimensionLoc3 = 0;

        ShaderStorageBufferObject<TRay> m_Rays;
        ShaderStorageBufferObject<Sphere> m_Spheres;

        Camera m_Camera;
        float m_Phi = 0;
        float m_T = 0;

        public Project08(String title, int w, int h) : base(title, w, h)
        {
            m_CameraRays = new ComputeShader("Resources/computeshaders/raytracer3/CameraRays.glsl");
            m_RaysToTexture = new ComputeShader("Resources/computeshaders/raytracer3/RaysToTexture.glsl");
            m_SphereRayTracer = new ComputeShader("Resources/computeshaders/raytracer3/SphereRayTracer.glsl");
            m_Rays = new ShaderStorageBufferObject<TRay>(0, GetClientWidth(), GetClientHeight(), 1);
            m_Spheres = new ShaderStorageBufferObject<Sphere>(4);

            m_CameraPosition = new Vector3(0, 0, 0);
            m_Camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), true);
        }

        protected override void Init()
        {
            m_CameraRays.Compile();
            m_RaysToTexture.Compile();
            m_SphereRayTracer.Compile();

            m_Rays.Init();
            m_Spheres.Set(0, new Sphere(new Vector3(-0.35f, 0, 2), 0.2f, new Vector4(0, 1, 0, 1)));
            m_Spheres.Set(1, new Sphere(new Vector3(0.35f, 0, 2), 0.2f, new Vector4(1, 1, 0, 1)));
            m_Spheres.Set(2, new Sphere(new Vector3(0, -0.25f, 1.5f), 0.3f, new Vector4(0, 1, 1, 1)));
            m_Spheres.Set(3, new Sphere(new Vector3(0, 0.2f, 2), 0.4f, new Vector4(1, 0, 1, 1)));
            m_Spheres.Init();

            m_ImgDimensionLoc1 = m_CameraRays.GetParameterLocation("imgDimension");
            m_CameraPositionLoc = m_CameraRays.GetParameterLocation("cameraPosition");
            m_CameraMatrixLoc = m_CameraRays.GetParameterLocation("cameraMatrix");
            m_ImgDimensionLoc2 = m_RaysToTexture.GetParameterLocation("imgDimension");
            m_ImgDimensionLoc3 = m_SphereRayTracer.GetParameterLocation("imgDimension");
            m_NrOfSpheresLoc = m_SphereRayTracer.GetParameterLocation("nrOfSpheres");
        }

        protected override void Compute()
        {
            // to do replace with actual time
            m_T += 0.005f;
            m_Phi = float.Pi / 2 + float.Pi / 6 * (float)Math.Sin(m_T);

            m_Camera.SetPhi(m_Phi);
            m_Camera.Update();
            //m_CameraPosition.z += 0.001f;
            // generate the rays
            m_CameraRays.Use();
            m_Rays.BindAsCompute(0);
            m_CameraRays.SetUniformInteger2(m_ImgDimensionLoc1, GetClientWidth(), GetClientHeight());
            m_CameraRays.SetUniformFloat3(m_CameraPositionLoc, m_CameraPosition.X, m_CameraPosition.Y, m_CameraPosition.Z);
            Matrix4 camMatrix = m_Camera.GetCameraMatrix();
            m_CameraRays.SetUniformMatrix(m_CameraMatrixLoc, ref camMatrix);
            m_CameraRays.Compute(GetClientWidth(), GetClientHeight());

            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            // bounce 1
            ComputeBounce();
            // bounce 2
            ComputeBounce();
            // Generate the final image
            WriteOutput();
        }

        protected void ComputeBounce()
        {
            m_SphereRayTracer.Use();

            m_Rays.BindAsCompute(0);
            m_Spheres.BindAsCompute(1);
            m_SphereRayTracer.SetUniformInteger(m_NrOfSpheresLoc, m_Spheres.Size());
            m_SphereRayTracer.SetUniformInteger2(m_ImgDimensionLoc3, GetClientWidth(), GetClientHeight());
            m_SphereRayTracer.Compute(GetClientWidth(), GetClientHeight());
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        }

        protected void WriteOutput()
        {
            m_RaysToTexture.Use();
            m_Rays.BindAsCompute(0);
            BindAsCompute(0);
            m_RaysToTexture.SetUniformInteger2(m_ImgDimensionLoc2, GetClientWidth(), GetClientHeight());
            m_RaysToTexture.Compute(GetClientWidth(), GetClientHeight());
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
        }
    };
}
