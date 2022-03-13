using System;
using UnityEngine;

namespace VariableInventorySystem
{
    public class Quantity
    {
        public Quantity(float value)
        {
            Value = Math.Sign(value);
        }

        public int Value { get; private set; }
    }
}
