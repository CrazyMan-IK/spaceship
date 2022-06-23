using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Extensions
{
    public static class MathExtensions
    {
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }

        public static double Remap(this double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }
    }
}
