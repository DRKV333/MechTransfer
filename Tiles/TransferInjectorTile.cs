using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using MechTransfer.Tiles.Simple;
using MechTransfer.Items;
using Terraria.ID;

namespace MechTransfer.Tiles
{
    public class TransferInjectorTile : SimpleTileObject, ITransferTarget
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

            ((MechTransfer)mod).transferAgent.targets.Add(Type, this);

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public bool Receive(TransferUtils agent, Point16 location, Item item)
        {
            bool success = false;

            foreach (var container in agent.FindContainerAdjacent(location.X, location.Y))
            {
                if (container.InjectItem(item))
                    success = true;

                if (item.stack < 1)
                    break;
            }

            if (success)
                mod.GetModWorld<MechTransferWorld>().TripWireDelayed(location.X, location.Y, 1, 1);

            return success;
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferInjectorItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer Injector");
            i.Tooltip.AddTranslation(LangID.English, "Injects items into adjacent chests");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.GoldenKey, 1);
            r.AddIngredient(ItemID.Wire, 2);
            r.SetResult(placeItems[0], 1);
            r.AddTile(TileID.WorkBenches);
            r.AddRecipe();
        }
    }
}