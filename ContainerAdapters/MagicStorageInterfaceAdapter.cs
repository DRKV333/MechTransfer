using MagicStorage;
using MagicStorage.Components;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    [JITWhenModsEnabled("MagicStorage")]
    internal class MagicStorageInterfaceAdapter
    {
        private TEStorageHeart FindHeart(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            int originX = x - tile.TileFrameX / 18;
            int originY = y - tile.TileFrameY / 18;

            Point16 center = TEStorageComponent.FindStorageCenter(new Point16(originX, originY));
            if (center.X == -1 && center.Y == -1)
                return null;

            TEStorageHeart heart = ((TEStorageCenter)TileEntity.ByPosition[center]).GetHeart();

            return heart;
        }

        private void HandleStorageItemChange(TEStorageHeart heart)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetHelper.SendRefreshNetworkItems(heart.Position);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                StorageGUI.RefreshItems();
            }
        }

        public bool InjectItem(int x, int y, Item item)
        {
            int oldstack = item.stack;

            TEStorageHeart targetHeart = FindHeart(x, y);
            if (targetHeart == null)
                return false;

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
            if (targetHeart == null)
                yield break;

            foreach (var item in targetHeart.GetStoredItems())
            {
                yield return new Tuple<Item, object>(item, item);
            }
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
            TEStorageHeart targetHeart = FindHeart(x, y);
            if (targetHeart == null)
                return;

            Item toWithdraw = ((Item)slot).Clone();
            toWithdraw.stack = amount;

            targetHeart.TryWithdraw(toWithdraw, true); //TODO Nebula: decide if keepOneIfFavorite should be hardcoded or a setting

            HandleStorageItemChange(targetHeart);
        }
    }
}