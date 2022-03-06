using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Interfaces;

namespace Astetrio.Spaceship.Models
{
    [CreateAssetMenu(fileName = "New RandomItemInformation", menuName = "Astetrio/Spaceship/Random Item Information", order = 61)]
    public class RandomItemInformation : ScriptableObject, IItemInformationPresenter
    {
        [SerializeField] private List<ItemInformation> _items = new List<ItemInformation>();

        public IItemInformation GetInformation()
        {
            return _items[Random.Range(0, _items.Count)];
        }
    }
}
