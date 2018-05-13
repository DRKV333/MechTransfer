using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    public class ItemFilterItem : ModItem
    {
        public delegate bool MatchConditionn(Item item);

        public int recipeItem = -1;
        public int Rarity = 0;
        public bool expert = false;

        private MatchConditionn matchConditionn;

        public override bool CloneNewInstances { get { return true; } }

        public ItemFilterItem(MatchConditionn matchConditionn)
        {
            this.matchConditionn = matchConditionn;
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 1;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = Rarity;
            item.expert = expert;
        }

        public bool MatchesItem(Item item)
        {
            return matchConditionn(item);
        }

        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void AddRecipes()
        {
            if (recipeItem != -1)
            {
                ModRecipe r = new ModRecipe(mod);
                r.AddIngredient(mod.ItemType("AnyFilterItem"), 1);
                r.AddIngredient(recipeItem, 1);
                r.SetResult(item.type, 1);
                r.AddTile(TileID.WorkBenches);
                r.AddRecipe();
            }
        }

        //Needed to stop ModLoader from assigning a default display name
        public override void AutoStaticDefaults()
        {
            Main.itemTexture[item.type] = ModLoader.GetTexture(Texture);
        }
    }
}