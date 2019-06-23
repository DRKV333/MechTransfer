using Terraria.ModLoader;

namespace MechTransfer.Tiles.Simple
{
    public abstract class SimplePlaceableTile : SimpleTile
    {
        public ModItem PlaceItem { get; protected set; }

        public override void SetDefaults()
        {
            drop = PlaceItem.item.type;
        }
    }
}