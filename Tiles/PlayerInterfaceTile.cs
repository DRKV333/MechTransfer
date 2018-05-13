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
    public class PlayerInterfaceTile : SimpleTileObject
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("PlayerInterfaceItem", i);
            i.value = Item.sellPrice(0, 1, 0, 0);
            i.DisplayName.AddTranslation(LangID.English, "Player interface");
            i.Tooltip.AddTranslation(LangID.English, "Allows you to inject and extract items from a players inventory");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.PlaceItemType<TransferExtractorTile>(0), 1);
            r.AddIngredient(mod.PlaceItemType<TransferInjectorTile>(0), 1);
            r.AddIngredient(ItemID.Mannequin, 1);
            r.AddIngredient(ItemID.Cog, 10);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(placeItems[0]);
            r.AddRecipe();
        }
    }
}