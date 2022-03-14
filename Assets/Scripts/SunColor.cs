using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SunColor : MonoBehaviour
    {
        [SerializeField] private Gradient _gradient = null;
        [SerializeField] private float _colorIntensity = 10;
        [SerializeField] private Light _light = null;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private bool _isRandom = true;

        private MeshRenderer _renderer = null;
        private readonly List<MeshRenderer> _renderers = new List<MeshRenderer>();

        public Color Color { get; private set; }

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();

            GetComponentsInChildren(_renderers);

            _renderers.Remove(_renderer);
        }

        private void OnEnable()
        {
            //var newColor = Random.ColorHSV(0, 1, 0.8f, 0.8f, 1, 1) * Mathf.Pow(2, _colorIntensity);
            var newColor = _defaultColor;
            if (_isRandom)
            {
                newColor = _gradient.Evaluate(Random.value);
            }
            Color = newColor;
            _light.color = newColor;

            var material = _renderer.material;
            material.color = newColor * Mathf.Pow(2, _colorIntensity);

            foreach (var renderer in _renderers)
            {
                renderer.sharedMaterial = material;
            }
        }
    }
}
