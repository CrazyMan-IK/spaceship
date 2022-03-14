using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship
{
    [RequireComponent(typeof(Rigidbody))]
    public class SpaceShip : MonoBehaviour
    {
        private Rigidbody _rigidbody = null;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            UpdateWeight();
        }

        private void OnTransformChildrenChanged()
        {
            UpdateWeight();
        }

        public void UpdateWeight()
        {
            _rigidbody.mass = transform.childCount * 100;
        }
    }
}
