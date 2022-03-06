using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using AYellowpaper;

namespace VariableInventorySystem
{
    public abstract class VariableInventoryCore<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : IVariableInventoryCellData
    {
        public event Action<T> DropedOutside = null;

        protected List<IVariableInventoryView> InventoryViews { get; set; } = new List<IVariableInventoryView>();

        protected virtual InterfaceReference<IVariableInventoryCell> CellPrefab { get; set; }
        protected virtual RectTransform EffectCellParent { get; set; }

        protected IVariableInventoryCell stareCell;
        protected IVariableInventoryCell effectCell;

        bool? originEffectCellRotate;
        Vector2 cursorPosition;

        private bool _isClicked = false;

        private void Update()
        {
            if (effectCell?.CellData != null && 
                (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && 
                _isClicked)
            {
                var droped = effectCell.CellData.Clone(1);
                effectCell.CellData.Merge(droped, new Quantity(-1));

                if (stareCell == null)
                {
                    DropedOutside?.Invoke((T)droped);
                }
                else
                {
                    /*var temp = effectCell.CellData;
                    effectCell.Apply(droped);

                    OnEndDrag(new PointerEventData(EventSystem.current));

                    effectCell.RectTransform.gameObject.SetActive(true);
                    effectCell.Apply(temp);

                    _isClicked = true;*/
                    
                    var temp = effectCell.CellData;
                    effectCell.Apply(droped);

                    foreach (var view in InventoryViews)
                    {
                        var result = view.OnDrop(stareCell, effectCell);

                        if (result.Value)
                        {
                            //isRelease = true;
                            break;
                        }

                        temp.Merge(droped, new Quantity(1));
                    }

                    effectCell.Apply(temp);
                }
                /*else if (stareCell.CellData == null)
                {
                    
                    stareCell.Apply(droped);
                }
                else
                {
                    stareCell.CellData.Merge(droped, new Quantity(1));
                }*/

                if (effectCell.CellData.Count < 1)
                {
                    effectCell.RectTransform.gameObject.SetActive(false);
                    effectCell.Apply(null);

                    _isClicked = false;
                }
                else
                {
                    effectCell.Apply(effectCell.CellData);
                }
            }
        }

        public virtual void Initialize()
        {
            effectCell = Instantiate(CellPrefab.UnderlyingValue, EffectCellParent) as IVariableInventoryCell;
            effectCell.RectTransform.gameObject.SetActive(false);
            effectCell.SetSelectable(false);
        }

        public virtual void AddInventoryView(IVariableInventoryView variableInventoryView)
        {
            /*while (InventoryViews.Count > 0)
            {
                var view = InventoryViews[0];
                RemoveInventoryView(view);
            }*/

            InventoryViews.Add(variableInventoryView);
            variableInventoryView.SetCellCallback(OnCellClick, OnCellOptionClick, OnCellEnter, OnCellExit);
        }

        public virtual void RemoveInventoryView(IVariableInventoryView variableInventoryView)
        {
            InventoryViews.Remove(variableInventoryView);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            /*if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }*/
            if (_isClicked)
            {
                eventData.Use();
                return;
            }

            foreach (var view in InventoryViews)
            {
                //view.Apply();

                view.OnPrePick(stareCell);
            }

            IVariableInventoryCellData stareData = null;

            bool isHold = false;
            foreach (var view in InventoryViews)
            {
                var data = view.OnPick(stareCell, eventData.button);
                if (data != null)
                {
                    isHold = true;
                    stareData = data;
                    break;
                }
            }

            if (!isHold)
            {
                return;
            }

            effectCell.RectTransform.gameObject.SetActive(true);
            effectCell.Apply(stareData);

            _isClicked = true;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!_isClicked || effectCell?.CellData == null)
            {
                return;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnDrag(stareCell, effectCell, eventData);
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(EffectCellParent, eventData.position, eventData.enterEventCamera, out cursorPosition);

            var (width, height) = GetRotateSize(effectCell.CellData);
            effectCell.RectTransform.localPosition = cursorPosition + new Vector2(
                 (width - 1) * effectCell.DefaultCellSize.x * -0.5f,
                (height - 1) * effectCell.DefaultCellSize.y * 0.5f);

            //stareCell.RectTransform.localEulerAngles = Vector3.forward * (stareCell.CellData?.IsRotate ?? false ? 90 : 0);
            //effectCell.RectTransform.localEulerAngles = Vector3.forward * (effectCell.CellData?.IsRotate ?? false ? 90 : 0);
        }

        (int, int) GetRotateSize(IVariableInventoryCellData cell)
        {
            return (cell.IsRotate ? cell.Height : cell.Width, cell.IsRotate ? cell.Width : cell.Height);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!_isClicked || effectCell.CellData == null)
            {
                return;
            }

            bool? isRelease = false;
            foreach (var view in InventoryViews)
            {
                var result = view.OnDrop(stareCell, effectCell);

                if (!result.HasValue)
                {
                    DropedOutside?.Invoke((T)effectCell.CellData);

                    isRelease = null;
                    break;
                }

                if (result.Value)
                {
                    isRelease = true;
                    break;
                }
            }

            if (isRelease.HasValue && !isRelease.Value && originEffectCellRotate.HasValue)
            {
                effectCell.CellData.IsRotate = originEffectCellRotate.Value;
            }
            originEffectCellRotate = null;

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnDroped(isRelease);
            }

            effectCell.RectTransform.gameObject.SetActive(false);
            effectCell.Apply(null);

            _isClicked = false;
        }

        public virtual void SwitchRotate()
        {
            if (effectCell.CellData == null)
            {
                return;
            }

            if (!originEffectCellRotate.HasValue)
            {
                originEffectCellRotate = effectCell.CellData.IsRotate;
            }

            effectCell.CellData.IsRotate = !effectCell.CellData.IsRotate;
            effectCell.Apply(effectCell.CellData);

            var (width, height) = GetRotateSize(effectCell.CellData);
            effectCell.RectTransform.localPosition = cursorPosition + new Vector2(
                 -(width - 1) * effectCell.DefaultCellSize.x * 0.5f,
                (height - 1) * effectCell.DefaultCellSize.y * 0.5f);

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnSwitchRotate(stareCell, effectCell);
            }
        }

        protected virtual void OnCellClick(IVariableInventoryCell cell)
        {
        }

        protected virtual void OnCellOptionClick(IVariableInventoryCell cell)
        {
        }

        protected virtual void OnCellEnter(IVariableInventoryCell cell)
        {
            stareCell = cell;

            foreach (var inventoryView in InventoryViews)
            {
                inventoryView.OnCellEnter(stareCell, effectCell);
            }
        }

        protected virtual void OnCellExit(IVariableInventoryCell cell)
        {
            foreach (var inventoryView in InventoryViews)
            {
                inventoryView.OnCellExit(stareCell);
            }

            stareCell = null;
        }
    }
}
