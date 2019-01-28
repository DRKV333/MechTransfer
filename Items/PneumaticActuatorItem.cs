using Terraria;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    public class PneumaticActuatorItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pneumatic actuator");
            DisplayName.AddTranslation(Terraria.Localization.GameCulture.Chinese, "气动执行器");
            Tooltip.SetDefault("Used to craft item transfer devices");
            Tooltip.AddTranslation(Terraria.Localization.GameCulture.Chinese, "合成物流设备的必需品");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 50, 0);
        }
    }
}