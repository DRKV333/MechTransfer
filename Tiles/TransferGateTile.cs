using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferGateTile : ModTile
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

            drop = mod.ItemType("TransferGateItem");
            AddMapEntry(new Color(200, 200, 200));
        }

        public override void HitWire(int i, int j)
        {
            if (Main.tile[i, j].frameY == 0)
            {
                Main.tile[i, j].frameY = 18;
            }
            else
            {
                Main.tile[i, j].frameY = 0;
            }
        }
    }
}