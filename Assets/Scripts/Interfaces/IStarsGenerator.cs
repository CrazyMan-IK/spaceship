using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Interfaces
{
    public interface IStarsGenerator
    {
        IReadOnlyList<Star> Stars { get; }
        bool IsGenerating { get; }

        void StartGeneration(Vector3Int position, int additionalSeed);
    }
}
