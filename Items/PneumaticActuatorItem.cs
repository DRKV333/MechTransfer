using Terraria;
using Terraria.ModLoader;

namespace MechTransfer.Items
{
    public class PneumaticActuatorItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 50, 0);
        }
    }
}