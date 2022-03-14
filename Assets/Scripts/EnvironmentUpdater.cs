using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
using Astetrio.Spaceship.Interfaces;
using System.Linq;

namespace Astetrio.Spaceship
{
    public class EnvironmentUpdater : MonoBehaviour
    {
        private const string _Tint = "_Tint";

        [SerializeField] private Material _material = null;
        [SerializeField] private List<Star> _stars = null;
        [SerializeField] private InterfaceReference<IStarsGenerator> _generator = null;

        private void Update()
        {
            var color = new Color(0, 0, 0, 0);

            var n = 0;
            foreach (var star in _stars.Concat(_generator.Value.Stars))
            {
                if (star.IsRealSize)
                {
                    color += star.Color;
                    n++;
                }
            }

            if (n == 0)
            {
                return;
            }

            color /= n;
            Color.RGBToHSV(color, out var h, out _, out _);

            //s -= 0.05f; //30
            color = Color.HSVToRGB(h, 0.30f, 0.35f);

            _material.SetColor(_Tint, color);

            //s += 0.34f; //64
            
            color = Color.HSVToRGB(h, 0.64f, 0.54f);
            RenderSettings.fogColor = color;

            //s += 0.11f; //75

            color = Color.HSVToRGB(h, 0.75f, 0.35f);
            RenderSettings.ambientSkyColor = color;
        }
    }
}
