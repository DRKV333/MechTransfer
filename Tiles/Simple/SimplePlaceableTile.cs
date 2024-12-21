using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MechTransfer.Tiles.Simple
{
    public abstract class SimplePlaceableTile : SimpleTile
    {
        public ModItem PlaceItem { get; protected set; }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(PlaceItem.Item.type);
        }
    }
}