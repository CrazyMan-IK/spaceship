using System;
using UnityEngine.EventSystems;

namespace VariableInventorySystem
{
    public interface IVariableInventoryView
    {
        IVariableInventoryViewData ViewData { get; }

        void SetCellCallback(
            Action<IVariableInventoryCell> onCellClick,
            Action<IVariableInventoryCell> onCellOptionClick,
            Action<IVariableInventoryCell> onCellEnter,
            Action<IVariableInventoryCell> onCellExit);

        int? GetEffectCellID();

        void Apply(IVariableInventoryViewData data);
        void ReApply();

        void OnPrePick(IVariableInventoryCell stareCell);
        ICellData OnPick(IVariableInventoryCell stareCell, PointerEventData.InputButton button);
        void OnDrag(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell, PointerEventData cursorPosition);
        bool? OnDrop(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell);
        void OnDroped(bool? isDroped);

        void OnCellEnter(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell);
        void OnCellExit(IVariableInventoryCell stareCell);

        void OnSwitchRotate(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell);
    }
}
