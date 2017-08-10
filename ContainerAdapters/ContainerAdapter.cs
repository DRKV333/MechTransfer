using System;
using System.Collections.Generic;
using Terraria;
using EnumerateItemsDelegate = System.Func<int, int, System.Collections.Generic.IEnumerable<System.Tuple<Terraria.Item, object>>>;
using InjectItemDelegate = System.Func<int, int, Terraria.Item, bool>;
using TakeItemDelegate = System.Action<int, int, object, int>;

namespace MechTransfer.ContainerAdapters
{
    public class ContainerAdapterDefinition
    {
        public InjectItemDelegate InjectItem; //bool InjectItem(int x, int y, Item item);
        public EnumerateItemsDelegate EnumerateItems; //IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y);
        public TakeItemDelegate TakeItem; //void TakeItem(int x, int y, object SlotIdentifier, int amount);

        public ContainerAdapter GetAdapter(int x, int y)
        {
            return new ContainerAdapter(this, x, y);
        }
    }

    public class ContainerAdapter
    {
        private ContainerAdapterDefinition definition;

        private int X;
        private int Y;

        public ContainerAdapter(ContainerAdapterDefinition def, int x, int y)
        {
            X = x;
            Y = y;
            definition = def;
        }

        public bool InjectItem(Item item)
        {
            return definition.InjectItem.Invoke(X, Y, item);
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems()
        {
            return definition.EnumerateItems(X, Y);
        }

        public void TakeItem(object identifier, int amount)
        {
            definition.TakeItem.Invoke(X, Y, identifier, amount);
        }
    }
}