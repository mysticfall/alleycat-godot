using System.Collections.Generic;

namespace AlleyCat.Item
{
    public interface IInventory
    {
        IEnumerable<IInventoryItem> Items { get; }

        void Add(IInventoryItem item);

        void Remove(IInventoryItem item);
    }
}
