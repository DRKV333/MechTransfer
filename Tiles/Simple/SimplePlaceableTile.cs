using Terraria.ModLoader;

namespace MechTransfer.Tiles.Simple
{
    public abstract class SimplePlaceableTile : SimpleTile
    {
        public ModItem placeItem { get; protected set; }

        public override void PostLoad()
        {
            mod.AddItem(LoadPlaceItem(), placeItem);
        }

        public override void SetDefaults()
        {
            drop = placeItem.item.type;
        }

        public abstract string LoadPlaceItem();
    }
}