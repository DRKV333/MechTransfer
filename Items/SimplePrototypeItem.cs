using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    [Autoload(false)]
    public class SimplePrototypeItem : ModItem
    {
        public override string Name { get; }
        
        private readonly string param_name;
        private readonly int placeType;
        private readonly int placeStyle;
        private readonly int width;
        private readonly int height;
        private readonly int value;

        public SimplePrototypeItem(string name, int placeType, int width = 16, int height = 16, int placeStyle = 0, int value = 50000)
        {
            Name = name;
            this.placeType = placeType;
            this.width = width;
            this.height = height;
            this.placeStyle = placeStyle;
            this.value = value;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(placeType, placeStyle);
            Item.width = width;
            Item.height = height;
            Item.value = value;
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