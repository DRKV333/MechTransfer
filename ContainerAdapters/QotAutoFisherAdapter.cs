using ImproveGame.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    [JITWhenModsEnabled("ImproveGame")]
    internal class QotAutoFisherAdapter
    {
        public void TakeItem(int x, int y, object slot, int amount)
        {
            if(!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
            {
                return;
            }
            // maybe use reflection?
            Item[] fishes = teFisher.AsProxy().Fish;

            fishes[(int)slot].stack -= amount;
            if (fishes[(int)slot].stack < 1)
                fishes[(int)slot] = new Item();

        }
        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            if (!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
            {
                yield break;
            }
            // maybe use reflection?
            Item[] fishes = teFisher.AsProxy().Fish;

            for (int i = 0; i < fishes.Length; i++)
            {
                if (fishes[i].stack > 0)
                {
                    yield return new Tuple<Item, object>(fishes[i], (object)i);
                }
            }
        }

        public bool InjectItem(int x, int y, Item item)
        {
            if (!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
            {
                return false;
            }

            if (item.bait == 0)
            {
                return false;
            }

            FisherProxy proxy = teFisher.AsProxy();
            ref Item baitSlot = ref proxy.Bait;
            if (!proxy.HasBait)
            {
                baitSlot.SetDefaults(item.type);
                item.stack--;
                return true;
            }
            else if(baitSlot.IsTheSameAs(item))
            {
                if (baitSlot.stack  < baitSlot.maxStack)
                {
                    baitSlot.stack++;
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

            Point16 position = new Point16(x - tile.TileFrameX / 18 / 2, y - tile.TileFrameY / 18 / 2);
            if (!TileEntity.ByPosition.TryGetValue(position, out TileEntity te) || te is not TEAutofisher fisher)
            {
                teFisher = null;
                return false;
            }

            teFisher = fisher;
            return true;
        }
    }

    public readonly struct FisherProxy(TEAutofisher fisher)
    {
        public TEAutofisher Fisher { get; } = fisher;

        public ref Item FishingPole => ref FisherProxyUtils.GetFishingPole(Fisher);

        public ref Item Bait => ref FisherProxyUtils.GetBait(Fisher);

        public ref Item Accessory => ref FisherProxyUtils.GetAccessory(Fisher);

        public ref Item[] Fish => ref FisherProxyUtils.GetFish(Fisher);

        public bool HasBait => Fisher.HasBait;
    }

    public static class FisherProxyUtils
    {

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "fishingPole")]
        public static extern ref Item GetFishingPole(TEAutofisher fisher);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "bait")]
        public static extern ref Item GetBait(TEAutofisher fisher);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "accessory")]
        public static extern ref Item GetAccessory(TEAutofisher fisher);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "fish")]
        public static extern ref Item[] GetFish(TEAutofisher fisher);

        public static FisherProxy AsProxy(this TEAutofisher fisher)
        {
            return new FisherProxy(fisher);
        }
    }
}
