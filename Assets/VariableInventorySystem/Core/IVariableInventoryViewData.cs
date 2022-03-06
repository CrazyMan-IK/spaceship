using UnityEngine;

namespace VariableInventorySystem
{
    public interface IVariableInventoryViewData
    {
        bool IsDirty { get; set; }

        int? GetId(IVariableInventoryCellData cellData);
        int? GetInsertableId(IVariableInventoryCellData cellData);
        void InsertInventoryItem(int id, IVariableInventoryCellData cellData);
        void Apply();
        bool CheckInsert(int id, IVariableInventoryCellData cellData, bool autoRotate = true);
    }
}