using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    [Autoload(false)]
    public class TransferInjectorTile : SimpleTileObject, ITransferTarget
    {
        public override void PostSetDefaults()
        {
            AddMapEntry(MapColors.Output, GetPlaceItem(0).DisplayName);

            ModContent.GetInstance<TransferAgent>().targets.Add(Type, this);
            ModContent.GetInstance<TransferPipeTile>().connectedTiles.Add(Type);

            base.PostSetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public bool Receive(Point16 location, Item item)
        {
            bool success = false;

            foreach (var container in ModContent.GetInstance<TransferAgent>().FindContainerAdjacent(location.X, location.Y))
            {
                if (container.InjectItem(item))
                    success = true;

                if (item.stack < 1)
                    break;
            }

            if (success)
                ModContent.GetInstance<TransferAgent>().TripWireDelayed(location.X, location.Y, 1, 1);

            return success;
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "TransferInjectorItem", Type);
        }

        public override void AddRecipes()
        {
            Recipe r = Recipe.Create(PlaceItems[0].Item.type, 1);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.GoldenKey, 1);
            r.AddIngredient(ItemID.Wire, 2);
            r.AddTile(TileID.WorkBenches);
            r.Register();
        }
    }
}