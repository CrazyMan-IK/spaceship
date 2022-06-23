using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
using Astetrio.Spaceship.Interfaces;
using System.Linq;
using Astetrio.Spaceship.Extensions;

namespace Astetrio.Spaceship
{
    public class EnvironmentUpdater : MonoBehaviour
    {
        private const string _Tint = "_Tint";

        //[SerializeField] private float _changingSpeedMultiplier = 5;
        [SerializeField] private double _remapToMax = 1.5;
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
                    color += star.Color * Convert.ToSingle(Math.Clamp(star.Distance.Remap(0, star.MaxDistance, _remapToMax, 0.0), 0, 1));
                    n++;
                }
            }

            if (n == 0)
            {
                return;
            }

            color /= n;
            Color.RGBToHSV(color, out var h, out var s, out var v);

            s += 0.34f; //30 //+24 //+4
            v += 0.20f;

            color = Color.HSVToRGB(h, s, Mathf.Max(v, 0.35f)); //h 30 35
            _material.SetColor(_Tint, color);
            //_material.SetColor(_Tint, Color.Lerp(_material.GetColor(_Tint), color, Time.deltaTime * _changingSpeedMultiplier));

            s += 0.34f; //64
            v += 0.19f;

            color = Color.HSVToRGB(h, s, Mathf.Max(v, 0.54f)); //h 64 54
            RenderSettings.fogColor = color;
            //RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, color, Time.deltaTime * _changingSpeedMultiplier);

            s += 0.11f; //75
            v -= 0.19f;

            color = Color.HSVToRGB(h, s, Mathf.Max(v, 0.35f)); //h 75 35
            RenderSettings.ambientSkyColor = color;
            //RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, color, Time.deltaTime * _changingSpeedMultiplier);
        }
    }
}
