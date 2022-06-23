using AYellowpaper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Interfaces;

namespace Astetrio.Spaceship
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IInputPresenter> _input = null;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private Transform _verticalAxisPivot = null;
        [SerializeField] private Transform _horizontalAxisPivot = null;
        [SerializeField] private float _acceleration = 10;

        private Rigidbody _rigidbody = null;
        //private Vector3 _velocity = Vector3.zero;
        private Vector2 _currentCameraRotation = Vector2.zero;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _currentCameraRotation.y = _horizontalAxisPivot.localEulerAngles.x;
            _currentCameraRotation.x = _verticalAxisPivot.localEulerAngles.y;
        }

        /*private void Update()
        {
            var direction = _input.Value.Direction * _acceleration * Time.deltaTime;
            _velocity += _rigidbody.rotation * direction;
        }*/

        private void FixedUpdate()
        {
            var direction = _acceleration * Time.deltaTime * _input.Value.Direction;
            //var rotation = _input.Value.Rotation * Time.deltaTime;

            var velocity = _camera.transform.rotation * direction;

            _rigidbody.AddForce(velocity, ForceMode.Acceleration);

            if (_input.Value.SlowDownPressed)
            {
                _rigidbody.AddForce(-_rigidbody.velocity * 2, ForceMode.Acceleration);
            }

            //_rigidbody.MovePosition(_rigidbody.position + _velocity);
            //_rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.AngleAxis(rotation.x, Vector3.up));
            //_rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.AngleAxis(rotation.x, Vector3.up));
        }

        private void LateUpdate()
        {
            var rotation = _input.Value.Rotation;// * Time.deltaTime;
            _currentCameraRotation.x += rotation.x;
            _currentCameraRotation.y = Mathf.Clamp(_currentCameraRotation.y - rotation.y, -90, 90);

            _horizontalAxisPivot.localEulerAngles = new Vector3(_currentCameraRotation.y, 0, 0);
            _verticalAxisPivot.localEulerAngles = new Vector3(0, _currentCameraRotation.x, 0);

            //Debug.Log(Time.deltaTime);
        }
    }
}
