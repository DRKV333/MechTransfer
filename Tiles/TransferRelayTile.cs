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
    public class TransferRelayTile : SimpleTileObject, ITransferTarget
    {
        public override void SetDefaults()
        {
            AddMapEntry(MapColors.Passthrough, GetPlaceItem(0).DisplayName);

            ModContent.GetInstance<TransferAgent>().targets.Add(Type, this);
            ModContent.GetInstance<TransferPipeTile>().connectedTiles.Add(Type);

            base.SetDefaults();
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

            if (tile.frameX == 0)
            {
                int decrement = agent.StartTransfer(location.X + 1, location.Y, item);
                item.stack -= decrement;
                if (decrement != 0)
                {
                    agent.TripWireDelayed(location.X, location.Y, 2, 1);
                    return true;
                }
            }
            else if (tile.frameX == 54)
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
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(mod, "TransferRelayItem", Type, 32, 16);
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.RedPressurePlate, 1);
            r.anyPressurePlate = true;
            r.AddTile(TileID.WorkBenches);
            r.SetResult(PlaceItems[0], 1);
            r.AddRecipe();
        }
    }
}