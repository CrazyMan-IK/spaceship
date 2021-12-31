using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Astetrio.Spaceship.Building
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Block))]
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private Connector _connectorPrefab = null;
        [SerializeField] private Material _normalMaterial = null;

        private BoxCollider _collider = null;
        private Renderer _renderer = null;
        private Block _block = null;

        public Block Block => _block;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _renderer = GetComponent<Renderer>();
            _block = GetComponent<Block>();
        }

        public void Show()
        {
            _renderer.enabled = true;
        }

        public void Hide()
        {
            _renderer.enabled = false;
        }

        public void Paste(Transform parent, int layerMask)
        {
            var newBlock = Instantiate(this, transform.position, transform.rotation, parent);

            newBlock.Show();
            SetLayerRecursively(newBlock.gameObject, LayerMask.NameToLayer("Default"));
            newBlock._renderer.material = _normalMaterial;

            foreach (var point in newBlock.Block.Connections)
            {
                if (point.TryGetCollidingConnection(layerMask, out var connection))
                {
                    var otherBlock = connection.GetComponentInParent<Block>();

                    //var connector = Instantiate(_connectorPrefab, (transform.position + otherBlock.transform.position) / 2, Quaternion.LookRotation(transform.position - otherBlock.transform.position, otherBlock.transform.forward), parent);
                    //var connector = Instantiate(_connectorPrefab, point.transform.position, Quaternion.LookRotation(transform.position - otherBlock.transform.position, otherBlock.transform.forward), parent);
                    //var connector = Instantiate(_connectorPrefab, point.transform.position, point.transform.rotation * Quaternion.AngleAxis(90, Vector3.right), parent);
                    var connector = Instantiate(_connectorPrefab, point.transform.position, Quaternion.identity, parent);
                    connector.Connect(newBlock.Block, otherBlock);
                }
            }

            Destroy(newBlock);

            //UnityEditor.EditorApplication.isPaused = true;
        }

        public bool IsCollideWithAnything(int layerMask)
        {
            return Physics.CheckBox(transform.position + _collider.center, Vector3.Scale(_collider.size, transform.lossyScale) / 2.01f, transform.rotation, layerMask, QueryTriggerInteraction.Ignore);
        }

        private void SetLayerRecursively(GameObject go, int layerMask)
        {
            go.layer = layerMask;
            foreach (Transform child in go.transform)
            {
                SetLayerRecursively(child.gameObject, layerMask);
            }
        }

        /*private void OnDrawGizmos()
        {
            Gizmos.matrix *= Matrix4x4.Translate(transform.position);
            Gizmos.matrix *= Matrix4x4.Rotate(transform.rotation);
            Gizmos.matrix *= Matrix4x4.Translate(-transform.position);
            Gizmos.DrawCube(transform.position + _collider.center, Vector3.Scale(_collider.size, transform.lossyScale) / 1.005f);
        }*/

        /*private void OnCollisionEnter(Collision collision)
        {
            _isColliding = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            _isColliding = false;
        }*/
    }
}
