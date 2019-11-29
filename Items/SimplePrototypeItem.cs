using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    public class SimplePrototypeItem : ModItem
    {
        [Obsolete("Manually using this constructor makes no sense")]
        public SimplePrototypeItem()
        {
            //Just so we don't have to use CloneNewInstances
        }

        public SimplePrototypeItem(Mod mod, string name)
        {
            mod.AddItem(name, this);
        }

        public override ModItem NewInstance(Item itemClone)
        {
            ModItem newModItem = base.NewInstance(itemClone);
            CopyItemFields(itemClone, item);
            return newModItem;
        }

        public override bool Autoload(ref string name)
        {
            return false;
        }

        //Needed to stop ModLoader from assigning a default display name
        public override void AutoStaticDefaults()
        {
            Main.itemTexture[item.type] = ModContent.GetTexture(Texture);
        }

        public static SimplePrototypeItem MakePlaceable(Mod mod, string name, int placeType, int width = 16, int height = 16, int placeStyle = 0, int value = 50000)
        {
            SimplePrototypeItem i = new SimplePrototypeItem(mod, name);

            i.item.width = width;
            i.item.height = height;
            i.item.value = value;
            i.item.maxStack = 999;
            i.item.useTurn = true;
            i.item.autoReuse = true;
            i.item.useAnimation = 15;
            i.item.useTime = 10;
            i.item.useStyle = 1;
            i.item.consumable = true;
            i.item.mech = true;
            i.item.createTile = placeType;
            i.item.placeStyle = placeStyle;
            i.item.rare = ItemRarityID.Blue;

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
            i.thrown = j.thrown;
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
            i.melee = j.melee;
            i.magic = j.magic;
            i.ranged = j.ranged;
            i.summon = j.summon;
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
            i.release = j.release;
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