using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Extensions
{
    public static class LayerMaskExtensions
    {
        public static int GetLayer(this LayerMask layerMask)
        {
            int layerNumber = 0;
            int layer = layerMask.value;

            while (layer > 1)
            {
                layer >>= 1;
                layerNumber++;
            }

            return layerNumber;
        }
    }
}
