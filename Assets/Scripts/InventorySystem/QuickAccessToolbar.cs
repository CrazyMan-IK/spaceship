using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using VariableInventorySystem;

namespace Astetrio.Spaceship.InventorySystem
{
    public class QuickAccessToolbar : MonoBehaviour, IVariableInventoryView
    {
        [SerializeField] ToolbarCell _cellPrefab;
        [SerializeField] GridLayoutGroup _gridLayoutGroup;
        [SerializeField] Graphic _condition;
        [SerializeField] RectTransform _conditionTransform;
        [SerializeField] RectTransform _background;

        [SerializeField] float _holdScrollPadding;
        [SerializeField] float _holdScrollRate;

        [SerializeField] Color _defaultColor;
        [SerializeField] Color _positiveColor;
        [SerializeField] Color _negativeColor;

        [SerializeField] private int _cellsCount = 6;

        private QuickAccessToolbarData _data = null;
        private IVariableInventoryCell[] _itemViews;
        protected CellCorner _cellCorner;

        int? _originalId;
        ICellData _originalCellData;
        Vector3 _conditionOffset;

        private Action<IVariableInventoryCell> _onCellClick;
        private Action<IVariableInventoryCell> _onCellOptionClick;
        private Action<IVariableInventoryCell> _onCellEnter;
        private Action<IVariableInventoryCell> _onCellExit;

        public IVariableInventoryViewData ViewData => _data;

        public void SetCellCallback(Action<IVariableInventoryCell> onCellClick, Action<IVariableInventoryCell> onCellOptionClick, Action<IVariableInventoryCell> onCellEnter, Action<IVariableInventoryCell> onCellExit)
        {
            Debug.Log("Qat: SetCellCallback");

            _onCellClick = onCellClick;
            _onCellOptionClick = onCellOptionClick;
            _onCellEnter = onCellEnter;
            _onCellExit = onCellExit;
        }

        public void Apply(IVariableInventoryViewData data)
        {
            Debug.Log("Qat: Apply");

            if (_data == null)
            {
                _data = new QuickAccessToolbarData(_cellsCount);
            }

            /*var count = gridLayoutGroup.transform.childCount;
            for (int i = 1; i < count; i++)
            {
                Destroy(gridLayoutGroup.transform.GetChild(1).gameObject);
            }*/

            if (_itemViews == null || _itemViews.Length != _cellsCount)
            {
                _itemViews = new IVariableInventoryCell[_cellsCount];

                for (var i = 0; i < _cellsCount; i++)
                {
                    var itemView = Instantiate(_cellPrefab, _gridLayoutGroup.transform);
                    _itemViews[i] = itemView;

                    itemView.transform.SetAsFirstSibling();
                    itemView.SetCellCallback(
                        _onCellClick,
                        _onCellOptionClick,
                        _onCellEnter,
                        _onCellExit,
                        null,
                        x => x.Apply(null));
                    itemView.SetHighlight(false);
                    //itemView.Apply(null);
                }

                _background.SetAsFirstSibling();

                _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                _gridLayoutGroup.constraintCount = _cellsCount;
                _gridLayoutGroup.cellSize = _itemViews.First().DefaultCellSize;
                _gridLayoutGroup.spacing = _itemViews.First().MargineSpace;
            }

            for (var i = 0; i < _data.CellData.Length; i++)
            {
                _itemViews[i].Apply(_data.CellData[i]);
            }
        }

        public void ReApply()
        {
            Debug.Log("Qat: ReApply");
        }

        public int? GetEffectCellID()
        {
            Debug.Log("Qat: GetEffectCellID");

            return 0;
        }

        public void OnCellEnter(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell)
        {
            Debug.Log("Qat: OnCellEnter");
        }

        public void OnCellExit(IVariableInventoryCell stareCell)
        {
            Debug.Log("Qat: OnCellExit");
        }

        public void OnDrag(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell, PointerEventData cursorPosition)
        {
            Debug.Log("Qat: OnDrag");
        }

        public bool? OnDrop(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell)
        {
            if (stareCell == null)
            {
                return null;
            }

            if (!_itemViews.Any(item => item == stareCell))
            {
                return false;
            }

            // check target;
            var index = GetIndex(stareCell);
            //var index = GetIndex(stareCell);
            if (!index.HasValue)
            {
                return false;
            }

            if (!_data.CheckInsert(index.Value, effectCell.CellData, false))
            {
                return false;
            }

            // place
            _data.InsertInventoryItem(index.Value, effectCell.CellData);
            _itemViews[index.Value].Apply(_data.CellData[index.Value]);

            _originalId = null;
            _originalCellData = null;
            return false;
        }

        public void OnDroped(bool? isDroped)
        {
            Debug.Log("Qat: OnDroped");
        }

        public ICellData OnPick(IVariableInventoryCell stareCell, PointerEventData.InputButton button)
        {
            Debug.Log("Qat: OnPick");

            return null;
        }

        public void OnPrePick(IVariableInventoryCell stareCell)
        {
            Debug.Log("Qat: OnPrePick");
        }

        public void OnSwitchRotate(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell)
        {
            Debug.Log("Qat: OnSwitchRotate");
        }

        protected virtual int? GetIndex(IVariableInventoryCell stareCell)
        {
            var index = (int?)null;
            for (var i = 0; i < _itemViews.Length; i++)
            {
                if (_itemViews[i] == stareCell)
                {
                    index = i;
                }
            }

            if (index < 0 || index >= _itemViews.Length)
            {
                return null;
            }

            return index;
        }

        protected virtual (int, int) GetRotateSize(ICellData cell)
        {
            if (cell == null)
            {
                return (1, 1);
            }

            return (cell.IsRotate ? cell.Height : cell.Width, cell.IsRotate ? cell.Width : cell.Height);
        }
    }
}
