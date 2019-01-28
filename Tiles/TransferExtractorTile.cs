﻿using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferExtractorTile : SimpleTileObject
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

            mod.GetTile<TransferPipeTile>().connectedTiles.Add(Type);

            base.SetDefaults();
        }

        public override void HitWire(int i, int j)
        {
            if (Main.netMode == 1)
                return;

            foreach (var c in mod.GetModWorld<TransferAgent>().FindContainerAdjacent(i, j))
            {
                foreach (var item in c.EnumerateItems())
                {
                    if (!item.Item1.IsAir)
                    {
                        Item clone = item.Item1.Clone();
                        clone.stack = 1;
                        if (mod.GetModWorld<TransferAgent>().StartTransfer(i, j, clone) > 0)
                        {
                            c.TakeItem(item.Item2, 1);
                            return;
                        }
                    }
                }
            }
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferExtractorItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer extractor");
            i.DisplayName.AddTranslation(Terraria.Localization.GameCulture.Chinese, "物品提取器");
            i.Tooltip.AddTranslation(LangID.English, "Extracts items from adjacent chests");
            i.Tooltip.AddTranslation(Terraria.Localization.GameCulture.Chinese, "从相邻的容器中提取物品\n一次一个");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.GoldenKey, 1);
            r.AddIngredient(ItemID.Wire, 2);
            r.SetResult(placeItems[0].item.type, 1);
            r.AddTile(TileID.WorkBenches);
            r.AddRecipe();
        }
    }
}