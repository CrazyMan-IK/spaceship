using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    public class StandardStashViewData : IVariableInventoryViewData
    {
        public bool IsDirty { get; set; }
        public ICellData[] CellData { get; }

        public int CapacityWidth { get; }
        public int CapacityHeight { get; }

        private readonly ICellData[] _prevCellData;
        private readonly bool[] _prevMask;
        private readonly bool[] _mask;

        public StandardStashViewData(int capacityWidth, int capacityHeight)
            : this(new ICellData[capacityWidth * capacityHeight], capacityWidth, capacityHeight)
        {
        }

        public StandardStashViewData(ICellData[] cellData, int capacityWidth, int capacityHeight)
        {
            Debug.Assert(cellData.Length == capacityWidth * capacityHeight);

            IsDirty = true;
            CellData = cellData;
            _prevCellData = CellData.Select(x => x?.Clone(x.Count)).ToArray();
            CapacityWidth = capacityWidth;
            CapacityHeight = capacityHeight;

            _prevMask = new bool[CapacityWidth * CapacityHeight];
            _mask = new bool[CapacityWidth * CapacityHeight];

            UpdateMask();
        }

        public virtual ICellData GetCell(int id)
        {
            return CellData[id];
        }

        public virtual int? GetId(ICellData cellData)
        {
            for (var i = 0; i < CellData.Length; i++)
            {
                if (CellData[i] == cellData)
                {
                    return i;
                }
            }

            return null;
        }

        public virtual int? GetInsertableId(ICellData cellData)
        {
            if (cellData == null)
            {
                return null;
            }

            for (int i = 0; i < _prevMask.Length; i++)
            {
                //if (CellData[i]?.Id == cellData.Id && !CellData[i].IsFull)
                //if (_prevCellData[i]?.CanInsert(cellData, new Quantity(1)) ?? false)
                if ((CellData[i]?.CanInsert(cellData, new Quantity(1)) ?? false) &&
                    (_prevCellData[i]?.CanInsert(cellData, new Quantity(1)) ?? true))
                {
                    return i;
                }
            }

            var mask = _mask.ToArray();
            for (int i = 0; i < _prevMask.Length; i++)
            {
                mask[i] |= _prevMask[i];
            }

            for (var i = 0; i < _prevMask.Length; i++)
            {
                if (!mask[i] && CheckInsert(i, cellData, mask, true))
                {
                    return i;
                }
            }

            return null;
        }

        public virtual void InsertInventoryItem(int id, ICellData cellData)
        {
            if (CellData[id] == cellData)
            {
                return;
            }

            //_prevCellData[id] = CellData[id].Clone(CellData[id].Count);

            if (cellData != null && !CheckInsert(id, cellData, false))
            {
                cellData.IsRotate = !cellData.IsRotate;
            }

            if (cellData != null && CellData[id]?.Id == cellData.Id)
            {
                //_prevCellData[id] = CellData[id].Clone(CellData[id].Count);
                CellData[id].Merge(cellData, new Quantity(1));
            }
            else
            {
                //_prevCellData[id] = _prevCellData[id]?.Clone(_prevCellData[id]?.Count ?? 1);
                CellData[id] = cellData;
            }
            IsDirty = true;

            UpdateMask();
        }

        public virtual void Apply()
        {
            for (int i = 0; i < _prevCellData.Length; i++)
            {
                _prevMask[i] = _mask[i];
                _prevCellData[i] = CellData[i]?.Clone(CellData[i].Count);
            }
        }

        public virtual bool CheckInsert(int id, ICellData cellData, bool autoRotate = true)
        {
            return CheckInsert(id, cellData, _mask, autoRotate);
        }

        protected void UpdateMask()
        {
            //mask = new bool[CapacityWidth * CapacityHeight];

            for (var i = 0; i < CellData.Length; i++)
            {
                _mask[i] = false;
            }

            for (var i = 0; i < CellData.Length; i++)
            {
                if (CellData[i] == null || _mask[i])
                {
                    continue;
                }

                /*var width = CellData[i].IsRotate ? CellData[i].Height : CellData[i].Width;
                var height = CellData[i].IsRotate ? CellData[i].Width : CellData[i].Height;*/
                var (width, height) = GetRotateSize(CellData[i]);
                
                for (var w = 0; w < width; w++)
                {
                    for (var h = 0; h < height; h++)
                    {
                        var checkIndex = i + w + (h * CapacityWidth);
                        if (checkIndex < _mask.Length)
                        {
                            _mask[checkIndex] = true;
                        }
                    }
                }
            }
        }

        protected (int, int) GetRotateSize(ICellData cell)
        {
            if (cell == null)
            {
                return (1, 1);
            }

            return (cell.IsRotate ? cell.Height : cell.Width, cell.IsRotate ? cell.Width : cell.Height);
        }

        private bool CheckInsert(int id, ICellData cellData, bool[] mask, bool autoRotate = true)
        {
            if (CellData[id]?.CanInsert(cellData, new Quantity(1)) ?? false)
            {
                return true;
            }

            var size = GetRotateSize(cellData);

            if (autoRotate)
            {
                if (CheckInsert(id, size, mask))
                {
                    return true;
                }

                cellData.IsRotate = !cellData.IsRotate;
                size = GetRotateSize(cellData);
                cellData.IsRotate = !cellData.IsRotate;

                if (CheckInsert(id, size, mask))
                {
                    return true;
                }

                //cellData.IsRotate = !cellData.IsRotate;

                return false;
            }
            else
            {
                return CheckInsert(id, size, mask);
            }
        }

        private bool CheckInsert(int id, (int, int) size, bool[] mask)
        {
            if (id < 0)
            {
                return false;
            }

            var (width, height) = size;

            // check width
            if ((id % CapacityWidth) + (width - 1) >= CapacityWidth)
            {
                return false;
            }

            // check height
            if (id + ((height - 1) * CapacityWidth) >= CellData.Length)
            {
                return false;
            }

            for (var i = 0; i < width; i++)
            {
                for (var t = 0; t < height; t++)
                {
                    if (mask[id + i + (t * CapacityWidth)])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
