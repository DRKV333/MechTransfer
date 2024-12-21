using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    [Autoload(false)]
    public class TransferPipeTile : SimpleTileObject
    {
        public HashSet<int> connectedTiles = new HashSet<int>();

        public override void PostSetDefaults()
        {
            base.PostSetDefaults();

            Main.tileFrameImportant[Type] = false;

            ModContent.GetInstance<TransferAgent>().unconditionalPassthroughType = Type;

            AddMapEntry(MapColors.FillMid);
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Main.tile[i, j];
            tile.TileFrameX = 0;
            tile.TileFrameY = 0;

            if (shouldConnect(i, j, 0, -1)) //Top
                tile.TileFrameY += 18;

            if (shouldConnect(i, j, 0, 1)) //Bottom
                tile.TileFrameX += 72;

            if (shouldConnect(i, j, -1, 0)) //Left
                tile.TileFrameX += 18;

            if (shouldConnect(i, j, 1, 0)) //Right
                tile.TileFrameX += 36;

            return false;
        }

        private bool shouldConnect(int x, int y, int offsetX, int offsetY)
        {
            Tile tile = Main.tile[x + offsetX, y + offsetY];
            if (tile != null && tile.HasTile)
            {
                return tile.TileType == Type || connectedTiles.Contains(tile.TileType);
            }
            return false;
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "TransferPipeItem", Type);
        }

        public override void AddRecipes()
        {
            Recipe r = Recipe.Create(PlaceItems[0].Item.type, 25);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddRecipeGroup(RecipeGroupID.IronBar, 1);
            r.AddTile(TileID.Anvils);
            r.Register();
        }
    }
}