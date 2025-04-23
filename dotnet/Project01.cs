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
        GLImage blurredHorizontal;
        ComputeShader blurShader;
        int _HorizontalLocation;


        
        public Project01(String title, int w, int h)
            : base(title,w,h)
        {
            inputImage = new GLImage(0, TextureAccess.ReadOnly, "Resources/computeshaders/input2.png");
            blurredHorizontal = new GLImage(1, TextureAccess.ReadWrite, "Resources/computeshaders/input2.png");
            blurShader = new ComputeShader("Resources/computeshaders/imageprocessing/blur.glsl");
            _HorizontalLocation = 0;
        }

        protected override void Init()
        {
            inputImage.Init();
            blurredHorizontal.Init();
            blurShader.Compile();
            _HorizontalLocation = blurShader.GetParameterLocation("horizontal");
        }

        protected override void Compute()
        {
            blurShader.Use();
            blurShader.SetUniformBool(_HorizontalLocation, true);
            inputImage.BindAsCompute(0);
            blurredHorizontal.BindAsCompute(1);
            blurShader.Compute(GetClientWidth(), GetClientHeight());
            GL.MemoryBarrier(MemoryBarrierFlags.TextureUpdateBarrierBit);

            
            blurShader.SetUniformBool(_HorizontalLocation, false);
            blurredHorizontal.BindAsCompute(0);
            BindAsCompute(1);
            blurShader.Compute(GetClientWidth(), GetClientHeight());
            GL.MemoryBarrier(MemoryBarrierFlags.TextureUpdateBarrierBit);
        }
    }
}
