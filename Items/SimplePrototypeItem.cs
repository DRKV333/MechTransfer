using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    [Autoload(false)]
    public class SimplePrototypeItem : ModItem
    {
        private readonly string param_name;
        private readonly int param_place_type;
        private readonly int param_place_style;
        private readonly int param_width;
        private readonly int param_height;
        private readonly int param_value;
        public override string Name => param_name;

        public SimplePrototypeItem(string name, int placeType, int width = 16, int height = 16, int placeStyle = 0, int value = 50000)
        {
            param_name = name;
            param_place_type = placeType;
            param_width = width;
            param_height = height;
            param_place_style = placeStyle;
            param_value = value;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(param_place_type, param_place_style);
            Item.width = param_width;
            Item.height = param_height;
            Item.value = param_value;
            Item.maxStack = Item.CommonMaxStack;
            Item.mech = true;
            Item.rare = ItemRarityID.Blue;
        }

        //Nebula: Sorry but CloneNewInstances was literally the solution
        protected override bool CloneNewInstances => true;

        public static SimplePrototypeItem MakePlaceable(Mod mod, string name, int placeType, int width = 16, int height = 16, int placeStyle = 0, int value = 50000)
        {
            SimplePrototypeItem i = new SimplePrototypeItem(name, placeType, width, height, placeStyle, value);
            mod.AddContent(i);
            return i;
        }
    }
}