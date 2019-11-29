using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class SmallButtonTile : SimpleTileObject, INetHandler
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(213, 203, 204), GetPlaceItem(0).DisplayName); //Same as switch
            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
            {
                124
            };
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
            {
                124
            };
            TileObjectData.addAlternate(2);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.addAlternate(3);
        }

        public override void MouseOver(int i, int j)
        {
            Main.LocalPlayer.showItemIcon = true;
            Main.LocalPlayer.showItemIcon2 = PlaceItems[0].item.type;
            Main.LocalPlayer.noThrow = 2;
        }

        public override bool NewRightClick(int i, int j)
        {
            if (Main.netMode == 0)
            {
                Wiring.TripWire(i, j, 1, 1);
            }
            else
            {
                ModPacket packet = NetRouter.GetPacketTo(this, mod);
                packet.Write((Int16)i);
                packet.Write((Int16)j);
                packet.Send();
            }
            ModContent.GetInstance<ButtonDelayWorld>().setPoint(new Point16(i, j));

            Main.PlaySound(SoundID.MenuTick);
			return true;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (ModContent.GetInstance<ButtonDelayWorld>().isPoint(new Point16(i, j), 1, 1))
            {
                frameYOffset = 18;
            }
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(mod, "SmallButtonItem", Type, 20, 20, 0, Item.sellPrice(0, 0, 4, 0));
            PlaceItems[0].item.rare = ItemRarityID.White;

            NetRouter.AddHandler(this);
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Switch, 1);
            r.SetResult(PlaceItems[0], 1);
            r.AddRecipe();

            r = new ModRecipe(mod);
            r.AddIngredient(PlaceItems[0], 1);
            r.SetResult(ItemID.Switch, 1);
            r.AddRecipe();
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            Wiring.TripWire(reader.ReadInt16(), reader.ReadInt16(), 1, 1);
        }
    }
}