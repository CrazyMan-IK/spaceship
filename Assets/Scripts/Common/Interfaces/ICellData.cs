using Astetrio.Spaceship.Interfaces;
using System;

namespace VariableInventorySystem
{
    public interface ICellData
    {
        IItemInformation Information { get; }
        int Id { get; }
        int Width { get; }
        int Height { get; }
        int StackSize { get; }
        int Count { get; }
        bool IsRotate { get; set; }
        bool IsFull { get; }
        bool IsEmpty { get; }
        IVariableInventoryAsset ImageAsset { get; }

        void Merge(ICellData other, Quantity quantity);
        bool CanInsert(ICellData other, Quantity quantity);
        ICellData Clone(int count);
    }
}
