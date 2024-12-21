using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    [Autoload(false)]
    public class BigButtonTile : SimpleTileObject, INetHandler
    {
        public override void PostSetDefaults()
        {
            AddMapEntry(new Color(144, 148, 144), GetPlaceItem(0).DisplayName); //Same as lever
            base.PostSetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16
            };
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.addAlternate(2);
        }

        public override void MouseOver(int i, int j)
        {
            // TODO
            // Main.LocalPlayer.showItemIcon = true;
            // Main.LocalPlayer.showItemIcon2 = PlaceItems[0].Item.type;
            
            Main.LocalPlayer.noThrow = 2;
        }

        public override bool RightClick(int i, int j)
        {
            Point16 origin = GetOrigin(i, j);
            Point16 topLeft = origin - tileObjectData.Origin;

            if (Main.netMode == 0)
            {
                Wiring.TripWire(topLeft.X, topLeft.Y, 2, 2);
            }
            else
            {
                ModPacket packet = NetRouter.GetPacketTo(this, Mod);
                packet.Write(topLeft.X);
                packet.Write(topLeft.Y);
                packet.Send();
            }
			ModContent.GetInstance<ButtonDelayWorld>().setPoint(topLeft);

            // TODO: Figure out how to play sound.
            // Main.PlaySound(SoundID.MenuTick);

            return true;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (ModContent.GetInstance<ButtonDelayWorld>().isPoint(new Point16(i, j), 2, 2))
            {
                frameXOffset = 36;
            }
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "BigButtonItem", Type, 24, 20);
            PlaceItems[0].Item.rare = ItemRarityID.White;

            NetRouter.AddHandler(this);
        }

        public override void AddRecipes()
        {
            Recipe r = Recipe.Create(PlaceItems[0].Item.type, 1);
            r.AddIngredient(ItemID.Lever, 1);
            r.Register();

            r = Recipe.Create(ItemID.Lever, 1);
            r.AddIngredient(PlaceItems[0], 1);
            r.Register();
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            Wiring.TripWire(reader.ReadInt16(), reader.ReadInt16(), 2, 2);
        }
    }
}