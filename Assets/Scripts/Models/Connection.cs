using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astetrio.Spaceship.Building;

namespace Astetrio.Spaceship.Models
{
    [Serializable]
    public class Connection
    {
        [SerializeField] private Block _first = null;
        [SerializeField] private Block _second = null;
        [SerializeField] private Block _third = null;
        [SerializeField] private Block _fourth = null;

        public Block First => _first;
        public Block Second => _second;
        public Block Third => _third;
        public Block Fourth => _fourth;

        public Connection(Block first, Block second) : this(first, second, null)
        {

        }

        public Connection(Block first, Block second, Block third) : this(first, second, third, null)
        {

        }

        public Connection(Block first, Block second, Block third, Block fourth)
        {
            _first = first;
            _second = second;
            _third = third;
            _fourth = fourth;
        }
    }
}
