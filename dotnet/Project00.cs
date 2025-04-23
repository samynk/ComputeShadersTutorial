using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    class Project00: ComputeWindow
    {
        GLImage inputImage;
        ComputeShader grayScale;

        private const int scaleFactor = 4;
        public Project00(String title, int w, int h)
            : base(title, w, h)
        {
            inputImage = new GLImage(0, TextureAccess.ReadOnly, "Resources/computeshaders/input2.png");
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
