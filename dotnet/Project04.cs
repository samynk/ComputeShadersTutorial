using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    struct Boid
    {
        public Boid(float x, float y, float vx, float vy)
        {
            m_px = x;
            m_py = y;
            m_vx = vx;
            m_vy = vy;
        }

        float m_px, m_py;
        float m_vx, m_vy;
    };


    internal class Project04 : ComputeWindow
    {
        ShaderStorageBufferObject<Boid> m_GridData0;
        ShaderStorageBufferObject<Boid> m_GridData1;

        ComputeShader m_FlockCompute;
        ComputeShader m_ClearShader;
        ComputeShader m_ConvertFlock;

        Random random = new Random();

        uint m_CurrentBufferID = 0;
        public Project04(String title, int w, int h)
           : base(title, w, h)
        {
            m_GridData0 = new ShaderStorageBufferObject<Boid>(0, 10000, 1, 1);
            m_GridData1 = new ShaderStorageBufferObject<Boid>(1, 10000, 1, 1);
            m_FlockCompute = new ComputeShader("Resources/computeshaders/flocking/flocking.glsl");
            m_ConvertFlock = new ComputeShader("Resources/computeshaders/flocking/convert_flock_to_texture.glsl");
            m_ClearShader = new ComputeShader("Resources/computeshaders/flocking/clear.glsl");
        }

        private double NextScalar()
        {
            return random.NextSingle();
        }

        private double NextAngle()
        {
            return random.NextSingle() * 2 * Math.PI;
        }

        protected override void Init()
        {
            m_FlockCompute.Compile();
            m_ConvertFlock.Compile();
            m_ClearShader.Compile();

            Random random = new Random();

            float radius = 100.0f;

            for (int iboid = 0; iboid < 2048; ++iboid)
            {

                // Generate random spherical coordinates
                double theta = NextAngle();  // Random angle in [0, 2*pi]
                double phi = NextAngle() / 2; ;  // Random angle in [0, pi] for uniform spherical distribution
                double r = radius * Math.Cbrt(random.NextSingle());  // Random radius scaled to maintain uniform density within sphere
                double speed = 3.0f * random.NextSingle() + 1.0f;

                // Convert spherical coordinates to Cartesian
                double x = r * Math.Cos(phi);
                double y = r * Math.Sin(phi);
                double vx = speed * Math.Cos(theta);
                double vy = speed * Math.Sin(theta);

                m_GridData0.Set(iboid, new Boid((float)x, (float)y, (float)vx, (float)vy));
            }
            m_GridData0.Init();
            m_GridData1.Init();
        }

        protected override void Compute()
        {
            uint nextID = (m_CurrentBufferID + 1) % 2;
            m_FlockCompute.Use();
            m_GridData0.BindAsCompute(m_CurrentBufferID);
            m_GridData1.BindAsCompute(nextID);
            m_FlockCompute.Compute(m_GridData0.GetBufferWidth(), 1);

            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            m_ClearShader.Use();
            m_ClearShader.Compute(GetClientWidth(), GetClientHeight());
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            m_ConvertFlock.Use();
            switch (m_CurrentBufferID)
            {
                case 0: m_GridData1.BindAsCompute(0); break;
                case 1: m_GridData0.BindAsCompute(0); break;
            }
            BindAsCompute(0);
            m_ConvertFlock.Compute(m_GridData0.GetBufferWidth(), 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
            m_CurrentBufferID = nextID;
        }
    }
}
