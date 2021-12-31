using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Interfaces;
using System;
using System.Linq;

namespace Astetrio.Spaceship
{
    public class KeyboardInput : MonoBehaviour, IInputPresenter
    {
        private const string _HorizontalAxisName = "Horizontal";
        private const string _VerticalAxisName = "Vertical";
        private const string _DepthAxisName = "Depth";
        private const string _MouseXAxisName = "Mouse X";
        private const string _MouseYAxisName = "Mouse Y";

        [SerializeField] private Vector2 _mouseSensitivity = Vector2.one;

        private readonly IReadOnlyCollection<KeyCode> _keyCodes = new HashSet<KeyCode>(Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>());
        private readonly Dictionary<KeyCode, bool> _keys = new Dictionary<KeyCode, bool>();// Enum.GetValues(typeof(KeyCode)).Length);

        public Vector3 Direction { get; private set; }
        public Vector2 Rotation { get; private set; }
        public IReadOnlyDictionary<KeyCode, bool> Keys => _keys;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            foreach (var key in _keyCodes)
            {
                _keys[key] = false;
            }
        }

        private void Update()
        {
            var x = Input.GetAxis(_HorizontalAxisName);
            var y = Input.GetAxis(_DepthAxisName);
            var z = Input.GetAxis(_VerticalAxisName);

            Direction = new Vector3(x, y, z).normalized;

            var mx = Input.GetAxis(_MouseXAxisName);
            var my = Input.GetAxis(_MouseYAxisName);

            Rotation = new Vector2(mx, my) * _mouseSensitivity;

            foreach (var key in _keyCodes)
            {
                _keys[key] = Input.GetKey(key);
            }
        }
    }
}
