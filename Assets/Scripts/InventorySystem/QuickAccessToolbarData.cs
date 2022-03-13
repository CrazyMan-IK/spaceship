using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VariableInventorySystem;

namespace Astetrio.Spaceship.InventorySystem
{
    public class QuickAccessToolbarData : IVariableInventoryViewData
    {
        public QuickAccessToolbarData(int cellsCount)
        {
            IsDirty = true;
            CellData = new ICellData[cellsCount];
            //_prevCellData = CellData.Select(x => x?.Clone(x.Count)).ToArray();
            CellsCount = cellsCount;

            //_prevMask = new bool[CapacityWidth * CapacityHeight];
            //_mask = new bool[CapacityWidth * CapacityHeight];

            //UpdateMask();
        }

        public int CellsCount { get; }
        public ICellData[] CellData { get; }
        public bool IsDirty { get; set; }

        public void Apply()
        {
            Debug.Log("Qat: Apply");
        }

        public bool CheckInsert(int id, ICellData cellData, bool autoRotate = true)
        {
            if (id < 0 || id > CellsCount)
            {
                return false;
            }

            return true;
        }

        public ICellData GetCell(int id)
        {
            Debug.Log("Qat: GetCell");

            return null;
        }

        public int? GetId(ICellData cellData)
        {
            Debug.Log("Qat: GetId");

            return null;
        }

        public int? GetInsertableId(ICellData cellData)
        {
            Debug.Log("Qat: GetInsertableId");

            return null;
        }

        public void InsertInventoryItem(int id, ICellData cellData)
        {
            if (CellData[id] == cellData)
            {
                return;
            }

            //_prevCellData[id] = CellData[id].Clone(CellData[id].Count);

            CellData[id] = cellData;
            IsDirty = true;
        }
    }
}
