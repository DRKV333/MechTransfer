using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace MechTransfer
{
    public static class RecipeUtils
    {
        public static int HashRecipe(Recipe recipe)
        {
            int hash = 217301651;

            for (int ing = 0; ing < recipe.requiredItem.Length; ing++)
                if (recipe.requiredItem[ing].IsAir) break;
                else
                {
                    hash *= recipe.requiredItem[ing].type * -1444222980 * (recipe.requiredItem[ing].stack + 1);
                }
            return hash;
        }
    }
}
