using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    [Autoload(false)]
    public class SimplePrototypeItem : ModItem
    {
        public override string Name { get; }

        [Obsolete("Manually using this constructor makes no sense")]
        public SimplePrototypeItem()
        {
            //Just so we don't have to use CloneNewInstances
        }

        public SimplePrototypeItem(Mod mod, string name)
        {
            Name = name;
            mod.AddContent(this);
        }

        public override ModItem NewInstance(Item itemClone)
        {
            ModItem newModItem = base.NewInstance(itemClone);
            CopyItemFields(itemClone, Item);
            return newModItem;
        }

        // TODO
        //Needed to stop ModLoader from assigning a default display name
        /*public override void AutoStaticDefaults()
        {
            Main.itemTexture[item.type] = ModContent.GetTexture(Texture);
        }*/

        public static SimplePrototypeItem MakePlaceable(Mod mod, string name, int placeType, int width = 16, int height = 16, int placeStyle = 0, int value = 50000)
        {
            SimplePrototypeItem i = new SimplePrototypeItem(mod, name);

            i.Item.width = width;
            i.Item.height = height;
            i.Item.value = value;
            i.Item.maxStack = 999;
            i.Item.useTurn = true;
            i.Item.autoReuse = true;
            i.Item.useAnimation = 15;
            i.Item.useTime = 10;
            i.Item.useStyle = 1;
            i.Item.consumable = true;
            i.Item.mech = true;
            i.Item.createTile = placeType;
            i.Item.placeStyle = placeStyle;
            i.Item.rare = ItemRarityID.Blue;

            return i;
        }

        //Copies every field of j to i, except for netID, active, favorited, type, modItem and globalItems
        //Made using Item.ResetStats
        public static void CopyItemFields(Item i, Item j)
        {
            //These two are not in Item.ResetStats, but I still need them...
            i.width = j.width;
            i.height = j.height;

            i.sentry = j.sentry;
            i.DD2Summon = j.DD2Summon;
            i.shopSpecialCurrency = j.shopSpecialCurrency;
            i.shopCustomPrice = j.shopCustomPrice;
            i.expert = j.expert;
            i.expertOnly = j.expertOnly;
            i.instanced = j.instanced;
            i.questItem = j.questItem;
            i.fishingPole = j.fishingPole;
            i.bait = j.bait;
            i.hairDye = j.hairDye;
            i.makeNPC = j.makeNPC;
            i.dye = j.dye;
            i.paint = j.paint;
            i.tileWand = j.tileWand;
            i.notAmmo = j.notAmmo;
            i.prefix = j.prefix;
            i.crit = j.crit;
            i.mech = j.mech;
            i.flame = j.flame;
            i.reuseDelay = j.reuseDelay;
            i.placeStyle = j.placeStyle;
            i.buffTime = j.buffTime;
            i.buffType = j.buffType;
            i.mountType = j.mountType;
            i.cartTrack = j.cartTrack;
            i.material = j.material;
            i.noWet = j.noWet;
            i.vanity = j.vanity;
            i.mana = j.mana;
            i.wet = j.wet;
            i.wetCount = j.wetCount;
            i.lavaWet = j.lavaWet;
            i.channel = j.channel;
            i.manaIncrease = j.manaIncrease;
            i.noMelee = j.noMelee;
            i.noUseGraphic = j.noUseGraphic;
            i.lifeRegen = j.lifeRegen;
            i.shootSpeed = j.shootSpeed;
            i.alpha = j.alpha;
            i.ammo = j.ammo;
            i.useAmmo = j.useAmmo;
            i.autoReuse = j.autoReuse;
            i.accessory = j.accessory;
            i.axe = j.axe;
            i.healMana = j.healMana;
            i.bodySlot = j.bodySlot;
            i.legSlot = j.legSlot;
            i.headSlot = j.headSlot;
            i.potion = j.potion;
            i.color = j.color;
            i.glowMask = j.glowMask;
            i.consumable = j.consumable;
            i.createTile = j.createTile;
            i.createWall = j.createWall;
            i.damage = j.damage;
            i.defense = j.defense;
            i.hammer = j.hammer;
            i.healLife = j.healLife;
            i.holdStyle = j.holdStyle;
            i.knockBack = j.knockBack;
            i.maxStack = j.maxStack;
            i.pick = j.pick;
            i.rare = j.rare;
            i.scale = j.scale;
            i.shoot = j.shoot;
            i.stack = j.stack;
            i.ToolTip = j.ToolTip;
            i.tileBoost = j.tileBoost;
            i.useStyle = j.useStyle;
            i.UseSound = j.UseSound;
            i.useTime = j.useTime;
            i.useAnimation = j.useAnimation;
            i.value = j.value;
            i.useTurn = j.useTurn;
            i.buy = j.buy;
            i.handOnSlot = j.handOnSlot;
            i.handOffSlot = j.handOffSlot;
            i.backSlot = j.backSlot;
            i.frontSlot = j.frontSlot;
            i.shoeSlot = j.shoeSlot;
            i.waistSlot = j.waistSlot;
            i.wingSlot = j.wingSlot;
            i.shieldSlot = j.shieldSlot;
            i.neckSlot = j.neckSlot;
            i.faceSlot = j.faceSlot;
            i.balloonSlot = j.balloonSlot;
            i.uniqueStack = j.uniqueStack;
        }
    }
}