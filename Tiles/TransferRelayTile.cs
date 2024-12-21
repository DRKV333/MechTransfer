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
    public class TransferRelayTile : SimpleTileObject, ITransferTarget
    {
        public override void PostSetDefaults()
        {
            AddMapEntry(MapColors.Passthrough, GetPlaceItem(0).DisplayName);

            ModContent.GetInstance<TransferAgent>().targets.Add(Type, this);
            ModContent.GetInstance<TransferPipeTile>().connectedTiles.Add(Type);

            base.PostSetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
        }

        public bool Receive(Point16 location, Item item)
        {
            TransferAgent agent = ModContent.GetInstance<TransferAgent>();

            Tile tile = Main.tile[location.X, location.Y];

            if (tile.TileFrameX == 0)
            {
                int decrement = agent.StartTransfer(location.X + 1, location.Y, item);
                item.stack -= decrement;
                if (decrement != 0)
                {
                    agent.TripWireDelayed(location.X, location.Y, 2, 1);
                    return true;
                }
            }
            else if (tile.TileFrameX == 54)
            {
                int decrement = agent.StartTransfer(location.X - 1, location.Y, item);
                item.stack -= decrement;
                if (decrement != 0)
                {
                    agent.TripWireDelayed(location.X - 1, location.Y, 2, 1);
                    return true;
                }
            }

            return false;
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "TransferRelayItem", Type, 32, 16);
        }

        public override void AddRecipes()
        {
            Recipe r = Recipe.Create(PlaceItems[0].Item.type, 1);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddRecipeGroup(RecipeGroupID.PressurePlate, 1);
            r.AddTile(TileID.WorkBenches);
            r.Register();
        }
    }
}