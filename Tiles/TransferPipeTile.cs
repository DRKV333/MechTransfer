﻿using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferPipeTile : SimpleTileObject
    {
        public HashSet<int> connectedTiles = new HashSet<int>();

        public override void SetDefaults()
        {
            base.SetDefaults();

            Main.tileFrameImportant[Type] = false;

            mod.GetModWorld<TransferAgent>().unconditionalPassthroughType = Type;

            AddMapEntry(new Color(200, 200, 200));
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            tile.frameX = 0;
            tile.frameY = 0;

            if (shouldConnect(i, j, 0, -1)) //Top
                tile.frameY += 18;

            if (shouldConnect(i, j, 0, 1)) //Bottom
                tile.frameX += 72;

            if (shouldConnect(i, j, -1, 0)) //Left
                tile.frameX += 18;

            if (shouldConnect(i, j, 1, 0)) //Right
                tile.frameX += 36;

            return false;
        }

        private bool shouldConnect(int x, int y, int offsetX, int offsetY)
        {
            Tile tile = Main.tile[x + offsetX, y + offsetY];
            if (tile != null && tile.active())
            {
                return tile.type == Type || connectedTiles.Contains(tile.type);
            }
            return false;
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferPipeItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer pipe");
            i.DisplayName.AddTranslation(Terraria.Localization.GameCulture.Chinese, "物流管道");
            i.Tooltip.AddTranslation(LangID.English, "Used to connect item transfer devices");
            i.Tooltip.AddTranslation(Terraria.Localization.GameCulture.Chinese, "用来连接物流设备和传输物品");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.IronBar, 1);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(placeItems[0].item.type, 25);
            r.AddRecipe();
        }
    }
}