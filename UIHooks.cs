using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MechTransfer
{
    internal static class UIHooks
    {
        private static MechTransfer mod;
        private static bool loaded;

        public static void Load(MechTransfer mechTransfer) 
        {
            if (loaded) return;

            mod = mechTransfer;
            loaded = true;

            On.Terraria.Player.HandleHotbar += Player_HandleHotbar;
        }

        public static void Unload() 
        {
            if (!loaded)
                return;

            On.Terraria.Player.HandleHotbar -= Player_HandleHotbar;
        }

        private static void Player_HandleHotbar(On.Terraria.Player.orig_HandleHotbar orig, Terraria.Player self)
        {
            if (mod.assemblerHoverUI.drawnLastFrame && mod.assemblerHoverUI.PressingAlt)
                return;

            orig(self);
        }
    }
}
