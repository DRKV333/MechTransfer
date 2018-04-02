using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using MechTransfer.Tiles.Simple;
using MechTransfer.Items;
using Terraria.ID;

namespace MechTransfer.Tiles
{
    public class MagicStorageInterfaceTile : SimpleTileObject
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

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
            i.DisplayName.AddTranslation(LangID.English, "Magic storage interface");
            i.Tooltip.AddTranslation(LangID.English, "Allows you to inject and extract items from storage systems");
            placeItems[0] = i;
        }
    }
}