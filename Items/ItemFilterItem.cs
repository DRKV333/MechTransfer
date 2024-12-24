using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    [Autoload(false)]
    public class ItemFilterItem : ModItem
    {
        public delegate bool MatchConditionn(Item item);

        public int recipeItem = -1;
        public int Rarity = 0;
        public bool expert = false;

        private MatchConditionn matchConditionn;

        protected override bool CloneNewInstances => true;

        public override string Name { get; }

        public ItemFilterItem(string name, MatchConditionn matchConditionn)
        {
            Name = name;
            this.matchConditionn = matchConditionn;
        }

        public override LocalizedText DisplayName
        {
            get
            {
                if (Main.halloween && Name == "DyeFilterItem") //Easter egg name handling
                    return Language.GetText("Mods.MechTransfer.Items.DyeFilterItem.EasterEggName");
                return base.DisplayName;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = Rarity;
            Item.expert = expert;
        }

        public bool MatchesItem(Item item)
        {
            return matchConditionn(item);
        }

        public override void AddRecipes()
        {
            if (recipeItem != -1)
            {
                Recipe r = CreateRecipe();
                r.AddIngredient(Mod.Find<ModItem>("AnyFilterItem").Item.type, 1);
                r.AddIngredient(recipeItem, 1);
                r.AddTile(TileID.WorkBenches);
                r.Register();
            }
        }

        // TODO
        //Needed to stop ModLoader from assigning a default display name
        /*public override void AutoStaticDefaults()
        {
            Main.itemTexture[Item.type] = ModContent.GetTexture(Texture);
        }*/
    }
}