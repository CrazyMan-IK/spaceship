using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Interfaces
{
    public interface IStarsGenerator
    {
        void StartGeneration(Vector3Int position, int additionalSeed);
    }
}
