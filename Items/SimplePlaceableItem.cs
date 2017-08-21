using Terraria;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    public class SimplePlaceableItem : ModItem
    {
        public int placeType;
        public int style = 0;
        public ModTranslation name;
        public int value = Item.sellPrice(0, 0, 50, 0);

        public ModTranslation DName { get { return DisplayName; } }
        public ModTranslation TTip { get { return Tooltip; } }

        public override bool CloneNewInstances { get { return true; } }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = value;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.mech = true;
            item.createTile = placeType;
            item.placeStyle = style;
        }

        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void AutoStaticDefaults()
        {
            Main.itemTexture[item.type] = ModLoader.GetTexture(Texture);
        }
    }
}