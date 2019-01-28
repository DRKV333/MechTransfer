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
            AddMapEntry(new Color(200, 200, 200));
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
            Main.LocalPlayer.showItemIcon2 = placeItems[0].item.type;
            Main.LocalPlayer.noThrow = 2;
        }

        public override void RightClick(int i, int j)
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
            mod.GetModWorld<ButtonDelayWorld>().setPoint(new Point16(i, j));

            Main.PlaySound(SoundID.MenuTick);
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (mod.GetModWorld<ButtonDelayWorld>().isPoint(new Point16(i, j), 1, 1))
            {
                frameYOffset = 18;
            }
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            i.value = Item.sellPrice(0, 0, 4, 0);
            mod.AddItem("SmallButtonItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Small button");
            i.DisplayName.AddTranslation(Terraria.Localization.GameCulture.Chinese, "小号按钮");
            placeItems[0] = i;

            NetRouter.AddHandler(this);
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Switch, 1);
            r.SetResult(placeItems[0], 1);
            r.AddRecipe();

            r = new ModRecipe(mod);
            r.AddIngredient(placeItems[0], 1);
            r.SetResult(ItemID.Switch, 1);
            r.AddRecipe();
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            Wiring.TripWire(reader.ReadInt16(), reader.ReadInt16(), 1, 1);
        }
    }
}