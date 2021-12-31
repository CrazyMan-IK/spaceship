using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Astetrio.Spaceship.Building
{
    [RequireComponent(typeof(BoxCollider))]
    public class ConnectionPoint : MonoBehaviour
    {
        [SerializeField] private bool _canOverlap = true;

        private BoxCollider _collider = null;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }

        public bool TryGetCollidingConnection(int layerMask, out ConnectionPoint other)
        {
            if (!_canOverlap)
            {
                other = null;
                return false;
            }

            //var results = Physics.OverlapBox(transform.position + transform.rotation * _collider.center + transform.forward * 0.1f, Vector3.Scale(_collider.size, transform.lossyScale) / 2.01f, transform.rotation, layerMask);
            //var results1 = Physics.OverlapBox(transform.position + transform.rotation * _collider.center + transform.forward * 0.1f, Vector3.Scale(_collider.size, transform.lossyScale) / 2.01f, transform.rotation, layerMask);
            //var results2 = Physics.OverlapBox(transform.position + transform.rotation * _collider.center + transform.forward * 0.5f, Vector3.one * 0.25f, transform.rotation, layerMask);
            var results = Physics.OverlapSphere(transform.position + transform.rotation * _collider.center + transform.forward * 0.2f, 0.1f, layerMask);

            //var results = results1.Concat(results2).ToArray();

            if (results.Length > 0)
            {
                var collidingConnections = results.Select(x => x?.GetComponent<ConnectionPoint>()).Where(x => x != null && x.transform.parent != transform.parent).ToList();
                if (collidingConnections.Count > 0)
                {
                    other = collidingConnections.First();
                    return true;
                }
            }

            other = null;
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
