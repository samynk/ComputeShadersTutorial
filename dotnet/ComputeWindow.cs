using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    internal abstract class ComputeWindow : GameWindow
    {
        private SurfaceRenderer renderer;
        private int m_Width;
        private int m_Height;

        public ComputeWindow(String title, int width, int height)
            :base(GameWindowSettings.Default,
                 new NativeWindowSettings()
                 {
                     ClientSize = new OpenTK.Mathematics.Vector2i(width, height),
                     Title = title,
                     APIVersion = new System.Version(4, 3), // Need at least 4.3 for compute shaders
                 })
        {

            m_Width = width;
            m_Height = height;

            renderer = new SurfaceRenderer(0, m_Width, m_Height,
              "Resources/shaders/fullscreen_quad.vert",
              "Resources/shaders/fullscreen_quad.frag");
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.On;
            renderer.Init();
            Init();
        }

        public int GetClientWidth() => m_Width;
        public int GetClientHeight() => m_Height;

        protected abstract void Init();
        protected abstract void Compute();



        public void BindAsCompute(int bindId)
        {
            renderer.BindAsCompute(bindId);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Compute();
            renderer.DrawQuadWithTexture();
            SwapBuffers();
        }
    }
}
