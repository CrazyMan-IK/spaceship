using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Building
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private bool _canAutoRotate = false;

        private readonly List<ConnectionPoint> _connections = new List<ConnectionPoint>();

        public bool CanAutoRotate => _canAutoRotate;
        public IReadOnlyList<ConnectionPoint> Connections => _connections;

        private void OnEnable()
        {
            _connections.AddRange(GetComponentsInChildren<ConnectionPoint>());
        }

        private void OnDisable()
        {
            _connections.Clear();
        }

        public ConnectionPoint GetClosestPoint(Vector3 position)
        {
            position = Quaternion.Inverse(transform.localRotation) * position;

            if (_connections.Count <= 0)
            {
                return null;
            }

            ConnectionPoint closest = null;
            var closestDistance = float.MaxValue;
            foreach (var connection in _connections)
            {
                var currentDistance = Vector3.Distance(position, connection.transform.localPosition);
                if (currentDistance < closestDistance)
                {
                    closest = connection;
                    closestDistance = currentDistance;
                }
            }

            return closest;
        }
    }
}
