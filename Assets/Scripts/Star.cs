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
    public class Star : MonoBehaviour
    {
        [SerializeField] private SphereCollider _spawnCollision = null;
        [SerializeField] private Light _light = null;
        [SerializeField] private Transform _target = null;
        [SerializeField] private float _triggerSize = 6144;
        [SerializeField] private float _maxDistance = 16384;

        private readonly List<MeshRenderer> _renderers = new List<MeshRenderer>();

        private DVector3 _basePosition = DVector3.Zero;
        private Vector3 _baseScale = Vector3.zero;

        public bool IsActive { get; private set; }

        private void Awake()
        {
            GetComponentsInChildren(_renderers);

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

            var baseDistance = DVector3.Distance(_basePosition, _target.position);

            if (baseDistance > _maxDistance)
            {
                _light.gameObject.SetActive(false);

                var multiplier = _maxDistance / baseDistance;
                if (multiplier <= 0.26f)
                {
                    IsActive = false;
                    DisableRenderers();

                    return;
                }

                IsActive = true;
                EnableRenderers();

                var direction = _target.position - _basePosition;
                direction.Normalize();

                transform.position = (Vector3)(_basePosition + direction * (baseDistance - _maxDistance));
                //transform.localScale = (float)(Math.Max(-0.6 + (1 + 0.6) * Math.Min(Math.Max(easing(multiplier), 0), 1), 0) * multiplier) * _baseScale;
                transform.localScale = (float)(Math.Max(-0.69 + (1 + 0.69) * Easing.ExpoOutInOptimized(multiplier), 0) * multiplier) * _baseScale;
            }
            else
            {
                transform.position = (Vector3)_basePosition;
                transform.localScale = _baseScale;

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
