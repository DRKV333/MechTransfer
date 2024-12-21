using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MechTransfer.UI
{
    internal class UIModSystem : ModSystem
    {
        private GameInterfaceLayer interfaceLayer;
        public FilterHoverUI filterHoverUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                filterHoverUI = new FilterHoverUI();
                filterHoverUI.Activate();

                interfaceLayer = new LegacyGameInterfaceLayer("MechTransfer: UI",
                                                                delegate
                                                                {
                                                                    filterHoverUI.Draw(Main.spriteBatch);
                                                                    return true;
                                                                },
                                                                InterfaceScaleType.UI);
            }

            base.Load();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(x => x.Name == "Vanilla: Mouse Text") + 1;
            layers.Insert(index, interfaceLayer);

            if (filterHoverUI.visible)
            {
                layers.Find(x => x.Name == "Vanilla: Interact Item Icon").Active = false;
            }
        }
    }
}
