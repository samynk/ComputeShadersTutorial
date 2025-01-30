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
    internal class Project01 : ComputeWindow
    {
        GLImage inputImage;
        ComputeShader grayScale;

        private const int scaleFactor = 4;
        public Project01(String title, int w, int h)
            : base(title,w,h)
        {
            inputImage = new GLImage(0, TextureAccess.ReadOnly, "Resources/computeshaders/input.png");
            grayScale = new ComputeShader("Resources/computeshaders/imageprocessing/grayscale.glsl");
        }

        protected override void Init()
        {
            inputImage.Init();
            grayScale.Compile();
        }

        protected override void Compute()
        {
            grayScale.Use();
            inputImage.BindAsCompute(0);
            BindAsCompute(1);
            grayScale.Compute(GetClientWidth(), GetClientHeight());
            GL.MemoryBarrier(MemoryBarrierFlags.AllBarrierBits);
        }
    }
}
