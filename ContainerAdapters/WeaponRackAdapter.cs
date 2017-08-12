using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace MechTransfer.ContainerAdapters
{
    class WeaponRackAdapter
    {
        //By god, this weapon rack thing is a complete mess...

        public Point16 FindOrigin(int x, int y, out bool something)
        {
            Tile tile = Main.tile[x, y];

            int originY = y - tile.frameY % 36 / 18 + 1;

            int realFrameX = 0;
            int temp = Math.DivRem(tile.frameX, 5000, out realFrameX);
            if (temp != 0)
            {
                realFrameX = (temp - 1) * 18;
            }

            something = false;
            if (realFrameX >= 54)
            {
                realFrameX -= 54;
                something = true;
            }

            int originX = x - realFrameX / 18;

            return new Point16(originX, originY);
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
            bool something;
            Point16 origin = FindOrigin(x, y, out something);

            Tile tileItemData = Main.tile[origin.X, origin.Y];
            if (tileItemData == null || !tileItemData.active())
                return;

            Tile tilePrefixData = Main.tile[origin.X + 1, origin.Y];
            if (tilePrefixData == null || !tilePrefixData.active())
                return;

            if (tileItemData.frameX < 5000)
                return;


            tileItemData.frameX = 0;
            tilePrefixData.frameX = 18;

            if (Main.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, 1, TileChangeType.None);
                NetMessage.SendTileSquare(-1, origin.X + 1, origin.Y, 1, TileChangeType.None);
            }

        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            bool something;
            Point16 origin = FindOrigin(x, y, out something);

            Tile tileItemData = Main.tile[origin.X, origin.Y];
            if (tileItemData == null || !tileItemData.active())
                yield break;

            Tile tilePrefixData = Main.tile[origin.X + 1, origin.Y];
            if (tilePrefixData == null || !tilePrefixData.active())
                yield break;

            if (tileItemData.frameX < 5000)
                yield break;

            int magicNumber1 = 5000;
            int magicNumber2 = 10000;
            if (something)
            {
                magicNumber1 = 20000;
                magicNumber2 = 25000;
            }

            Item item = new Item();

            item.netDefaults(tileItemData.frameX - magicNumber1 - 100);
            item.prefix = (byte)(tilePrefixData.frameX - magicNumber2);

            yield return new Tuple<Item, object>(item, null);
        }

        public bool InjectItem(int x, int y, Item item)
        {
            if (!Main.LocalPlayer.ItemFitsWeaponRack(item))
                return false;

            bool something;
            Point16 origin = FindOrigin(x, y, out something);

            Tile tileItemData = Main.tile[origin.X, origin.Y];
            if (tileItemData == null || !tileItemData.active())
                return false;

            Tile tilePrefixData = Main.tile[origin.X + 1, origin.Y];
            if (tilePrefixData == null || !tilePrefixData.active())
                return false;

            if (tileItemData.frameX >= 5000)
                return false; //Already has item
           

            int magicNumber1 = 5000;
            int magicNumber2 = 10000;
            if (something)
            {
                magicNumber1 = 20000;
                magicNumber2 = 25000;
            }

            tileItemData.frameX = (short)(item.netID + magicNumber1 + 100);
            tilePrefixData.frameX = (short)(item.prefix + magicNumber2);

            if (Main.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, origin.X, origin.Y, 1, TileChangeType.None);
                NetMessage.SendTileSquare(-1, origin.X + 1, origin.Y, 1, TileChangeType.None);
            }

            return true;
        }
    }
}
