using System;
using System.Collections;
using UnityEngine;

namespace VariableInventorySystem
{
    public class StandardAssetLoader
    {
        public virtual IEnumerator LoadAsync(IVariableInventoryAsset imageAsset, Action<Sprite> onLoad)
        {
            if (imageAsset is SpriteAsset spriteAsset)
            {
                onLoad(spriteAsset.Sprite);

                yield break;
            }

            var loader = Resources.LoadAsync<Texture2D>((imageAsset as StandardAsset).Path);
            yield return loader;
            //onLoad(loader.asset as Texture2D);
            var tex = loader.asset as Texture2D;
            onLoad(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2));
        }
    }
}