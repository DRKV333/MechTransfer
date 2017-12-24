using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using MagicStorage;
using MagicStorage.Components;
using Terraria.DataStructures;
using Terraria.ModLoader;
using MechTransfer.Tiles;

namespace MechTransfer.ContainerAdapters
{
    class MagicStorageInterfaceAdapter
    {
        private TEStorageHeart FindHeart(int x, int y)
        {
            Point16 center = TEStorageComponent.FindStorageCenter(new Point16(x, y));
            if (center.X == -1 && center.Y == -1)
                return null;

            TEStorageHeart heart = ((TEStorageCenter)TileEntity.ByPosition[center]).GetHeart();

            if (Main.netMode == 0)
            {
                StoragePlayer storagePlayer = Main.LocalPlayer.GetModPlayer<StoragePlayer>();
                if (storagePlayer.GetStorageHeart() == heart)
                    storagePlayer.CloseStorage();
            }

            return heart;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            int oldstack = item.stack;

            TEStorageHeart targetHeart = FindHeart(x, y);
            targetHeart.DepositItem(item);

            return oldstack != item.stack;
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            TEStorageHeart targetHeart = FindHeart(x, y);
            foreach (var item in targetHeart.GetStoredItems())
            {
                yield return new Tuple<Item, object>(item, item.type);
            }
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
            TEStorageHeart targetHeart = FindHeart(x, y);

            Item toWithdraw = new Item();
            toWithdraw.SetDefaults((int)slot);
            toWithdraw.stack = amount;

            targetHeart.TryWithdraw(toWithdraw);
        }
    }
}
