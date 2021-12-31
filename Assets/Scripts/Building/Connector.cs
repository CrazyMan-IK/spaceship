using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Models;

namespace Astetrio.Spaceship.Building
{
    [RequireComponent(typeof(MeshFilter))]
    public class Connector : MonoBehaviour
    {
        [SerializeField] private ConnectorTypes _types = null;
        [SerializeField] private Connection _connection = null;

        private MeshFilter _filter = null;

        private void Awake()
        {
            _filter = GetComponent<MeshFilter>();
        }

        public void Connect(Block first, Block second)
        {
            Connect(first, second, null);
        }

        public void Connect(Block first, Block second, Block third)
        {
            Connect(first, second, third, null);
        }

        public void Connect(Block first, Block second, Block third, Block fourth)
        {
            _connection = new Connection(first, second, third, fourth);

            if (first != null && second != null)
            {
                if (first.transform.forward == second.transform.forward)
                {
                    _filter.mesh = _types.Default;
                    transform.rotation = Quaternion.LookRotation(first.transform.position - second.transform.position, second.transform.forward);
                    return;
                }

                var forward = (first.transform.position - transform.position).normalized;
                var upwards = (second.transform.position - transform.position).normalized;

                transform.rotation = Quaternion.LookRotation(forward, upwards);
            }

            int count = first != null ? 1 : 0;
            count += second != null ? 1 : 0;
            count += third != null ? 1 : 0;
            count += fourth != null ? 1 : 0;

            if (_types.Types.TryGetValue(count, out var mesh))
            {
                _filter.mesh = mesh;
                return;
            }

            _filter.mesh = _types.Default;
        }
    }
}
