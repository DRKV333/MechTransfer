using Terraria;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    public class PneumaticActuatorItem : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 50, 0);
        }
    }
}