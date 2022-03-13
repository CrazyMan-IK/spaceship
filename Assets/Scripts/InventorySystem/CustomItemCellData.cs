using System;
using VariableInventorySystem;
using Astetrio.Spaceship.Interfaces;

namespace Astetrio.Spaceship.InventorySystem
{
    public class CustomItemCellData : ICellData
    {
        private readonly IItemInformation _item = null;

        public CustomItemCellData(IItemInformation item) : this(item, 1)
        {

        }

        public CustomItemCellData(IItemInformation item, int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            _item = item ?? throw new ArgumentNullException(nameof(item));

            ImageAsset = new SpriteAsset(item.Icon);

            Count = count;
        }

        public IItemInformation Information => _item;
        public int Id => GetHashCode();
        public int Width => _item.CellSize.x;
        public int Height => _item.CellSize.y;
        public int StackSize => _item.StackSize;
        public int Count { get; private set; } = 1;
        public bool IsRotate { get; set; }
        public bool IsFull => Count >= StackSize;
        public bool IsEmpty => Count <= 0;
        public IVariableInventoryAsset ImageAsset { get; }

        public void Merge(ICellData other, Quantity quantity)
        {
            var newCount = Count + other.Count * quantity.Value;

            if (!CanInsert(other, newCount))
            {
                throw new InvalidOperationException();
            }

            Count = newCount;
        }

        public bool CanInsert(ICellData other, Quantity quantity)
        {
            var newCount = Count + other.Count * quantity.Value;

            if (CanInsert(other, newCount))
            {
                return true;
            }

            return false;
        }

        public ICellData Clone(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return new CustomItemCellData(_item)
            {
                Count = count,
                IsRotate = IsRotate,
            };
        }

        private bool CanInsert(ICellData other, int newCount)
        {
            return other != null && Id == other.Id && newCount >= 0 && newCount <= StackSize;
        }

        public override bool Equals(object obj)
        {
            return obj is CustomItemCellData data &&
                   _item.InternalID == data._item.InternalID &&
                   //EqualityComparer<ItemInformation>.Default.Equals(_item, data._item) &&
                   //Id.Equals(data.Id) &&
                   Width == data.Width &&
                   Height == data.Height &&
                   StackSize == data.StackSize;// &&
                   //IsRotate == data.IsRotate &&
                   //EqualityComparer<IVariableInventoryAsset>.Default.Equals(ImageAsset, data.ImageAsset) &&
                   //Count == data.Count;
        }

        public override int GetHashCode()
        {
            int hashCode = 1693468656;
            hashCode = hashCode * -1521134295 + _item.InternalID.GetHashCode();
            //hashCode = hashCode * -1521134295 + EqualityComparer<ItemInformation>.Default.GetHashCode(_item);
            //hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            hashCode = hashCode * -1521134295 + StackSize.GetHashCode();
            //hashCode = hashCode * -1521134295 + IsRotate.GetHashCode();
            //hashCode = hashCode * -1521134295 + EqualityComparer<IVariableInventoryAsset>.Default.GetHashCode(ImageAsset);
            //hashCode = hashCode * -1521134295 + Count.GetHashCode();
            return hashCode;
        }
    }
}