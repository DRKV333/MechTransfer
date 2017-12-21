using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;

namespace MechTransfer.ContainerAdapters
{
    internal class ItemFrameAdapter
    {
        private TEItemFrame FindItemFrame(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active())
                return null;

            int originX = x - tile.frameX % 36 / 18;
            int originY = y - tile.frameY / 18;

            int id = TEItemFrame.Find(originX, originY);
            if (id == -1)
                return null;

            return (TEItemFrame)TileEntity.ByID[id];
        }

        private void HandleItemFrameChange(int x, int y, int id)
        {
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, id, (float)x, (float)y, 0f, 0, 0, 0);
        }

        public void InjectItem(int x, int y, Item item)
        {
            TEItemFrame frame = FindItemFrame(x, y);

            if (!frame.item.IsAir)
                return;

            frame.item = item.Clone();
            frame.item.stack = 1;
            item.stack--;
            HandleItemFrameChange(x, y, frame.ID);
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            TEItemFrame frame = FindItemFrame(x, y);

            yield return new Tuple<Item, object>(frame.item, null);
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
            TEItemFrame frame = FindItemFrame(x, y);

            frame.item = new Item();
            HandleItemFrameChange(x, y, frame.ID);
        }
    }
}