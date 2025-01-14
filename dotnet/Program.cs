using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using System.Transactions;

namespace ComputeShaderTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            //Project04 window = new Project04("Boids", 1024, 1024);
            //Project05 window = new Project05("Camera rays",512,512);
            //Project06 window = new Project06("Sphere scene", 512, 512);
            Project07 window = new Project07("Sphere scene with camera", 512, 512);
            window.Run();
        }
    }

    public class ComputeShaderWindow : GameWindow
    {
        SurfaceRenderer renderer;
        GLImage inputImage;
        ComputeShader grayScale;
        public ComputeShaderWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            renderer = new SurfaceRenderer(0, base.Size.X, base.Size.Y,
               "Resources/shaders/fullscreen_quad.vert",
               "Resources/shaders/fullscreen_quad.frag");

            inputImage = new GLImage(0, TextureAccess.ReadOnly, "Resources/computeshaders/input.png");
            grayScale = new ComputeShader("Resources/computeshaders/imageprocessing/grayscale.glsl");
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            renderer.Init();
            inputImage.Init();
            grayScale.Compile();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Dispatch compute work and draw results here
            grayScale.Use();
            inputImage.BindAsCompute(0);
            renderer.BindAsCompute(1);
            grayScale.Compute(renderer._width, renderer._height);
            
            renderer.DrawQuadWithTexture();

            SwapBuffers();
        }
    }
}

