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

        /*public event Action ToggleInventoryUI = null;
        public event Action RotateInventoryItem = null;
        public event Action PickupItem = null;
        public event Action ToggleMainUI = null;*/

        [SerializeField] private Vector2 _mouseSensitivity = Vector2.one;

        private readonly IReadOnlyCollection<KeyCode> _keyCodes = new HashSet<KeyCode>(Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>());
        private readonly Dictionary<KeyCode, bool> _keys = new Dictionary<KeyCode, bool>();
        private readonly Dictionary<KeyCode, bool> _keysPressedInCurrentFrame = new Dictionary<KeyCode, bool>();
        private readonly Dictionary<KeyCode, bool> _keysReleasedInCurrentFrame = new Dictionary<KeyCode, bool>();

        public Vector3 Direction { get; private set; } = Vector3.zero;
        public Vector2 Rotation { get; private set; } = Vector2.zero;
        public IReadOnlyDictionary<KeyCode, bool> Keys => _keys;
        public IReadOnlyDictionary<KeyCode, bool> KeysPressedInCurrentFrame => _keysPressedInCurrentFrame;
        public IReadOnlyDictionary<KeyCode, bool> KeysReleasedInCurrentFrame => _keysReleasedInCurrentFrame;
        public bool IsEnabled { get; private set; } = true;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            foreach (var key in _keyCodes)
            {
                _keys[key] = false;
                _keysPressedInCurrentFrame[key] = false;
                _keysReleasedInCurrentFrame[key] = false;
            }
        }

        private void Update()
        {
            var x = Input.GetAxis(_HorizontalAxisName);
            var y = Input.GetAxis(_DepthAxisName);
            var z = Input.GetAxis(_VerticalAxisName);

            Direction = new Vector3(x, y, z).normalized;

            if (IsEnabled)
            {
                var mx = Input.GetAxis(_MouseXAxisName);
                var my = Input.GetAxis(_MouseYAxisName);

                Rotation = new Vector2(mx, my) * _mouseSensitivity;
            }
            else
            {
                Rotation = Vector2.zero;
            }

            foreach (var key in _keyCodes)
            {
                _keys[key] = Input.GetKey(key);
                _keysPressedInCurrentFrame[key] = Input.GetKeyDown(key);
                _keysReleasedInCurrentFrame[key] = Input.GetKeyUp(key);
            }
        }

        public void Enable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            IsEnabled = true;
        }

        public void Disable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            IsEnabled = false;
        }
    }
}
