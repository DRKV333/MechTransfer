﻿using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class OmniTurretTile : SimpleTileObject, INetHandler
    {
        public override void SetDefaults()
        {
            AddMapEntry(MapColors.FillLight, GetPlaceItem(0).DisplayName);
            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
        }

        public override void HitWire(int i, int j)
        {
            Rotate(i, j, true);
        }

        public override bool NewRightClick(int i, int j)
        {
            if (Main.netMode == 1)
                RequestRotate(i, j);
            else
                Rotate(i, j, false);

			return true;
        }

        public void Rotate(int i, int j, bool skipWires)
        {
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.active())
                return;

            Point16 originLoc = GetOrigin(i, j);

            int LeftX = originLoc.X - 1;
            int TopY = originLoc.Y - 1;

            Tile origin = Main.tile[LeftX, TopY];
            if (origin == null || !origin.active())
                return;

            if (tile.frameX % 36 == 0)
            {
                if (origin.frameY > 0 && origin.frameY < 228)
                {
                    origin.frameY -= 38;
                    Main.tile[LeftX + 1, TopY].frameY -= 38;
                    Main.tile[LeftX, TopY + 1].frameY -= 38;
                    Main.tile[LeftX + 1, TopY + 1].frameY -= 38;
                }
                else if (origin.frameY == 0)
                {
                    origin.frameY = 228;
                    Main.tile[LeftX + 1, TopY].frameY = 228;
                    Main.tile[LeftX, TopY + 1].frameY = 246;
                    Main.tile[LeftX + 1, TopY + 1].frameY = 246;
                }
            }
            else
            {
                if (origin.frameY < 190)
                {
                    origin.frameY += 38;
                    Main.tile[LeftX + 1, TopY].frameY += 38;
                    Main.tile[LeftX, TopY + 1].frameY += 38;
                    Main.tile[LeftX + 1, TopY + 1].frameY += 38;
                }
                else if (origin.frameY == 228)
                {
                    origin.frameY = 0;
                    Main.tile[LeftX + 1, TopY].frameY = 0;
                    Main.tile[LeftX, TopY + 1].frameY = 18;
                    Main.tile[LeftX + 1, TopY + 1].frameY = 18;
                }
            }

            if (skipWires)
            {
                Wiring.SkipWire(LeftX, TopY);
                Wiring.SkipWire(LeftX + 1, TopY);
                Wiring.SkipWire(LeftX, TopY + 1);
                Wiring.SkipWire(LeftX + 1, TopY + 1);
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, LeftX, TopY, 2, TileChangeType.None);
            }
        }

        public void RequestRotate(int i, int j)
        {
            ModPacket packet = NetRouter.GetPacketTo(this, mod);
            packet.Write((Int16)i);
            packet.Write((Int16)j);
            packet.Send();
        }

        public override void MouseOver(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.active())
                return;

            Main.LocalPlayer.showItemIcon2 = PlaceItems[GetDropKind(tile.frameX, tile.frameY)].item.type;
            Main.LocalPlayer.showItemIcon = true;
        }

        public override int GetDropKind(int Fx, int Fy)
        {
            return Fx / tileObjectData.CoordinateFullWidth;
        }

        public override void PostLoad()
        {
            PlaceItems = new ModItem[3];

            int sell = Item.sellPrice(0, 1, 0, 0);

            //Omni turret
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(mod, "OmniTurretItem", Type, 32, 32, 0, sell);
            PlaceItems[0].item.rare = ItemRarityID.Green;

            //Super omni turret
            PlaceItems[1] = SimplePrototypeItem.MakePlaceable(mod, "SuperOmniTurretItem", Type, 32, 32, 1, sell);
            PlaceItems[1].item.rare = ItemRarityID.LightRed;

            //Matter projector
            PlaceItems[2] = SimplePrototypeItem.MakePlaceable(mod, "MatterProjectorItem", Type, 32, 32, 2, sell);
            PlaceItems[2].item.rare = ItemRarityID.Cyan;

            NetRouter.AddHandler(this);
        }

        public override void AddRecipes()
        {
            //Omni turret
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 5);
            r.AddIngredient(ItemID.IllegalGunParts, 1);
            r.AddIngredient(ItemID.DartTrap, 1);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(PlaceItems[0], 1);
            r.AddRecipe();

            //Super omni turret
            r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType("OmniTurretItem"), 1);
            r.AddIngredient(ItemID.Cog, 10);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(PlaceItems[1], 1);
            r.AddRecipe();

            //Matter projector
            r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType("SuperOmniTurretItem"), 1);
            r.AddIngredient(ItemID.FragmentVortex, 5);
            r.AddIngredient(ItemID.LunarBar, 5);
            r.AddTile(TileID.LunarCraftingStation);
            r.SetResult(PlaceItems[2], 1);
            r.AddRecipe();
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            Rotate(reader.ReadInt16(), reader.ReadInt16(), false);
        }
    }
}