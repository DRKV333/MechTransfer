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

        public void HandleRackItemChange(int x, int y)
        {
            if (Main.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                NetMessage.SendTileSquare(-1, x + 1, y, 1, TileChangeType.None);
            }
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
            bool something;
            Point16 origin = FindOrigin(x, y, out something);

            Main.tile[origin.X, origin.Y].frameX = 0;
            Main.tile[origin.X + 1, origin.Y].frameX = 18;

            HandleRackItemChange(origin.X, origin.Y);
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            bool something;
            Point16 origin = FindOrigin(x, y, out something);

            if (Main.tile[origin.X, origin.Y].frameX < 5000)
                yield break; //Empty

            Item item = new Item();

            if (something)
            {
                item.netDefaults(Main.tile[origin.X, origin.Y].frameX - 20100);
                item.prefix = (byte)(Main.tile[origin.X + 1, origin.Y].frameX - 25000);
            }
            else
            {
                item.netDefaults(Main.tile[origin.X, origin.Y].frameX - 5100);
                item.prefix = (byte)(Main.tile[origin.X + 1, origin.Y].frameX - 10000);
            }

            yield return new Tuple<Item, object>(item, null);
        }

        public bool InjectItem(int x, int y, Item item)
        {
            if (!Main.LocalPlayer.ItemFitsWeaponRack(item))
                return false;

            bool something;
            Point16 origin = FindOrigin(x, y, out something);


            if (Main.tile[origin.X, origin.Y].frameX >= 5000)
                return false; //Already has item
           

            if(something)
            {
                Main.tile[origin.X, origin.Y].frameX = (short)(item.netID + 20100);
                Main.tile[origin.X + 1, origin.Y].frameX = (short)(item.prefix + 25000);
            }
            else
            {
                Main.tile[origin.X, origin.Y].frameX = (short)(item.netID + 5100);
                Main.tile[origin.X + 1, origin.Y].frameX = (short)(item.prefix + 10000);
            }

            HandleRackItemChange(origin.X, origin.Y);

            return true;
        }
    }
}
