using MechTransfer.ContainerAdapters;
using System;
using System.Collections.Generic;
using Terraria;

namespace MechTransfer
{
    internal class ItemCatalog
    {
        private List<ContainerAdapter> container = new List<ContainerAdapter>();
        private List<object> identifier = new List<object>();
        private List<int> amount = new List<int>();
        private List<int> amountTook = new List<int>();

        public void RegisterItem(ContainerAdapter c, Tuple<Item, object> i)
        {
            container.Add(c);
            identifier.Add(i.Item2);
            amount.Add(i.Item1.stack);
            amountTook.Add(0);
        }

        private int TotalSingle(int i)
        {
            return amount[i] - amountTook[i];
        }

        public int Total()
        {
            int total = 0;
            for (int i = 0; i < amount.Count; i++)
            {
                total += TotalSingle(i);
            }
            return total;
        }

        public void Take(int many)
        {
            int left = many;
            for (int i = 0; i < amount.Count; i++)
            {
                int single = TotalSingle(i);
                if (single >= left)
                {
                    amountTook[i] += left;
                    return;
                }
                else
                {
                    amountTook[i] += single;
                    left -= single;
                }
            }
        }

        public void Commit()
        {
            for (int i = 0; i < amount.Count; i++)
            {
                if (amountTook[i] > 0)
                    container[i].TakeItem(identifier[i], amountTook[i]);
            }
        }

        public void CommitAlchemy()
        {
            for (int i = 0; i < amount.Count; i++)
            {
                for (int j = 0; j < amountTook[i]; j++)
                {
                    if (Main.rand.Next(3) != 0)
                        container[i].TakeItem(identifier[i], 1);
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < amount.Count; i++)
            {
                amountTook[i] = 0;
            }
        }
    }

    internal class ItemInventory
    {
        private Dictionary<int, ItemCatalog> catalogs = new Dictionary<int, ItemCatalog>();

        public void Commit(bool alchemy)
        {
            if (alchemy)
            {
                foreach (var item in catalogs)
                {
                    item.Value.CommitAlchemy();
                }
            }
            else
            {
                foreach (var item in catalogs)
                {
                    item.Value.Commit();
                }
            }
        }

        public void Reset()
        {
            foreach (var item in catalogs)
            {
                item.Value.Reset();
            }
        }

        public void Clear()
        {
            catalogs.Clear();
        }

        public void RegisterContainer(ContainerAdapter container)
        {
            foreach (var item in container.EnumerateItems())
            {
                if (!item.Item1.IsAir)
                {
                    if (!catalogs.ContainsKey(item.Item1.type))
                        catalogs.Add(item.Item1.type, new ItemCatalog());
                    catalogs[item.Item1.type].RegisterItem(container, item);
                }
            }
        }

        public int ItemCount(int type)
        {
            ItemCatalog catalog;
            if (catalogs.TryGetValue(type, out catalog))
            {
                return catalog.Total();
            }
            else
            {
                return 0;
            }
        }

        public void TakeItem(Item take)
        {
            catalogs[take.type].Take(take.stack);
        }

        public bool TryTakeItem(Item take)
        {
            ItemCatalog catalog;
            if(catalogs.TryGetValue(take.type, out catalog))
            {
                if (catalog.Total() >= take.stack)
                {
                    catalog.Take(take.stack);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool TryTakeIngredient(Recipe recipe, Item ingredient)
        {
            int need = ingredient.stack;
            int type = ingredient.type;

            foreach (var item in catalogs)
            {
                if (recipe.useWood(item.Key, type) || recipe.useSand(item.Key, type) || recipe.useIronBar(item.Key, type)
                    || recipe.useFragment(item.Key, type) || recipe.AcceptedByItemGroups(item.Key, type) || recipe.usePressurePlate(item.Key, type)
                    || item.Key == type)
                {
                    int total = item.Value.Total();
                    if (total >= need)
                    {
                        item.Value.Take(need);
                        return true;
                    }
                    else
                    {
                        item.Value.Take(total);
                        need -= total;
                    }
                }
            }

            return false;
        }
    }
}