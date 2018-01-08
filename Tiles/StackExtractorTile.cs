using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class StackExtractorTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);

            drop = mod.ItemType("StackExtractorItem");
            AddMapEntry(new Color(200, 200, 200));
        }

        public override void HitWire(int i, int j)
        {
            if (Main.netMode == 1)
                return;

            foreach (var c in ((MechTransfer)mod).transferAgent.FindContainerAdjacent(i, j))
            {
                foreach (var item in c.EnumerateItems())
                {
                    if (!item.Item1.IsAir)
                    {
                        Item clone = item.Item1.Clone();
                        int numTook = ((MechTransfer)mod).transferAgent.StartTransfer(i, j, clone);
                        if (numTook > 0)
                        {
                            c.TakeItem(item.Item2, numTook);
                            return;
                        }
                    }
                }
            }
        }
    }
}