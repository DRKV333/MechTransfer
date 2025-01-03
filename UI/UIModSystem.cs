using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MechTransfer.UI
{
    internal class UIModSystem : ModSystem
    {
        private GameInterfaceLayer filterLayer;
        private GameInterfaceLayer assemblerLayer;
        public FilterHoverUI filterHoverUI;
        public AssemblerHoverUI assemblerHoverUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                filterHoverUI = new FilterHoverUI();
                filterHoverUI.Activate();

                filterLayer = new LegacyGameInterfaceLayer("MechTransfer: UI_Filter",
                                                                delegate
                                                                {
                                                                    filterHoverUI.Draw(Main.spriteBatch);
                                                                    return true;
                                                                },
                                                                InterfaceScaleType.UI);

                assemblerHoverUI = new AssemblerHoverUI();
                assemblerHoverUI.Activate();

                assemblerLayer = new LegacyGameInterfaceLayer("MechTransfer: UI_Assembler",
                                                                delegate
                                                                {
                                                                    assemblerHoverUI.Draw(Main.spriteBatch);
                                                                    return true;
                                                                },
                                                                InterfaceScaleType.UI);
            }

            base.Load();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(x => x.Name == "Vanilla: Mouse Text") + 1;
            layers.Insert(index, filterLayer);
            layers.Insert(index, assemblerLayer);

            if (filterHoverUI.visible || assemblerHoverUI.visible)
            {
                layers.Find(x => x.Name == "Vanilla: Interact Item Icon").Active = false;
            }
        }
    }
}
