using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Interfaces
{
    public interface IInputPresenter
    {
        public event Action ToggleInventoryUI;
        public event Action RotateInventoryItem;
        public event Action PickupItem;
        public event Action ToggleMainUI;

        Vector3 Direction { get; }
        Vector2 Rotation { get; }
        //IReadOnlyDictionary<KeyCode, bool> Keys { get; }
        //IReadOnlyDictionary<KeyCode, bool> KeysPressedInCurrentFrame { get; }
        //IReadOnlyDictionary<KeyCode, bool> KeysReleasedInCurrentFrame { get; }
        bool IsEnabled { get; }
        bool SlowDownPressed { get; }

        void Enable();
        void Disable();
    }
}
