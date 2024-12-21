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
    public class TransferGateTile : SimpleTileObject, ITransferPassthrough
    {
        public override void PostSetDefaults()
        {
            AddMapEntry(MapColors.Passthrough, GetPlaceItem(0).DisplayName);

            ModContent.GetInstance<TransferAgent>().passthroughs.Add(Type, this);
            ModContent.GetInstance<TransferPipeTile>().connectedTiles.Add(Type);

            base.PostSetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.StyleHorizontal = true;
        }

        public override void HitWire(int i, int j)
        {
            if (Main.tile[i, j].TileFrameY == 0)
            {
                Main.tile[i, j].TileFrameY = 18;
            }
            else
            {
                Main.tile[i, j].TileFrameY = 0;
            }

            if (Main.netMode == 2)
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
        }

        public bool ShouldPassthrough(Point16 location, Item item)
        {
            return Main.tile[location.X, location.Y].TileFrameY == 0;
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "TransferGateItem", Type);
        }

        public override void AddRecipes()
        {
            Recipe r = Recipe.Create(PlaceItems[0].Item.type, 1);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Actuator, 1);
            r.AddTile(TileID.WorkBenches);
            r.Register();
        }
    }
}