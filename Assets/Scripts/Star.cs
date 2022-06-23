using Astetrio.Spaceship.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using URandom = UnityEngine.Random;

namespace Astetrio.Spaceship
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(SunColor))]
    public class Star : MonoBehaviour
    {
        [SerializeField] private SphereCollider _spawnCollision = null;
        [SerializeField] private Light _light = null;
        [SerializeField] private Transform _target = null;
        [SerializeField] private float _triggerSize = 6144;
        [SerializeField] private float _maxDistance = 16384;

        private readonly List<MeshRenderer> _renderers = new List<MeshRenderer>();
        private MeshRenderer _renderer = null;
        private SunColor _sunColor = null;
        private bool _isActive = true;

        private DVector3 _basePosition = DVector3.Zero;
        private Vector3 _baseScale = Vector3.zero;

        public MeshRenderer Renderer => _renderer;
        public Color Color => _sunColor.Color;
        public float MaxDistance => _maxDistance;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;

                    if (_isActive)
                    {
                        EnableRenderers();
                    }
                    else
                    {
                        DisableRenderers();
                    }
                }
            }
        }
        public bool IsRealSize { get; private set; }
        public double Distance { get; private set; }

        private void Awake()
        {
            GetComponentsInChildren(_renderers);
            _renderer = GetComponent<MeshRenderer>();
            _sunColor = GetComponent<SunColor>();

            _basePosition = transform.position;
            _baseScale = transform.localScale;

            if (!enabled)
            {
                DisableRenderers();
            }
        }

        private void OnEnable()
        {
            EnableRenderers();
        }

        private void OnDisable()
        {
            DisableRenderers();
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            /*Func<double, double> easing = Easing.Lerp;
            if (Input.GetKey(KeyCode.Z))
            {
                easing = Easing.ExpoIn;
            }
            if (Input.GetKey(KeyCode.X))
            {
                easing = Easing.ExpoOut;
            }
            if (Input.GetKey(KeyCode.C))
            {
                easing = Easing.SineOutIn;
            }
            if (Input.GetKey(KeyCode.V))
            {
                easing = Easing.QuadOutIn;
            }
            if (Input.GetKey(KeyCode.B))
            {
                easing = Easing.CubicOutIn;
            }
            if (Input.GetKey(KeyCode.N))
            {
                easing = Easing.ExpoOutIn;
            }*/

            var direction = _target.position - _basePosition;
            Distance = direction.Magnitude;

            if (Distance > _maxDistance)
            {
                IsRealSize = false;

                _light.gameObject.SetActive(false);

                //_light.intensity = Convert.ToSingle(Math.Clamp(Distance.Remap(0, MaxDistance, 5.65, 0.0), 0, 1));
                //_light.color = _sunColor.Color * _light.intensity * 0.5f;

                var multiplier = _maxDistance / Distance;
                if (multiplier <= 0.26f)
                {
                    IsActive = false;
                    //DisableRenderers();

                    return;
                }

                IsActive = true;
                //EnableRenderers();

                direction /= Distance;

                transform.position = (Vector3)(_basePosition + direction * (Distance - _maxDistance));
                //transform.localScale = (float)(Math.Max(-0.6 + (1 + 0.6) * Math.Min(Math.Max(easing(multiplier), 0), 1), 0) * multiplier) * _baseScale;
                transform.localScale = (float)(Math.Max(-0.69 + 1.69 * Easing.ExpoOutInOptimized(multiplier), 0) * multiplier) * _baseScale;
            }
            else
            {
                IsRealSize = true;

                transform.position = (Vector3)_basePosition;
                transform.localScale = _baseScale;

                //_light.intensity = Convert.ToSingle(Math.Clamp(Distance.Remap(0, MaxDistance, 5.65, 0.0), 0, 1));
                _light.intensity = Convert.ToSingle(Math.Clamp(Distance.Remap(0, MaxDistance, 1.7, 0.0), 0, 1));
                _light.color = _sunColor.Color * _light.intensity * 0.5f;
                _light.gameObject.SetActive(true);
            }

            UpdateTrigger();
        }

        public void Initialize(Transform target)
        {
            _target = target;

            _light.GetComponent<LookAtConstraint>().SetSource(0, new ConstraintSource() { sourceTransform = _target, weight = 1 });

            if (enabled)
            {
                UpdateTrigger();
            }
        }

        public void AddBaseOffset(Vector3 offset)
        {
            _basePosition += offset;
        }

        private void EnableRenderers()
        {
            foreach (var renderer in _renderers)
            {
                renderer.enabled = true;
            }
        }

        private void DisableRenderers()
        {
            foreach (var renderer in _renderers)
            {
                renderer.enabled = false;
            }
        }

        private void UpdateTrigger()
        {
            _spawnCollision.radius = _triggerSize / transform.localScale.x;
            _spawnCollision.center = (Vector3)(_basePosition - transform.position) / transform.localScale.x;
        }
    }
}
