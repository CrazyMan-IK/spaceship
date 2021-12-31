using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Interfaces
{
    public interface IInputPresenter
    {
        Vector3 Direction { get; }
        Vector2 Rotation { get; }
        IReadOnlyDictionary<KeyCode, bool> Keys { get; }
    }
}
