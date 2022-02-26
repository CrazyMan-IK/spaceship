using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Astetrio.Spaceship.Building
{
    [RequireComponent(typeof(Collider))]
    public class ConnectionPoint : MonoBehaviour
    {
        [SerializeField] private bool _canOverlap = true;
        [SerializeField] private Vector3 _colliderOffset = Vector3.zero;

        private Collider _collider = null;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public bool TryGetCollidingConnection(int layerMask, out List<ConnectionPoint> others)
        {
            others = new List<ConnectionPoint>();

            if (!_canOverlap)
            {
                return false;
            }

            var results = Physics.OverlapSphere(transform.position + transform.rotation * _colliderOffset + transform.forward * 0.5f, 0.1f, layerMask);

            if (results.Length > 0)
            {
                var collidingConnections = results.Select(x => x.GetComponent<ConnectionPoint>()).Where(x => x != null && x.transform.parent != transform.parent).ToList();
                if (collidingConnections.Count > 0)
                {
                    others = collidingConnections;
                    return true;
                }
            }

            return false;
        }

        /*private void OnDrawGizmosSelected()
        {
            *//*Gizmos.matrix *= Matrix4x4.Translate(transform.position);
            Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
            Gizmos.matrix *= Matrix4x4.Translate(-transform.position);
            Gizmos.DrawCube(transform.position + _collider.center, Vector3.Scale(_collider.size, transform.lossyScale) / 1.005f);*//*

            //Gizmos.matrix *= Matrix4x4.TRS(transform.position + _collider.center, transform.rotation, Vector3.Scale(_collider.size, transform.lossyScale) / 1.005f);
            Gizmos.matrix *= Matrix4x4.TRS(transform.TransformPoint(_collider.center), transform.rotation, transform.TransformVector(_collider.size / 1.005f));
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }*/
    }
}
