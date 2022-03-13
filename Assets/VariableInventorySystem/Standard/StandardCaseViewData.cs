using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    public class StandardCaseViewData : StandardStashViewData
    {
        public StandardCaseViewData(int capacityWidth, int capacityHeight) : base(capacityWidth, capacityHeight)
        {
        }

        public StandardCaseViewData(ICellData[] cellData, int capacityWidth, int capacityHeight) : base(cellData, capacityWidth, capacityHeight)
        {
        }

        public override int? GetInsertableId(ICellData cellData)
        {
            if (cellData is IStandardCaseCellData)
            {
                return null;
            }

            return base.GetInsertableId(cellData);
            /*var result = base.GetInsertableId(cellData);

            if (result.HasValue)
            {
                return result;
            }

            cellData.IsRotate = !cellData.IsRotate;
            result = base.GetInsertableId(cellData);
            //cellData.IsRotate = !cellData.IsRotate;

            if (result.HasValue)
            {
                return result;
            }

            cellData.IsRotate = !cellData.IsRotate;

            return null;*/
        }

        public override bool CheckInsert(int id, ICellData cellData, bool autoRotate = true)
        {
            if (cellData is IStandardCaseCellData)
            {
                return false;
            }

            return base.CheckInsert(id, cellData, autoRotate);
        }
    }
}
