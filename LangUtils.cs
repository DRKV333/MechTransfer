using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Reflection;

namespace MechTransfer
{
    static class LangUtils
    {
        public static ModTranslation GetModTranslation(this Mod mod, string key)
        {
            IDictionary<string, ModTranslation> translations = (IDictionary<string, ModTranslation>)(typeof(Mod).GetField("translations", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mod));
            return translations[key];
        }

        public static void SwapTranslation(this ModTranslation target, ModTranslation source)
        {
            //Currently the Steam API supports 29 languages
            for (int i = 1; i <= 29; i++)
            {
                string translation = source.GetTranslation(i);
                if (i == 1 || translation != source.GetDefault())
                    target.AddTranslation(i, translation);
            }
        }
    }
}
