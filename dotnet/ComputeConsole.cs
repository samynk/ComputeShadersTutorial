using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    abstract class ComputeConsole: GameWindow
    {
        public ComputeConsole(String title)
            :base(GameWindowSettings.Default,
                 new NativeWindowSettings()
                 {
                     ClientSize = new OpenTK.Mathematics.Vector2i(1,1),
                     Title = title,
                     StartVisible = false,
                     APIVersion = new System.Version(4, 3), // Need at least 4.3 for compute shaders
                 })
        { 
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.On;
            Init();
            Compute();
        }

        protected abstract void Init();
        protected abstract void Compute();

    }
}
