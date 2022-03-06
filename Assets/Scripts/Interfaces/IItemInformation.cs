using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astetrio.Spaceship.Interfaces
{
    public interface IItemInformation
    {
        public string Name { get; }
        public Vector2Int CellSize { get; }
        public int StackSize { get; }
        public Sprite Icon { get; }
        public Mesh Mesh { get; }
        public List<Material> Materials { get; }
        public string InternalID { get; }
    }
}
