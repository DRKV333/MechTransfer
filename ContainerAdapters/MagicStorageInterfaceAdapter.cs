using MagicStorage;
using MagicStorage.Components;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace MechTransfer.ContainerAdapters
{
    internal class MagicStorageInterfaceAdapter
    {
        private TEStorageHeart FindHeart(int x, int y)
        {
            Point16 center = TEStorageComponent.FindStorageCenter(new Point16(x, y));
            if (center.X == -1 && center.Y == -1)
                return null;

            TEStorageHeart heart = ((TEStorageCenter)TileEntity.ByPosition[center]).GetHeart();

            return heart;
        }

        private void HandleStorageItemChange(TEStorageHeart heart)
        {
            if (Main.netMode == 2)
            {
                NetHelper.SendRefreshNetworkItems(heart.ID);
            }
            else if (Main.netMode == 0)
            {
                StorageGUI.RefreshItems();
            }
        }

        public void KickMe()
        {
            Main.LocalPlayer.GetModPlayer<StoragePlayer>().CloseStorage();
        }

        public bool InjectItem(int x, int y, Item item)
        {
            int oldstack = item.stack;

            TEStorageHeart targetHeart = FindHeart(x, y);
            targetHeart.DepositItem(item);

            if (oldstack != item.stack)
            {
                HandleStorageItemChange(targetHeart);
                return true;
            }
            return false;
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

            HandleStorageItemChange(targetHeart);
        }
    }
}