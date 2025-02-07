using ImproveGame.Content.Tiles;
using ImproveGame.Packets.NetAutofisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    [JITWhenModsEnabled("ImproveGame")]
    internal class QotAutoFisherAdapter
    {
        public const int BassSpeedingCap = 100;

        //TODO: should we send a packet to the client to update the UI?
        //Qot use NetSimplified lib to send packets
        public void TakeItem(int x, int y, object slot, int amount)
        {
            if (!TryGetTEAutofisher(x, y, out TEAutofisher teFisher))
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
            int bassExists = 0;

            for (int i = 0; i < fishes.Length; i++)
            {
                Item fish = fishes[i];
                if (fish.stack > 0)
                {
                    if(fish.type == ItemID.Bass && bassExists < BassSpeedingCap)
                    {
                        bassExists += fish.stack;
                        continue;
                    }
                    yield return new Tuple<Item, object>(fish, i);
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
            else if (baitSlot.IsTheSameAs(item))
            {
                if (baitSlot.stack < baitSlot.maxStack)
                {
                    baitSlot.stack++;
                    item.stack--;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetTEAutofisher(int x, int y, out TEAutofisher teFisher)
        {

            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile)
            {
                teFisher = null;
                return false;
            }

            int originX = x - (tile.TileFrameX % 36) / 18;
            int originY = y - (tile.TileFrameY % 36) / 18;

            Tile origin = Main.tile[originX, originY];
            if (origin == null || !origin.HasTile)
            {
                teFisher = null;
                return false;
            }

            Point16 position = new Point16(originX, originY);
            if (!TileEntity.ByPosition.TryGetValue(position, out TileEntity te) || te is not TEAutofisher fisher)
            {
                teFisher = null;
                return false;
            }

            teFisher = fisher;
            return true;
        }
        
        internal struct FisherProxy(TEAutofisher fisher)
        {
            public TEAutofisher Fisher { get; } = fisher;

            public ref Item FishingPole => ref FisherProxyUtils.GetFishingPole(Fisher);

            public ref Item Bait => ref FisherProxyUtils.GetBait(Fisher);

            public ref Item Accessory => ref FisherProxyUtils.GetAccessory(Fisher);

            public ref Item[] Fish => ref FisherProxyUtils.GetFish(Fisher);

            public bool HasBait => Fisher.HasBait;
        }

    }

    [JITWhenModsEnabled("ImproveGame")]
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

        internal static QotAutoFisherAdapter.FisherProxy AsProxy(this TEAutofisher fisher)
        {
            return new QotAutoFisherAdapter.FisherProxy(fisher);
        }
    }
}
