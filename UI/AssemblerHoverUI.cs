using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace MechTransfer.UI
{
    public class AssemblerHoverUI : UIState
    {
        public bool visible = false;

        private UIPanel panel;
        private UIText itemText;
        private UIText titleText;
        private Item[] fakeInv = new Item[11];
        private List<RecipeUI> recipes = new List<RecipeUI>();

        private const string NotSetTextKey = "Mods.MechTransfer.UI.Hover.NotSet";

        public override void OnInitialize()
        {
            fakeInv[10] = new Item();

            titleText = new UIText(Language.GetTextValue(NotSetTextKey));
            titleText.Left.Set(5, 0);
            titleText.Top.Set(5, 0);
            Append(titleText);

            panel = new UIPanel();
            panel.Left.Set(0, 0);
            panel.Top.Set(25, 0);
            panel.Width.Set(200, 0);
            panel.Height.Set(Main.inventoryBackTexture.Height * 0.5f * Main.UIScale + 10, 0);

            itemText = new UIText(Language.GetTextValue(NotSetTextKey));
            itemText.Left.Set(Main.inventoryBackTexture.Width * 0.5f * Main.UIScale, 0);
            itemText.Top.Set(0, 0);
            panel.Append(itemText);

            Append(panel);
        }

        public void Display(Item type, string text, Color textColor, int selectedHash, List<Recipe> foundRecipes)
        {
            visible = true;

            titleText.SetText(text);
            titleText.TextColor = textColor;

            fakeInv[10] = type;
            itemText.SetText(type.IsAir ? Language.GetTextValue(NotSetTextKey) : fakeInv[10].Name);
            itemText.TextColor = type.IsAir ? Color.Red : ItemRarity.GetColor(fakeInv[10].rare);
            panel.Width.Pixels = itemText.MinWidth.Pixels + Main.inventoryBackTexture.Width * 0.5f * Main.UIScale + 20;

            Vector2 pos = Vector2.Transform(Main.MouseScreen, Main.GameViewMatrix.TransformationMatrix);

            Left.Pixels = (pos.X + 10) / Main.UIScale;
            Top.Pixels = (pos.Y + 10) / Main.UIScale;

            int i = 0;

            foundRecipes.Clear();

            for (int r = 0; r < Recipe.maxRecipes && !Main.recipe[r].createItem.IsAir; r++)
            {
                if (Main.recipe[r].createItem.type == type.type)
                {
                    if (recipes.Count <= i)
                    {
                        RecipeUI recipe = new RecipeUI();
                        recipes.Add(recipe);
                        Append(recipe);
                    }
                    foundRecipes.Add(Main.recipe[r]);
                    recipes[i].recipe = Main.recipe[r];

                    recipes[i].highlight = false;

                    if (selectedHash != 0 && selectedHash == RecipeUtils.HashRecipe(Main.recipe[r])) recipes[i].highlight = true;

                    i++;
                }
            }

            for (int j = 0; j < recipes.Count; j++)
            {
                if (j >= i && recipes[j].recipe != null)
                {
                    recipes[j].recipe = null;
                    break;
                }

                recipes[j].Top.Pixels = panel.Top.Pixels + panel.Height.Pixels + 5 + (j * (Main.inventoryBackTexture.Height * 0.5f + 15));
                recipes[j].Left.Pixels = 0;
            }

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 0.5f;

            base.Draw(spriteBatch);
            ItemSlot.Draw(spriteBatch, fakeInv, ItemSlot.Context.InventoryItem, 10, new Vector2(Left.Pixels + 5, Top.Pixels + 30));
            Utils.DrawBorderString(spriteBatch, "Scroll to select recipe", 
                new Vector2(Left.Pixels + Main.inventoryBackTexture.Width * 0.5f * Main.UIScale + 15, Top.Pixels + 65), Color.White, Main.UIScale);

            Main.inventoryScale = oldScale;

            visible = false;
        }
    }
}