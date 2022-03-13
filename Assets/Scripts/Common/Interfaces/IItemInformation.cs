using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VariableInventorySystem;

namespace Astetrio.Spaceship.Interfaces
{
    public interface IItemInformation
    {
        string Name { get; }
        Vector2Int CellSize { get; }
        int StackSize { get; }
        Sprite Icon { get; }
        Mesh Mesh { get; }
        List<Material> Materials { get; }
        string InternalID { get; }

        ICellData AsCellData();
        ICellData AsCellData(int count);
    }
}
