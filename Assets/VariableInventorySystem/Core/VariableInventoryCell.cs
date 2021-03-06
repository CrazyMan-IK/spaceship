using System;
using UnityEngine;

namespace VariableInventorySystem
{
    public abstract class VariableInventoryCell : MonoBehaviour, IVariableInventoryCell
    {
        public RectTransform RectTransform => (RectTransform)transform;
        public ICellData CellData { get; protected set; }

        public virtual Vector2 DefaultCellSize { get; set; }
        public virtual Vector2 MargineSpace { get; set; }

        protected virtual IVariableInventoryCellActions ButtonActions { get; set; }

        public virtual void SetCellCallback(
            Action<IVariableInventoryCell> onPointerClick,
            Action<IVariableInventoryCell> onPointerOptionClick,
            Action<IVariableInventoryCell> onPointerEnter,
            Action<IVariableInventoryCell> onPointerExit,
            Action<IVariableInventoryCell> onPointerDown,
            Action<IVariableInventoryCell> onPointerUp)
        {
            ButtonActions.SetCallback(
                () => onPointerClick?.Invoke(this),
                () => onPointerOptionClick?.Invoke(this),
                () => onPointerEnter?.Invoke(this),
                () => onPointerExit?.Invoke(this),
                () => onPointerDown?.Invoke(this),
                () => onPointerUp?.Invoke(this));
        }

        public void Apply(ICellData cellData)
        {
            CellData = cellData;
            OnApply();
        }

        public abstract Vector2 GetLocalPosition(Vector2 position, Camera camera);

        public abstract void SetSelectable(bool value);
        public abstract void SetHighlight(bool value);

        protected abstract void OnApply();
    }
}
