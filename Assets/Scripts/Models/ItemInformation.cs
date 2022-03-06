using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Interfaces;

namespace Astetrio.Spaceship.Models
{
    [CreateAssetMenu(fileName = "New ItemInformation", menuName = "Astetrio/Spaceship/Item Information", order = 50)]
    public class ItemInformation : ScriptableObjectWithID, IItemInformationPresenter, IItemInformation
    {
        [SerializeField] private string _name = null;
        [SerializeField] private Vector2Int _cellSize = Vector2Int.zero;
        [SerializeField, Min(1)] private int _stackSize = 1;
        [SerializeField] private Sprite _icon = null;
        [SerializeField] private Mesh _mesh = null;
        [SerializeField] private List<Material> _materials = new List<Material>();

        public string Name => _name;
        public Vector2Int CellSize => _cellSize;
        public int StackSize => _stackSize;
        public Sprite Icon => _icon;
        public Mesh Mesh => _mesh;
        public List<Material> Materials => _materials;

        public IItemInformation GetInformation()
        {
            return this;
        }
    }
}
