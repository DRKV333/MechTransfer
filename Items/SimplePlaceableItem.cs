using Terraria;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    public class SimplePlaceableItem : ModItem
    {
        public int placeType;
        public int style = 0;
        public int value = Item.sellPrice(0, 0, 50, 0);

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

        //Needed to stop ModLoader from assigning a default display name
        public override void AutoStaticDefaults()
        {
            Main.itemTexture[item.type] = ModLoader.GetTexture(Texture);
        }
    }
}