using MechTransfer.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer
{
    internal class MechTransferMechanic : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Mechanic)
            {
                shop.Add(
                    new NPCShop.Entry(ModContent.ItemType<PneumaticActuatorItem>()),
                    new NPCShop.Entry(Mod.Find<ModItem>("AnyFilterItem").Item.type)
                );
            }
        }
    }
}