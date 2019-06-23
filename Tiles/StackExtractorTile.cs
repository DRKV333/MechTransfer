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
    public class StackExtractorTile : SimpleTileObject
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(56, 56, 56), GetPlaceItem(0).DisplayName);

            mod.GetTile<TransferPipeTile>().connectedTiles.Add(Type);

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public override void HitWire(int i, int j)
        {
            if (Main.netMode == 1)
                return;

            foreach (var c in mod.GetModWorld<TransferAgent>().FindContainerAdjacent(i, j))
            {
                foreach (var item in c.EnumerateItems())
                {
                    if (!item.Item1.IsAir)
                    {
                        Item clone = item.Item1.Clone();
                        int numTook = mod.GetModWorld<TransferAgent>().StartTransfer(i, j, clone);
                        if (numTook > 0)
                        {
                            c.TakeItem(item.Item2, numTook);
                            return;
                        }
                    }
                }
            }
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(mod, "StackExtractorItem", Type, 16, 16, 0, Item.sellPrice(0, 1, 0, 0));
            PlaceItems[0].item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType("TransferExtractorItem"), 1);
            r.AddIngredient(ItemID.Nanites, 10);
            r.SetResult(PlaceItems[0], 1);
            r.AddTile(TileID.WorkBenches);
            r.AddRecipe();
        }
    }
}