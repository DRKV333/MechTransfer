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

        public void InjectItem(int x, int y, Item item)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active())
                return;

            int expectedType = 0;
            int ammotype = 0;
            if (Main.tile[x, y].frameX < 72)
            {
                expectedType = ItemID.Cannonball;
                ammotype = 1;
            }
            else if (Main.tile[x, y].frameX < 144)
            {
                expectedType = ItemID.ExplosiveBunny;
                ammotype = 2;
            }
            else if (Main.tile[x, y].frameX < 288)
            {
                expectedType = ItemID.Confetti;
                ammotype = 3;
            }

            if (ammotype == 0 || item.type != expectedType)
                return;

            int originX = x - tile.frameX % 72 / 18;
            int originY = y - tile.frameY % 54 / 18;
            int angle = tile.frameY / 54;

            if (!Wiring.CheckMech(originX, originY, 30))
                return;

            WorldGen.ShootFromCannon(originX, originY, angle, ammotype, item.damage, item.knockBack, Main.myPlayer);

            item.stack--;
        }
    }
}