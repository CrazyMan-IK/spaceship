using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace VariableInventorySystem
{
    public class StandardStashView : MonoBehaviour, IVariableInventoryView
    {
        [SerializeField] StandardCell cellPrefab;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] GridLayoutGroup gridLayoutGroup;
        [SerializeField] Graphic condition;
        [SerializeField] RectTransform conditionTransform;
        [SerializeField] RectTransform background;
        [SerializeField] private TextMeshProUGUI _debugText = null;

        [SerializeField] float holdScrollPadding;
        [SerializeField] float holdScrollRate;

        [SerializeField] Color defaultColor;
        [SerializeField] Color positiveColor;
        [SerializeField] Color negativeColor;

        protected IVariableInventoryCell[] itemViews;
        protected CellCorner cellCorner;

        int? originalId;
        ICellData originalCellData;
        Vector3 conditionOffset;

        Action<IVariableInventoryCell> onCellClick;
        Action<IVariableInventoryCell> onCellOptionClick;
        Action<IVariableInventoryCell> onCellEnter;
        Action<IVariableInventoryCell> onCellExit;

        public IVariableInventoryViewData ViewData => StashData;
        public StandardStashViewData StashData { get; private set; }
        public int CellCount => StashData.CapacityWidth * StashData.CapacityHeight;

        public void SetCellCallback(
            Action<IVariableInventoryCell> onCellClick,
            Action<IVariableInventoryCell> onCellOptionClick,
            Action<IVariableInventoryCell> onCellEnter,
            Action<IVariableInventoryCell> onCellExit)
        {
            this.onCellClick = onCellClick;
            this.onCellOptionClick = onCellOptionClick;
            this.onCellEnter = onCellEnter;
            this.onCellExit = onCellExit;
        }

        public int? GetEffectCellID()
        {
            return originalId;
        }

        public int? GetCellID(IVariableInventoryCell cell)
        {
            for (int i = 0; i < itemViews.Length; i++)
            {
                if (itemViews[i] == cell)
                {
                    return i;
                }
            }

            return null;
        }

        public virtual void Apply(IVariableInventoryViewData data)
        {
            StashData = ((StandardStashViewData)data);

            /*var count = gridLayoutGroup.transform.childCount;
            for (int i = 1; i < count; i++)
            {
                Destroy(gridLayoutGroup.transform.GetChild(1).gameObject);
            }*/

            if (itemViews == null || itemViews.Length != CellCount)
            {
                itemViews = new IVariableInventoryCell[CellCount];

                for (var i = 0; i < CellCount; i++)
                {
                    var itemView = Instantiate(cellPrefab, gridLayoutGroup.transform);
                    itemViews[i] = itemView;

                    itemView.transform.SetAsFirstSibling();
                    itemView.SetCellCallback(
                        onCellClick,
                        onCellOptionClick,
                        onCellEnter,
                        onCellExit,
                        _ => scrollRect.enabled = false,
                        _ => scrollRect.enabled = true);
                    itemView.Apply(null);
                }

                background.SetAsFirstSibling();

                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = StashData.CapacityWidth;
                gridLayoutGroup.cellSize = itemViews.First().DefaultCellSize;
                gridLayoutGroup.spacing = itemViews.First().MargineSpace;
            }

            for (var i = 0; i < StashData.CellData.Length; i++)
            {
                itemViews[i].Apply(StashData.CellData[i]);
            }
        }

        public virtual void ReApply()
        {
            if (!StashData.IsDirty)
            {
                return;
            }

            Apply(StashData);
            StashData.IsDirty = false;
        }

        public virtual void OnPrePick(IVariableInventoryCell stareCell)
        {
            if (stareCell?.CellData == null)
            {
                return;
            }

            StashData.Apply();

            var (width, height) = GetRotateSize(stareCell.CellData);
            conditionTransform.sizeDelta = new Vector2(stareCell.DefaultCellSize.x * width, stareCell.DefaultCellSize.y * height);
        }

        public virtual ICellData OnPick(IVariableInventoryCell stareCell, PointerEventData.InputButton button)
        {
            if (stareCell?.CellData == null)
            {
                return null;
            }

            var id = StashData.GetId(stareCell.CellData);
            if (id.HasValue)
            {
                originalId = id;
                originalCellData = stareCell.CellData;

                var itemView = itemViews[id.Value];
                if (button == PointerEventData.InputButton.Left)
                {
                    itemView.Apply(null);
                    StashData.InsertInventoryItem(id.Value, null);
                }
                else if (button == PointerEventData.InputButton.Right)
                {
                    var newCell = itemView.CellData.Clone(Mathf.CeilToInt(itemView.CellData.Count / 2.0f));
                    itemView.CellData.Merge(newCell, new Quantity(-1));

                    originalCellData = newCell;

                    if (itemView.CellData.IsEmpty)
                    {
                        itemView.Apply(null);
                        StashData.InsertInventoryItem(id.Value, null);
                    }
                    else
                    {
                        itemView.Apply(itemView.CellData);
                        StashData.InsertInventoryItem(id.Value, itemView.CellData);
                    }
                }
                else
                {
                    return null;
                }
                return originalCellData;
            }

            return null;
        }

        public virtual void OnDrag(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell, PointerEventData pointerEventData)
        {
            if (stareCell == null)
            {
                return;
            }

            // auto scroll
            var pointerViewportPosition = GetLocalPosition(scrollRect.viewport, pointerEventData.position, pointerEventData.enterEventCamera);

            if (pointerViewportPosition.y < scrollRect.viewport.rect.min.y + holdScrollPadding)
            {
                var scrollValue = scrollRect.verticalNormalizedPosition * scrollRect.viewport.rect.height;
                scrollValue -= holdScrollRate;
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollValue / scrollRect.viewport.rect.height);
            }

            if (pointerViewportPosition.y > scrollRect.viewport.rect.max.y - holdScrollPadding)
            {
                var scrollValue = scrollRect.verticalNormalizedPosition * scrollRect.viewport.rect.height;
                scrollValue += holdScrollRate;
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollValue / scrollRect.viewport.rect.height);
            }

            var (width, height) = GetRotateSize(effectCell.CellData);
            var size = new Vector2(width, height);
            //var size2 = new Vector2(stareWidth, stareHeight);

            // depends on anchor
            var cellSize = stareCell.DefaultCellSize + stareCell.MargineSpace;

            var cellPointerLocalPosition = GetLocalPosition(stareCell.RectTransform, pointerEventData.position, pointerEventData.enterEventCamera);
            var anchor = cellSize * 0.5f;
            var anchoredPosition = cellPointerLocalPosition + anchor;
            conditionOffset = new Vector3(
                Mathf.Floor(anchoredPosition.x / cellSize.x) * cellSize.x,
                Mathf.Ceil(anchoredPosition.y / cellSize.y) * cellSize.y);

            // cell corner
            var prevCorner = cellCorner;
            cellCorner = GetCorner(new Vector2(anchoredPosition.x % cellSize.x - anchor.x, anchoredPosition.y % cellSize.y + anchor.y));

            var conditionSize = size * (effectCell.DefaultCellSize + effectCell.MargineSpace) - effectCell.MargineSpace;

            // shift the position only even number size
            conditionTransform.sizeDelta = conditionSize;
            var evenNumberOffset = GetEvenNumberOffset(width, height, cellSize.x * 0.5f, cellSize.y * 0.5f);
            conditionTransform.position = stareCell.RectTransform.position + ((conditionOffset + evenNumberOffset) * stareCell.RectTransform.lossyScale.x);

            // update condition
            if (prevCorner != cellCorner)
            {
                UpdateCondition(stareCell, effectCell);
            }
        }

        public virtual bool? OnDrop(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell)
        {
            if (stareCell == null)
            {
                return null;
            }

            if (!itemViews.Any(item => item == stareCell))
            {
                return false;
            }

            // check target;
            var index = GetIndex(stareCell, effectCell.CellData, cellCorner);
            //var index = GetIndex(stareCell);
            if (!index.HasValue)
            {
                return false;
            }

            if (!StashData.CheckInsert(index.Value, effectCell.CellData, false))
            {
                if (stareCell.CellData != null)
                {
                    if (stareCell.CellData is ICellData cellData && cellData.CanInsert(effectCell.CellData, new Quantity(1)))
                    {
                        StashData.InsertInventoryItem(index.Value, effectCell.CellData);
                        itemViews[index.Value].Apply(StashData.CellData[index.Value]);
                        //stareCell.CellData.Add();

                        originalId = null;
                        originalCellData = null;
                        return true;
                    }

                    // check free space in case
                    if (stareCell.CellData is IStandardCaseCellData caseData)
                    {
                        var id = caseData.CaseData.GetInsertableId(effectCell.CellData);
                        if (id.HasValue)
                        {
                            caseData.CaseData.InsertInventoryItem(id.Value, effectCell.CellData);

                            originalId = null;
                            originalCellData = null;
                            return true;
                        }
                    }
                }

                return false;
            }

            // place
            StashData.InsertInventoryItem(index.Value, effectCell.CellData);
            itemViews[index.Value].Apply(StashData.CellData[index.Value]);

            originalId = null;
            originalCellData = null;
            return true;
        }

        public virtual void OnDroped(bool? isDroped)
        {
            conditionTransform.gameObject.SetActive(false);
            condition.color = defaultColor;

            if (isDroped.HasValue && !isDroped.Value && originalId.HasValue)
            {
                // revert
                var itemView = itemViews[originalId.Value];
                if (itemView.CellData != null)
                {
                    itemView.CellData.Merge(originalCellData, new Quantity(1));
                    itemView.Apply(itemView.CellData);
                    //StashData.InsertInventoryItem(originalId.Value, itemView.CellData);
                }
                else
                {
                    itemView.Apply(originalCellData);
                    StashData.InsertInventoryItem(originalId.Value, originalCellData);
                }
            }

            StashData.Apply();

            originalId = null;
            originalCellData = null;
        }

        public virtual void OnCellEnter(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell)
        {
            conditionTransform.gameObject.SetActive(effectCell?.CellData != null);
            stareCell.SetHighlight(true);
        }

        public virtual void OnCellExit(IVariableInventoryCell stareCell)
        {
            conditionTransform.gameObject.SetActive(false);
            condition.color = defaultColor;

            cellCorner = CellCorner.None;

            stareCell.SetHighlight(false);
        }

        public virtual void OnSwitchRotate(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell)
        {
            if (stareCell == null)
            {
                return;
            }

            var (width, height) = GetRotateSize(effectCell.CellData);
            conditionTransform.sizeDelta = new Vector2(effectCell.DefaultCellSize.x * width, effectCell.DefaultCellSize.y * height);

            var evenNumberOffset = GetEvenNumberOffset(width, height, stareCell.DefaultCellSize.x * 0.5f, stareCell.DefaultCellSize.y * 0.5f);
            conditionTransform.position = stareCell.RectTransform.position + ((conditionOffset + evenNumberOffset) * stareCell.RectTransform.lossyScale.x);

            UpdateCondition(stareCell, effectCell);
        }

        protected virtual int? GetIndex(IVariableInventoryCell stareCell)
        {
            var index = (int?)null;
            for (var i = 0; i < itemViews.Length; i++)
            {
                if (itemViews[i] == stareCell)
                {
                    index = i;
                }
            }

            if (index < 0 || index >= itemViews.Length)
            {
                return null;
            }

            return index;
        }

        protected virtual int? GetIndex(IVariableInventoryCell stareCell, ICellData effectCellData, CellCorner cellCorner)
        {
            var index = GetIndex(stareCell);

            if (stareCell.CellData != null && stareCell.CellData.Id == effectCellData.Id)
            {
                return index;
            }

            // offset index
            var (width, height) = GetRotateSize(effectCellData);
            if (width % 2 == 0)
            {
                if ((cellCorner & CellCorner.Left) != CellCorner.None)
                {
                    index--;
                }
            }

            if (height % 2 == 0)
            {
                if ((cellCorner & CellCorner.Top) != CellCorner.None)
                {
                    index -= StashData.CapacityWidth;
                }
            }

            index -= (width - 1) / 2;
            index -= (height - 1) / 2 * StashData.CapacityWidth;

            if (index < 0 || index >= itemViews.Length)
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

        protected virtual Vector2 GetLocalPosition(RectTransform parent, Vector2 position, Camera camera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, position, camera, out var localPosition);
            return localPosition;
        }

        protected virtual CellCorner GetCorner(Vector2 localPosition)
        {
            // depends on pivot
            var corner = CellCorner.None;
            if (localPosition.x < Mathf.Epsilon)
            {
                corner |= CellCorner.Left;
            }

            if (localPosition.x > Mathf.Epsilon)
            {
                corner |= CellCorner.Right;
            }

            if (localPosition.y > Mathf.Epsilon)
            {
                corner |= CellCorner.Top;
            }

            if (localPosition.y < Mathf.Epsilon)
            {
                corner |= CellCorner.Bottom;
            }

            return corner;
        }

        protected virtual Vector3 GetEvenNumberOffset(int width, int height, float widthOffset, float heightOffset)
        {
            var evenNumberOffset = new Vector3(0, -heightOffset * 2, 0);

            if (width % 2 == 0)
            {
                if ((cellCorner & CellCorner.Left) != CellCorner.None)
                {
                    evenNumberOffset.x -= widthOffset;
                }

                if ((cellCorner & CellCorner.Right) != CellCorner.None)
                {
                    evenNumberOffset.x += widthOffset;
                }
            }

            if (height % 2 == 0)
            {
                if ((cellCorner & CellCorner.Top) != CellCorner.None)
                {
                    evenNumberOffset.y += heightOffset;
                }

                if ((cellCorner & CellCorner.Bottom) != CellCorner.None)
                {
                    evenNumberOffset.y -= heightOffset;
                }
            }

            return evenNumberOffset;
        }

        protected virtual void UpdateCondition(IVariableInventoryCell stareCell, IVariableInventoryCell effectCell)
        {
            /*int? index = null;
            if (stareCell.CellData != null && stareCell.CellData.Id == effectCell.CellData.Id)
            {
                index = GetIndex(stareCell);
            }
            else
            {
                index = GetIndex(stareCell, effectCell.CellData, cellCorner);
            }*/
            var index = GetIndex(stareCell, effectCell.CellData, cellCorner);
            //var index = GetIndex(stareCell);
            if (index.HasValue && StashData.CheckInsert(index.Value, effectCell.CellData, false))
            {
                condition.color = positiveColor;
            }
            else
            {
                // check free space in case
                if (stareCell.CellData != null &&
                    stareCell.CellData is IStandardCaseCellData caseData &&
                    caseData.CaseData.GetInsertableId(effectCell.CellData).HasValue)
                {
                    condition.color = positiveColor;
                }
                else
                {
                    condition.color = negativeColor;
                }
            }
        }
    }
}
