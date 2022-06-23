using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VariableInventorySystem;
using Astetrio.Spaceship.Interfaces;
using Astetrio.Spaceship.InventorySystem;

namespace Astetrio.Spaceship.Models
{
    [CreateAssetMenu(fileName = "New ItemInformation", menuName = "Astetrio/Spaceship/Item Information", order = 50)]
    public class ItemInformation : ScriptableObjectWithID, IItemInformationPresenter, IItemInformation
    {
        [field: SerializeField] public string Name { get; private set; } = null;
        [field: SerializeField] public Vector2Int CellSize { get; private set; } = Vector2Int.zero;
        [field: SerializeField, Min(1)] public int StackSize { get; private set; } = 1;
        [field: SerializeField] public Sprite Icon { get; private set; } = null;
        [field: SerializeField] public Mesh Mesh { get; private set; } = null;
        [field: SerializeField] public List<Material> Materials { get; private set; } = new List<Material>();

        public ICellData AsCellData()
        {
            return new CustomItemCellData(this);
        }

        public ICellData AsCellData(int count)
        {
            return new CustomItemCellData(this, count);
        }

        public IItemInformation GetInformation()
        {
            return this;
        }
    }
}
