using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Microsoft.Xna.Framework;
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
            AddMapEntry(new Color(200, 200, 200));

            ((MechTransfer)mod).transferAgent.targets.Add(Type, this);

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

        public bool Receive(TransferUtils agent, Point16 location, Item item)
        {
            Tile tile = Main.tile[location.X, location.Y];

            if (tile.frameX == 0)
            {
                int decrement = agent.StartTransfer(location.X + 1, location.Y, item);
                item.stack -= decrement;
                if (decrement != 0)
                {
                    mod.GetModWorld<MechTransferWorld>().TripWireDelayed(location.X, location.Y, 2, 1);
                    return true;
                }
            }
            else if (tile.frameX == 54)
            {
                int decrement = agent.StartTransfer(location.X - 1, location.Y, item);
                item.stack -= decrement;
                if (decrement != 0)
                {
                    mod.GetModWorld<MechTransferWorld>().TripWireDelayed(location.X - 1, location.Y, 2, 1);
                    return true;
                }
            }

            return false;
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferRelayItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer relay");
            i.Tooltip.AddTranslation(LangID.English, "Receives items, and sends them out again");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.RedPressurePlate, 1);
            r.anyPressurePlate = true;
            r.AddTile(TileID.WorkBenches);
            r.SetResult(placeItems[0], 1);
            r.AddRecipe();
        }
    }
}