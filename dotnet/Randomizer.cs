using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTutorial
{
    class Randomizer
    {
        public static void Fill<T>(T[] data, T min, T max) where T : IFloatingPointIeee754<T>
        {
            var rng = Random.Shared;
            if (max < min) (min, max) = (max, min);

            T span = max - min;
            for (int i = 0; i < data.Length; ++i)
            {
                T u = T.CreateTruncating(rng.NextDouble());
                data[i] = min + u * span;
            }
        }
    }
}
