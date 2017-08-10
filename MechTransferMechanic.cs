using MechTransfer.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer
{
    internal class MechTransferMechanic : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Mechanic)
            {
                shop.item[nextSlot].SetDefaults(mod.ItemType<PneumaticActuatorItem>());
                nextSlot++;
            }
        }
    }
}