using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Interfaces;
using VariableInventorySystem;
using AYellowpaper;

namespace Astetrio.Spaceship.Models
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Astetrio/Spaceship/Recipe", order = 25)]
    public class Recipe : ScriptableObjectWithID, IRecipe
    {
        [SerializeField] private List<InterfaceReference<IItemInformation>> _components = new List<InterfaceReference<IItemInformation>>();
        [SerializeField] private InterfaceReference<IItemInformation> _result = null;

        public IReadOnlyList<IItemInformation> Componenets => _components.Select(x => x.Value).ToList();
        public IItemInformation Result => _result.Value;
    }
}
