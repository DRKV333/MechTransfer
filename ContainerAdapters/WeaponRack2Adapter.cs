using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;

namespace MechTransfer.ContainerAdapters
{
    internal class WeaponRack2Adapter
    {
        private TEWeaponsRack FindWeaponRack(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile)
                return null;

            int originX = x - tile.TileFrameX % 54 / 18;
            int originY = y - tile.TileFrameY % 54 / 18;

            int id = TEWeaponsRack.Find(originX, originY);
            if (id == -1)
                return null;

            return (TEWeaponsRack)TileEntity.ByID[id];
        }

        private void HandleItemFrameChange(int x, int y, int id)
        {
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, id, (float)x, (float)y, 0f, 0, 0, 0);
        }

        public bool InjectItem(int x, int y, Item item)
        {
            TEWeaponsRack rack = FindWeaponRack(x, y);

            if (!rack.item.IsAir)
                return false;

            rack.item = item.Clone();
            rack.item.stack = 1;
            item.stack--;
            HandleItemFrameChange(x, y, rack.ID);
            return true;
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            TEWeaponsRack rack = FindWeaponRack(x, y);

            yield return new Tuple<Item, object>(rack.item, null);
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
            TEWeaponsRack rack = FindWeaponRack(x, y);

            rack.item = new Item();
            HandleItemFrameChange(x, y, rack.ID);
        }
    }
}
