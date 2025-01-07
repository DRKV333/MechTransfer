using ImproveGame.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    internal class QolFisherAdapter
    {
        public void TakeItem(int x, int y, object slot, int amount)
        {
            if(!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
            {
                return;
            }
            // maybe use reflection?
            Item[] fishes = teFisher.fish;

            fishes[(int)slot].stack -= amount;
            if (fishes[(int)slot].stack < 1)
                fishes[(int)slot] = new Item();

        }
        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            yield break;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            if(!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
            {
                return false;
            }

            if (item.bait == 0)
            {
                return false;
            }

            if(!teFisher.HasBait)
            {
                Item clone = item.Clone();
                teFisher.bait.SetDefaults(item.type, 1);
                item.stack--;
                return true;
            }
            else if(teFisher.bait.IsTheSameAs(item))
            {
                if (teFisher.bait.stack  < item.maxStack)
                {
                    teFisher.bait.stack++;
                    item.stack--;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetTEAutofisher(int x, int y, out TEAutofisher teFisher) 
        {

            Tile tile = Main.tile[x, y];

            Autofisher autofisher = ModContent.GetInstance<Autofisher>();

            if ((tile == null) || (tile.TileType != autofisher.Type))
            {
                teFisher = null;
                return false;
            }

            Point16 position = new Point16(x, y);
            if (!TileEntity.ByPosition.TryGetValue(position, out TileEntity te) || te is not TEAutofisher fisher)
            {
                teFisher = null;
                return false;
            }

            teFisher = fisher;
            return true;
        }
    }
}
