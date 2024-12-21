using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace MechTransfer.ContainerAdapters
{
    internal class CannonAdapter
    {
        public void TakeItem(int x, int y, object slot, int amount)
        {
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            yield break;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile)
                return false;

            int expectedType = 0;
            int ammotype = 0;
            if (Main.tile[x, y].TileFrameX < 72)
            {
                expectedType = ItemID.Cannonball;
                ammotype = 1;
            }
            else if (Main.tile[x, y].TileFrameX < 144)
            {
                expectedType = ItemID.ExplosiveBunny;
                ammotype = 2;
            }
            else if (Main.tile[x, y].TileFrameX < 288)
            {
                expectedType = ItemID.Confetti;
                ammotype = 3;
            }

            if (ammotype == 0 || item.type != expectedType)
                return false;

            int originX = x - tile.TileFrameX % 72 / 18;
            int originY = y - tile.TileFrameY % 54 / 18;
            int angle = tile.TileFrameY / 54;

            if (!Wiring.CheckMech(originX, originY, 30))
                return false;

            WorldGen.ShootFromCannon(originX, originY, angle, ammotype, item.damage, item.knockBack, Main.myPlayer, false);

            item.stack--;

            return true;
        }
    }
}