using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship
{
    [RequireComponent(typeof(MeshRenderer))]
    public class RandomSunColor : MonoBehaviour
    {
        [SerializeField] private Gradient _gradient = null;
        [SerializeField] private float _colorIntensity = 10;

        private MeshRenderer _renderer = null;
        private readonly List<MeshRenderer> _renderers = new List<MeshRenderer>();

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();

            GetComponentsInChildren(_renderers);

            _renderers.Remove(_renderer);
        }

        private void OnEnable()
        {
            //var newColor = Random.ColorHSV(0, 1, 0.8f, 0.8f, 1, 1) * Mathf.Pow(2, _colorIntensity);
            var newColor = _gradient.Evaluate(Random.value) * Mathf.Pow(2, _colorIntensity);

            var material = _renderer.material;
            material.color = newColor;

            foreach (var renderer in _renderers)
            {
                renderer.sharedMaterial = material;
            }
        }
    }
}
