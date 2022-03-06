using System;

namespace VariableInventorySystem
{
    public interface IVariableInventoryCellData
    {
        int Id { get; }
        int Width { get; }
        int Height { get; }
        int StackSize { get; }
        int Count { get; }
        bool IsRotate { get; set; }
        bool IsFull { get; }
        bool IsEmpty { get; }
        IVariableInventoryAsset ImageAsset { get; }

        void Merge(IVariableInventoryCellData other, Quantity quantity);
        bool CanInsert(IVariableInventoryCellData other, Quantity quantity);
        IVariableInventoryCellData Clone(int count);
    }
}
