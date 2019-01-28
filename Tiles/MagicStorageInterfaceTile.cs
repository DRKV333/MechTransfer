using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

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
            i.DisplayName.AddTranslation(Terraria.Localization.GameCulture.Chinese, "魔法存储接口");
            i.Tooltip.AddTranslation(LangID.English, "Allows you to inject and extract items from storage systems");
            i.Tooltip.AddTranslation(Terraria.Localization.GameCulture.Chinese, "使物流系统可以访问魔法存储的库存");
            placeItems[0] = i;
        }
    }
}