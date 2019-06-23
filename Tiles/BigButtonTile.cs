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
    public class BigButtonTile : SimpleTileObject, INetHandler
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(144, 148, 144), GetPlaceItem(0).DisplayName); //Same as lever
            base.SetDefaults();
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
            Main.LocalPlayer.showItemIcon = true;
            Main.LocalPlayer.showItemIcon2 = PlaceItems[0].item.type;
            Main.LocalPlayer.noThrow = 2;
        }

        public override void RightClick(int i, int j)
        {
            Point16 origin = GetOrigin(i, j);
            Point16 topLeft = origin - tileObjectData.Origin;

            if (Main.netMode == 0)
            {
                Wiring.TripWire(topLeft.X, topLeft.Y, 2, 2);
            }
            else
            {
                ModPacket packet = NetRouter.GetPacketTo(this, mod);
                packet.Write(topLeft.X);
                packet.Write(topLeft.Y);
                packet.Send();
            }
            mod.GetModWorld<ButtonDelayWorld>().setPoint(topLeft);

            Main.PlaySound(SoundID.MenuTick);
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (mod.GetModWorld<ButtonDelayWorld>().isPoint(new Point16(i, j), 2, 2))
            {
                frameXOffset = 36;
            }
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(mod, "BigButtonItem", Type, 24, 20);
            PlaceItems[0].item.rare = ItemRarityID.White;

            NetRouter.AddHandler(this);
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Lever, 1);
            r.SetResult(PlaceItems[0], 1);
            r.AddRecipe();

            r = new ModRecipe(mod);
            r.AddIngredient(PlaceItems[0], 1);
            r.SetResult(ItemID.Lever, 1);
            r.AddRecipe();
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            Wiring.TripWire(reader.ReadInt16(), reader.ReadInt16(), 2, 2);
        }
    }
}