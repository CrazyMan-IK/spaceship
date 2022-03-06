using UnityEngine;

namespace VariableInventorySystem
{
    public class SpriteAsset : IVariableInventoryAsset
    {
        public Sprite Sprite { get; }

        public SpriteAsset(Sprite sprite)
        {
            Sprite = sprite;
        }
    }
}
