using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

namespace Astetrio.Spaceship
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Star : MonoBehaviour
    {
        [SerializeField] private SphereCollider _spawnCollision = null;
        [SerializeField] private Transform _target = null;
        [SerializeField] private float _triggerSize = 6144;
        [SerializeField] private float _maxDistance = 16384;

        private readonly List<MeshRenderer> _renderers = new List<MeshRenderer>();

        private DVector3 _basePosition = DVector3.Zero;
        private Vector3 _baseScale = Vector3.zero;

        private void Awake()
        {
            GetComponentsInChildren(_renderers);

            _basePosition = transform.position;
            _baseScale = transform.localScale;
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            var baseDistance = DVector3.Distance(_basePosition, _target.position);

            if (baseDistance > _maxDistance)
            {
                var multiplier = _maxDistance / baseDistance;
                if (multiplier <= 0.385f)
                {
                    DisableRenderers();

                    return;
                }
                EnableRenderers();

                var direction = _target.position - _basePosition;
                direction.Normalize();

                transform.position = (Vector3)(_basePosition + direction * (baseDistance - _maxDistance));
                transform.localScale = (float)(Math.Max(-0.6 + (1 + 0.6) * Math.Min(Math.Max(multiplier, 0), 1), 0) * multiplier) * _baseScale;
            }
            else
            {
                transform.position = (Vector3)_basePosition;
                transform.localScale = _baseScale;
            }

            UpdateTrigger();
        }

        public void Initialize(Transform target)
        {
            _target = target;

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
        }
    }
}
