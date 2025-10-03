using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using System.Transactions;
using ComputeShaderTutorial.HandsOn;
using ComputeShaderTutorial.HandsOn.ComputeShaderTutorial;

namespace ComputeShaderTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            //Project00 window = new Project00("Postprocessing", 512, 512);
            //Project01 window = new Project01("Postprocessing", 512, 512);
            //Project02 window = new Project02("Convert to texture", 1024, 1024);
            //Project03 window = new Project03("Conway", 1024, 1024);
            //Project04 window = new Project04("Boids", 1024, 1024);
            //Project05 window = new Project05("Camera rays",512,512);
            //Project06 window = new Project06("Sphere scene", 512, 512);
            //Project07 window = new Project07("Sphere scene with camera", 512, 512);
            //Project08 window = new Project08("Sphere scene with bounces", 1024,1024);
            //Project10 window = new Project10("Reduce: Max ", 20000, 0.1f, 1000.0f);
            //Project11 window = new Project11("Map function", 256 * 10);
            // window.Run();
            //HandsOn02 console = new HandsOn02();
            HandsOn03 console = new HandsOn03("Raytracer",512,512); // ray tracer 
            console.Run();


        }
    }
}
