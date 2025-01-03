using MechTransfer.Items;
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
    [Autoload(false)]
    public class OmniTurretTile : SimpleTileObject, INetHandler
    {
        public override void PostSetDefaults()
        {
            AddMapEntry(MapColors.FillLight, GetPlaceItem(0).DisplayName);
            base.PostSetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(1,1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
        }

        public override void HitWire(int i, int j)
        {
            Rotate(i, j, true);
        }

        public override bool RightClick(int i, int j)
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
            if (tile == null || !tile.HasTile)
                return;

            Point16 originLoc = GetOrigin(i, j);

            int LeftX = originLoc.X - 1;
            int TopY = originLoc.Y - 1;

            Tile origin = Main.tile[LeftX, TopY];
            if (origin == null || !origin.HasTile)
                return;

            if (tile.TileFrameX % 36 == 0)
            {
                if (origin.TileFrameY > 0 && origin.TileFrameY < 228)
                {
                    origin.TileFrameY -= 38;
                    Main.tile[LeftX + 1, TopY].TileFrameY -= 38;
                    Main.tile[LeftX, TopY + 1].TileFrameY -= 38;
                    Main.tile[LeftX + 1, TopY + 1].TileFrameY -= 38;
                }
                else if (origin.TileFrameY == 0)
                {
                    origin.TileFrameY = 228;
                    Main.tile[LeftX + 1, TopY].TileFrameY = 228;
                    Main.tile[LeftX, TopY + 1].TileFrameY = 246;
                    Main.tile[LeftX + 1, TopY + 1].TileFrameY = 246;
                }
            }
            else
            {
                if (origin.TileFrameY < 190)
                {
                    origin.TileFrameY += 38;
                    Main.tile[LeftX + 1, TopY].TileFrameY += 38;
                    Main.tile[LeftX, TopY + 1].TileFrameY += 38;
                    Main.tile[LeftX + 1, TopY + 1].TileFrameY += 38;
                }
                else if (origin.TileFrameY == 228)
                {
                    origin.TileFrameY = 0;
                    Main.tile[LeftX + 1, TopY].TileFrameY = 0;
                    Main.tile[LeftX, TopY + 1].TileFrameY = 18;
                    Main.tile[LeftX + 1, TopY + 1].TileFrameY = 18;
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
            ModPacket packet = NetRouter.GetPacketTo(this, Mod);
            packet.Write((Int16)i);
            packet.Write((Int16)j);
            packet.Send();
        }

        public override void MouseOver(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.HasTile)
                return;

            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = PlaceItems[GetDropKind(tile.TileFrameX, tile.TileFrameY)].Type;
            player.noThrow = 2;
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
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "OmniTurretItem", Type, 32, 32, 0, sell);
            PlaceItems[0].Item.rare = ItemRarityID.Green;

            //Super omni turret
            PlaceItems[1] = SimplePrototypeItem.MakePlaceable(Mod, "SuperOmniTurretItem", Type, 32, 32, 1, sell);
            PlaceItems[1].Item.rare = ItemRarityID.LightRed;

            //Matter projector
            PlaceItems[2] = SimplePrototypeItem.MakePlaceable(Mod, "MatterProjectorItem", Type, 32, 32, 2, sell);
            PlaceItems[2].Item.rare = ItemRarityID.Cyan;

            NetRouter.AddHandler(this);
        }

        public override void AddRecipes()
        {
            //Omni turret
            Recipe r = Recipe.Create(PlaceItems[0].Item.type, 1);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 5);
            r.AddIngredient(ItemID.IllegalGunParts, 1);
            r.AddIngredient(ItemID.DartTrap, 1);
            r.AddTile(TileID.WorkBenches);
            r.Register();

            //Super omni turret
            r = Recipe.Create(PlaceItems[1].Item.type, 1);
            r.AddIngredient(Mod.Find<ModItem>("OmniTurretItem"), 1);
            r.AddIngredient(ItemID.Cog, 10);
            r.AddTile(TileID.WorkBenches);
            r.Register();

            //Matter projector
            r = Recipe.Create(PlaceItems[2].Item.type, 1);
            r.AddIngredient(Mod.Find<ModItem>("SuperOmniTurretItem"), 1);
            r.AddIngredient(ItemID.FragmentVortex, 5);
            r.AddIngredient(ItemID.LunarBar, 5);
            r.AddTile(TileID.LunarCraftingStation);
            r.Register();
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            Rotate(reader.ReadInt16(), reader.ReadInt16(), false);
        }
    }
}