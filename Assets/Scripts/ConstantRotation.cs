using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship
{
    public class ConstantRotation : MonoBehaviour
    {
        [SerializeField] private Vector3 _speed = Vector3.zero;

        private void Update()
        {
            transform.rotation *= Quaternion.Euler(_speed * Time.deltaTime);
        }
    }
}