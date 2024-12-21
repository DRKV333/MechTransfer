using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.ModLoader;

namespace MechTransfer.Tiles
{
    [Autoload(false)]
    public class MagicStorageExtraInterfaceTile : SimpleTileObject
    {
        public override void PostSetDefaults()
        {
            AddMapEntry(MapColors.FillDark, GetPlaceItem(0).DisplayName);

            base.PostSetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "MagicStorageExtraInterfaceItem", Type, 32, 32);
        }
    }
}