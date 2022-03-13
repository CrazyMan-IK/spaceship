using UnityEngine;

namespace VariableInventorySystem
{
    public interface IVariableInventoryViewData
    {
        bool IsDirty { get; set; }

        ICellData GetCell(int id);
        int? GetId(ICellData cellData);
        int? GetInsertableId(ICellData cellData);
        void InsertInventoryItem(int id, ICellData cellData);
        void Apply();
        bool CheckInsert(int id, ICellData cellData, bool autoRotate = true);
    }
}