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
    public class TransferExtractorTile : SimpleTileObject
    {
        public override void PostSetDefaults()
        {
            AddMapEntry(MapColors.Input, GetPlaceItem(0).DisplayName);

            ModContent.GetInstance<TransferPipeTile>().connectedTiles.Add(Type);

            base.PostSetDefaults();
        }

        public override void HitWire(int i, int j)
        {
            if (Main.netMode == 1)
                return;

            foreach (var c in ModContent.GetInstance<TransferAgent>().FindContainerAdjacent(i, j))
            {
                foreach (var item in c.EnumerateItems())
                {
                    if (!item.Item1.IsAir)
                    {
                        Item clone = item.Item1.Clone();
                        clone.stack = 1;
                        if (ModContent.GetInstance<TransferAgent>().StartTransfer(i, j, clone) > 0)
                        {
                            c.TakeItem(item.Item2, 1);
                            return;
                        }
                    }
                }
            }
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "TransferExtractorItem", Type);
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