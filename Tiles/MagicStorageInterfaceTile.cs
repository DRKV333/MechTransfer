using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class MagicStorageInterfaceTile : SimpleTileObject
    {
        public override void SetDefaults()
        {
            AddMapEntry(MapColors.FillDark, GetPlaceItem(0).DisplayName);

            base.SetDefaults();
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
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("MagicStorageInterfaceItem", i);
            placeItems[0] = i;
        }
    }
}