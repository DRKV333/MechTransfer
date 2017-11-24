using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class OmniTurretTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 16, DropType(frameX));
        }

        public int DropType(int frameX)
        {
            int style = frameX / 36;
            switch (style)
            {
                case 0: return mod.ItemType("OmniTurretItem");
                case 1: return mod.ItemType("SuperOmniTurretItem");
                case 2: return mod.ItemType("MatterProjectorItem");
                default: return 0;
            }
        }

        public override void HitWire(int i, int j)
        {
            Rotate(i, j, true);
        }

        public override void RightClick(int i, int j)
        {
            if (Main.netMode == 1)
                RequestRotate(i, j);
            else
                Rotate(i, j, false);
        }

        public void Rotate(int i, int j, bool skipWies)
        {
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.active())
                return;

            int originX = i - (tile.frameX % 36) / 18;
            int originY = j - (tile.frameY % 36) / 18;

            Tile origin = Main.tile[originX, originY];
            if (origin == null || !origin.active())
                return;

            if (tile.frameX % 36 == 0)
            {
                if (origin.frameY > 0 && origin.frameY < 228)
                {
                    origin.frameY -= 38;
                    Main.tile[originX + 1, originY].frameY -= 38;
                    Main.tile[originX, originY + 1].frameY -= 38;
                    Main.tile[originX + 1, originY + 1].frameY -= 38;
                }
                else if (origin.frameY == 0)
                {
                    origin.frameY = 228;
                    Main.tile[originX + 1, originY].frameY = 228;
                    Main.tile[originX, originY + 1].frameY = 246;
                    Main.tile[originX + 1, originY + 1].frameY = 246;
                }
            }
            else
            {
                if (origin.frameY < 190)
                {
                    origin.frameY += 38;
                    Main.tile[originX + 1, originY].frameY += 38;
                    Main.tile[originX, originY + 1].frameY += 38;
                    Main.tile[originX + 1, originY + 1].frameY += 38;
                }
                else if (origin.frameY == 228)
                {
                    origin.frameY = 0;
                    Main.tile[originX + 1, originY].frameY = 0;
                    Main.tile[originX, originY + 1].frameY = 18;
                    Main.tile[originX + 1, originY + 1].frameY = 18;
                }
            }

            if (skipWies)
            {
                Wiring.SkipWire(originX, originY);
                Wiring.SkipWire(originX + 1, originY);
                Wiring.SkipWire(originX, originY + 1);
                Wiring.SkipWire(originX + 1, originY + 1);
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, originX, originY, 2, TileChangeType.None);
            }
        }

        public void RequestRotate(int i, int j)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)MechTransfer.ModMessageID.RotateTurret);
            packet.Write((Int16)i);
            packet.Write((Int16)j);
            packet.Send();
        }

        public override void MouseOver(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.active())
                return;

            Main.LocalPlayer.showItemIcon2 = DropType(tile.frameX);
            Main.LocalPlayer.showItemIcon = true;
        }
    }
}