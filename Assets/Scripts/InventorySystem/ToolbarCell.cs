using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VariableInventorySystem
{
    public class ToolbarCell : VariableInventoryCell
    {
        [SerializeField] Vector2 defaultCellSize;
        [SerializeField] Vector2 _iconPadding;

        [SerializeField] RectTransform sizeRoot;
        [SerializeField] RectTransform target;
        [SerializeField] Graphic background;
        [SerializeField] Image cellImage;
        [SerializeField] Graphic highlight;
        [SerializeField] TextMeshProUGUI _countText;

        [SerializeField] StandardButton button;

        public override Vector2 DefaultCellSize => defaultCellSize;
        public override Vector2 MargineSpace => Vector2.zero;
        protected override IVariableInventoryCellActions ButtonActions => button;
        protected virtual StandardAssetLoader Loader { get; set; }

        protected bool isSelectable = true;
        protected IVariableInventoryAsset currentImageAsset;

        private void Update()
        {
            ApplySize();
            //target.localEulerAngles = Vector3.forward * (CellData?.IsRotate ?? false ? 90 : 0);
        }

        public Vector2 GetCellSize()
        {
            return defaultCellSize;
        }

        public override Vector2 GetLocalPosition(Vector2 position, Camera camera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(background.rectTransform, position, camera, out var localPosition);
            return localPosition;
        }

        public override void SetSelectable(bool value)
        {
            ButtonActions.SetActive(value);
            isSelectable = value;
        }

        public override void SetHighlight(bool value)
        {
            highlight.gameObject.SetActive(value);
        }

        protected override void OnApply()
        {
            //SetHighlight(false);
            target.gameObject.SetActive(CellData != null);
            ApplySize();

            if (CellData == null)
            {
                _countText.gameObject.SetActive(false);
                cellImage.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
            }
            else
            {
                _countText.text = CellData.Count > 1 ? CellData.Count.ToString() : "";
                _countText.gameObject.SetActive(true);

                // update cell image
                if (currentImageAsset != CellData.ImageAsset)
                {
                    currentImageAsset = CellData.ImageAsset;

                    cellImage.gameObject.SetActive(false);
                    if (Loader == null)
                    {
                        Loader = new StandardAssetLoader();
                    }

                    StartCoroutine(Loader.LoadAsync(CellData.ImageAsset, sprite =>
                    {
                        cellImage.sprite = sprite;
                        cellImage.gameObject.SetActive(true);
                    }));
                }

                background.gameObject.SetActive(true && isSelectable);
            }
        }

        protected virtual void ApplySize()
        {
            sizeRoot.sizeDelta = GetCellSize();
            target.sizeDelta = GetCellSize() - _iconPadding;

            target.localEulerAngles = Vector3.zero;
        }
    }
}
