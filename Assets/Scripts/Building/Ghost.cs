using UnityEngine;
using Astetrio.Spaceship.Extensions;

namespace Astetrio.Spaceship.Building
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Block))]
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private Material _normalMaterial = null;
        [SerializeField] private LayerMask _defaultMask = default;
        [SerializeField] private LayerMask _withoutCollisionsMask = default;
        [SerializeField] private BoxCollider _outerCollider = null;
        [SerializeField] private GameObject _connectionsParent = null;

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
            /*SetLayerRecursively(newBlock.gameObject, LayerMask.NameToLayer("Default"));
            newBlock.gameObject.layer = _defaultMask >> 1;*/
            SetLayerRecursively(newBlock._connectionsParent, LayerMask.NameToLayer("Default"));
            newBlock._outerCollider.gameObject.layer = _withoutCollisionsMask.GetLayer();
            newBlock.gameObject.layer = _defaultMask.GetLayer();
            newBlock._renderer.material = _normalMaterial;

            Destroy(newBlock);

            //UnityEditor.EditorApplication.isPaused = true;
        }

        public bool IsCollideWithAnything(int layerMask)
        {
            return Physics.OverlapBox(transform.position + _collider.center, Vector3.Scale(_collider.size, transform.lossyScale) / 2.01f, transform.rotation, layerMask, QueryTriggerInteraction.Ignore).Length > 0;
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
