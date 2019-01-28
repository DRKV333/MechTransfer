﻿using MechTransfer.Items;
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
            AddMapEntry(new Color(200, 200, 200));
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
            Main.LocalPlayer.showItemIcon2 = placeItems[0].item.type;
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
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            i.value = Item.sellPrice(0, 0, 6, 0);
            mod.AddItem("BigButtonItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Big button");
            i.DisplayName.AddTranslation(Terraria.Localization.GameCulture.Chinese, "大号按钮");
            placeItems[0] = i;

            NetRouter.AddHandler(this);
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Lever, 1);
            r.SetResult(placeItems[0], 1);
            r.AddRecipe();

            r = new ModRecipe(mod);
            r.AddIngredient(placeItems[0], 1);
            r.SetResult(ItemID.Lever, 1);
            r.AddRecipe();
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            Wiring.TripWire(reader.ReadInt16(), reader.ReadInt16(), 2, 2);
        }
    }
}