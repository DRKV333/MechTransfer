﻿using MechTransfer.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferFilterTile : FilterableTile<TransferFilterTileEntity>, ITransferPassthrough
    {
        private Dictionary<int, ItemFilterItem> filterItems = new Dictionary<int, ItemFilterItem>();

        private HashSet<int> Bags;

        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

            mod.GetModWorld<TransferAgent>().passthroughs.Add(Type, this);
            mod.GetTile<TransferPipeTile>().connectedTiles.Add(Type);

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            base.SetTileObjectData();
        }

        public bool ShouldPassthrough(Point16 location, Item item)
        {
            TransferFilterTileEntity TE;
            if (TryGetEntity(location.X, location.Y, out TE))
            {
                ItemFilterItem filterItem;
                if (filterItems.TryGetValue(TE.item.type, out filterItem))
                {
                    if (Main.tile[location.X, location.Y].frameY == 0)
                        return filterItem.MatchesItem(item);
                    else
                        return !filterItem.MatchesItem(item);
                }
                else
                {
                    if (Main.tile[location.X, location.Y].frameY == 0)
                        return TE.item.type == item.type;
                    else
                        return TE.item.type != item.type;
                }
            }
            return false;
        }

        public override string HoverText(TransferFilterTileEntity entity)
        {
            Tile tile = Main.tile[entity.Position.X, entity.Position.Y];
            if (tile.frameY == 0)
                return "Item allowed:";
            else
                return "Item restricted:";
        }

        public override int GetDropKind(int Fx, int Fy)
        {
            return Fy / tileObjectData.CoordinateFullHeight;
        }

        public override void PostLoad()
        {
            placeItems = new ModItem[2];

            //Filter
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferFilterItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer filter (whitelist)");
            i.DisplayName.AddTranslation(GameCulture.Chinese, "物流过滤器(白名单)");
            i.Tooltip.AddTranslation(LangID.English, "Place in line with Transfer pipe\nRight click with item in hand to set filter");
            i.Tooltip.AddTranslation(GameCulture.Chinese, "将其放置在物流管道中用以过滤允许通过的物品\n手持物品右键来设置允许通过的物品\n可以使用过滤卡");
            placeItems[0] = i;

            //InverseFilter
            i = new SimplePlaceableItem();
            i.placeType = Type;
            i.style = 1;
            mod.AddItem("InverseTransferFilterItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer filter (blacklist)");
            i.DisplayName.AddTranslation(GameCulture.Chinese, "物流过滤器(黑名单)");
            i.Tooltip.AddTranslation(LangID.English, "Place in line with Transfer pipe\nRight click with item in hand to set filter");
            i.Tooltip.AddTranslation(GameCulture.Chinese, "将其放置在物流管道中用以过滤不允许通过的物品\n手持物品右键来设置不允许通过的物品\n可以使用过滤卡");
            placeItems[1] = i;

            LoadFilters();
        }

        public override void Addrecipes()
        {
            //Filter
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Actuator, 1);
            r.AddIngredient(ItemID.ItemFrame, 1);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(placeItems[0], 1);
            r.AddRecipe();
            ModRecipe r2 = new ModRecipe(mod);
            r2.AddIngredient(placeItems[1]);
            r2.SetResult(placeItems[0], 1);
            r2.AddRecipe();

            //InverseFilter
            r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Actuator, 1);
            r.AddIngredient(ItemID.ItemFrame, 1);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(placeItems[1], 1);
            r.AddRecipe();
            r2 = new ModRecipe(mod);
            r2.AddIngredient(placeItems[0]);
            r2.SetResult(placeItems[1], 1);
            r2.AddRecipe();

            LoadBagFilter();
            //LogFilterTets();
        }

        private ItemFilterItem createFilter(string type, int recipeItem, ItemFilterItem.MatchConditionn condition)
        {
            ItemFilterItem i = new ItemFilterItem(condition);
            i.recipeItem = recipeItem;
            mod.AddItem(type + "FilterItem", i);
            if (Main.halloween && type == "Dye")
            {
                i.DisplayName.AddTranslation(LangID.English, string.Format("Item filter (Die)", type));
                i.DisplayName.AddTranslation(GameCulture.Chinese, string.Format("物品过滤卡(Dye,Die,黛!)", filterTypeLocalizationStrDic[GameCulture.Chinese][type]));
            }
            else
            {
                i.DisplayName.AddTranslation(LangID.English, string.Format("Item filter ({0})", type));
                i.DisplayName.AddTranslation(GameCulture.Chinese, string.Format("物品过滤卡({0})", filterTypeLocalizationStrDic[GameCulture.Chinese][type]));
            }
            i.Tooltip.AddTranslation(LangID.English, "Use in Transfer filter");
            i.Tooltip.AddTranslation(GameCulture.Chinese, "手持它右键物流过滤器来设置指定类型的过滤条件");

            filterItems.Add(i.item.type, i);

            return i;
        }

        private void LoadFilters()
        {
            createFilter("Any", -1, x => true);

            createFilter("Rarity-Gray", ItemID.GrayPaint, x => x.rare == -1).Rarity = -1;
            createFilter("Rarity-White", ItemID.WhitePaint, x => x.rare == 0).Rarity = 0;
            createFilter("Rarity-Blue", ItemID.BluePaint, x => x.rare == 1).Rarity = 1;
            createFilter("Rarity-Green", ItemID.GreenPaint, x => x.rare == 2).Rarity = 2;
            createFilter("Rarity-Orange", ItemID.OrangePaint, x => x.rare == 3).Rarity = 3;
            createFilter("Rarity-LightRed", ItemID.RedPaint, x => x.rare == 4).Rarity = 4;
            createFilter("Rarity-Pink", ItemID.PinkPaint, x => x.rare == 5).Rarity = 5;
            createFilter("Rarity-LightPurple", ItemID.PurplePaint, x => x.rare == 6).Rarity = 6;
            createFilter("Rarity-Lime", ItemID.LimePaint, x => x.rare == 7).Rarity = 7;
            createFilter("Rarity-Yellow", ItemID.YellowPaint, x => x.rare == 8).Rarity = 8;
            createFilter("Rarity-Cyan", ItemID.CyanPaint, x => x.rare == 9).Rarity = 9;
            createFilter("Rarity-Red", ItemID.RedPaint, x => x.rare == 10).Rarity = 10;
            createFilter("Rarity-Purple", ItemID.PurplePaint, x => x.rare == 11).Rarity = 11;
            createFilter("Rarity-Rainbow", ItemID.DemonHeart, x => x.expert == true).expert = true;
            createFilter("Rarity-Amber", ItemID.Amber, x => x.rare == -11).Rarity = -11;

            createFilter("Equipable", ItemID.Shackle, x => (x.headSlot >= 0 || x.bodySlot >= 0 || x.legSlot >= 0 || x.accessory || Main.projHook[x.shoot] || x.mountType >= 0 || x.dye > 0 || (x.buffType > 0 && (Main.lightPet[x.buffType] || Main.vanityPet[x.buffType]))));
            createFilter("Armor", ItemID.WoodBreastplate, x => ((x.headSlot >= 0 || x.bodySlot >= 0 || x.legSlot >= 0) && !x.vanity));
            createFilter("Vanity", ItemID.FamiliarWig, x => (x.vanity));
            createFilter("Accessory", ItemID.Shackle, x => (x.accessory));
            createFilter("Dye", ItemID.SilverDye, x => (x.dye > 0));

            createFilter("Ammo", ItemID.MusketBall, x => x.ammo != 0);
            createFilter("Bait", ItemID.ApprenticeBait, x => x.bait > 0);
            createFilter("Money", ItemID.GoldCoin, x => x.type == ItemID.CopperCoin || x.type == ItemID.SilverCoin || x.type == ItemID.GoldCoin || x.type == ItemID.PlatinumCoin);
            createFilter("Bag", ItemID.WoodenCrate, x => Bags.Contains(x.type));

            createFilter("Tool", ItemID.CopperPickaxe, x => x.pick > 0 || x.axe > 0 || x.hammer > 0);
            createFilter("Weapon", ItemID.CopperShortsword, x => x.damage > 0 && x.pick == 0 && x.axe == 0 && x.hammer == 0);

            createFilter("Consumable", ItemID.PumpkinPie, x => x.consumable);
            createFilter("Material", ItemID.Wood, x => x.material);

            createFilter("Potion", ItemID.LesserHealingPotion, x => x.consumable && (x.healLife > 0 || x.healMana > 0 || x.buffType > 0));

            createFilter("Tile", ItemID.DirtBlock, x => x.createTile > -1);
            createFilter("Wall", ItemID.WoodWall, x => x.createWall > 0);
        }
        private Dictionary<Terraria.Localization.GameCulture, Dictionary<string, string>> filterTypeLocalizationStrDic =
            new Dictionary<Terraria.Localization.GameCulture, Dictionary<string, string>>
            {
                { GameCulture.Chinese, new Dictionary<string, string>
                    {
                        { "Any", "任意" },
                        { "Rarity-Gray", "稀有度-灰色" },
                        { "Rarity-White", "稀有度-白色" },
                        { "Rarity-Blue", "稀有度-蓝色" },
                        { "Rarity-Green", "稀有度-绿色" },
                        { "Rarity-Orange", "稀有度-橙色" },
                        { "Rarity-LightRed", "稀有度-浅红色" },
                        { "Rarity-Pink", "稀有度-粉红色" },
                        { "Rarity-LightPurple", "稀有度-浅紫色" },
                        { "Rarity-Lime", "稀有度-青柠色" },
                        { "Rarity-Yellow", "稀有度-黄色" },
                        { "Rarity-Cyan", "稀有度-青色" },
                        { "Rarity-Red", "稀有度-红色" },
                        { "Rarity-Purple", "稀有度-紫色" },
                        { "Rarity-Rainbow", "稀有度-彩虹色" },
                        { "Rarity-Amber", "稀有度-琥珀色" },
                        { "Equipable", "可装备" },
                        { "Armor", "盔甲" },
                        { "Vanity", "时装" },
                        { "Accessory", "配饰" },
                        { "Dye", "染料" },
                        { "Ammo", "弹药" },
                        { "Bait", "鱼饵" },
                        { "Money", "钱币" },
                        { "Bag", "宝藏袋" },
                        { "Tool", "工具" },
                        { "Weapon", "武器" },
                        { "Consumable", "消耗品" },
                        { "Material", "材料" },
                        { "Potion", "药水" },
                        { "Tile", "图格" },
                        { "Wall", "墙" },
                    }
                },
                //{ Terraria.Localization.GameCulture.OtherCulture, new Dictionary<string, string>
                //    {
                //        { "Any", "" },
                //        { "Rarity-Gray", "" },
                //        { "Rarity-White", "" },
                //        { "Rarity-Blue", "" },
                //        { "Rarity-Green", "" },
                //        { "Rarity-Orange", "" },
                //        { "Rarity-LightRed", "" },
                //        { "Rarity-Pink", "" },
                //        { "Rarity-LightPurple", "" },
                //        { "Rarity-Lime", "" },
                //        { "Rarity-Yellow", "" },
                //        { "Rarity-Cyan", "" },
                //        { "Rarity-Red", "" },
                //        { "Rarity-Purple", "" },
                //        { "Rarity-Rainbow", "" },
                //        { "Rarity-Amber", "" },
                //        { "Equipable", "" },
                //        { "Armor", "" },
                //        { "Vanity", "" },
                //        { "Accessory", "" },
                //        { "Dye", "" },
                //        { "Ammo", "" },
                //        { "Bait", "" },
                //        { "Money", "" },
                //        { "Bag", "" },
                //        { "Tool", "" },
                //        { "Weapon", "" },
                //        { "Consumable", "" },
                //        { "Material", "" },
                //        { "Potion", "" },
                //        { "Tile", "" },
                //        { "Wall", "" },
                //    }
                //}
            };
        private void LoadBagFilter()
        {
            Bags = new HashSet<int>();

            Bags.Add(ItemID.HerbBag);

            Bags.Add(ItemID.GoodieBag);
            Bags.Add(ItemID.Present);
            Bags.Add(ItemID.BluePresent);
            Bags.Add(ItemID.GreenPresent);
            Bags.Add(ItemID.YellowPresent);

            Bags.Add(ItemID.KingSlimeBossBag);
            Bags.Add(ItemID.EyeOfCthulhuBossBag);
            Bags.Add(ItemID.EaterOfWorldsBossBag);
            Bags.Add(ItemID.BrainOfCthulhuBossBag);
            Bags.Add(ItemID.QueenBeeBossBag);
            Bags.Add(ItemID.WallOfFleshBossBag);
            Bags.Add(ItemID.SkeletronBossBag);
            Bags.Add(ItemID.DestroyerBossBag);
            Bags.Add(ItemID.TwinsBossBag);
            Bags.Add(ItemID.SkeletronPrimeBossBag);
            Bags.Add(ItemID.PlanteraBossBag);
            Bags.Add(ItemID.GolemBossBag);
            Bags.Add(ItemID.FishronBossBag);
            Bags.Add(ItemID.CultistBossBag);
            Bags.Add(ItemID.MoonLordBossBag);
            Bags.Add(ItemID.BossBagBetsy);
            Bags.Add(ItemID.BossBagDarkMage);
            Bags.Add(ItemID.BossBagOgre);

            Bags.Add(ItemID.LockBox);
            Bags.Add(ItemID.WoodenCrate);
            Bags.Add(ItemID.IronCrate);
            Bags.Add(ItemID.GoldenCrate);
            Bags.Add(ItemID.JungleFishingCrate);
            Bags.Add(ItemID.FloatingIslandFishingCrate);
            Bags.Add(ItemID.CorruptFishingCrate);
            Bags.Add(ItemID.CrimsonFishingCrate);
            Bags.Add(ItemID.HallowedFishingCrate);
            Bags.Add(ItemID.DungeonFishingCrate);

            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                ModItem item = ItemLoader.GetItem(i);
                if (item != null && item.GetType().GetMethod("OpenBossBag").DeclaringType != typeof(ModItem))
                {
                    Bags.Add(i);
                }
            }
        }

        private void LogFilterTets()
        {
            ErrorLogger.Log("---BEGIN FILTER LISTING---");
            foreach (var item in filterItems)
            {
                //if (item.Value.Name != "BagFilterItem")
                //    continue;

                ErrorLogger.Log("----" + item.Value.DisplayName.GetDefault());
                for (int i = 0; i < ItemLoader.ItemCount; i++)
                {
                    Item testItem = new Item();
                    testItem.SetDefaults(i);

                    if (item.Value.MatchesItem(testItem))
                        ErrorLogger.Log(testItem.Name);
                }
            }
            ErrorLogger.Log("---END FILTER LISTING---");
        }
    }
}