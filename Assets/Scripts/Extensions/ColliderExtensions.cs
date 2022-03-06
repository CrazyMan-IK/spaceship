using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Extensions
{
    public static class ColliderExtensions
    {
        public static Bounds GetLocalBounds(this Collider collider)
        {
            switch (collider)
            {
                case BoxCollider box:
                    return new Bounds(box.center, box.size);
                case MeshCollider mesh:
                    return mesh.sharedMesh.bounds;
            }

            throw new ArgumentException($"Unsupported collider type: {collider.GetType()}");
        }
    }
}
