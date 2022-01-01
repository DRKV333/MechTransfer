using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace MechTransfer.UI
{
    internal class RecipeUI : UIPanel
    {
        public Recipe recipe = null;
        public bool highlight = false;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (recipe is null) return;

            int ing;
            for (ing = 0; ing < recipe.requiredItem.Length; ing++)
            {
                if (recipe.requiredItem[ing].IsAir)
                {
                    break;
                }
            }

            Height.Pixels = Main.inventoryBackTexture.Height * Main.inventoryScale + 10;
            Width.Pixels = Main.inventoryBackTexture.Width * ing * Main.inventoryScale + 10;

            Recalculate();

            if (highlight)
            {
                BackgroundColor = Color.Green;
            }
            else BackgroundColor = new Color(63, 82, 151) * 0.7f;

            base.DrawSelf(spriteBatch);

            CalculatedStyle dim = GetDimensions();

            float x;

            for (int i = 0; i < ing; i++) 
            {
                x = Main.inventoryBackTexture.Width * Main.inventoryScale;
                
                ItemSlot.Draw(spriteBatch, ref recipe.requiredItem[i], ItemSlot.Context.InventoryCoin, new Vector2(dim.X + 5 + (x * i), dim.Y + 5));
            }
        }
    }
}
