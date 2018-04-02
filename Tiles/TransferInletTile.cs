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
    public class TransferInletTile : SimpleTETile<TransferInletTileEntity>, ITransferPassthrough
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;

            AddMapEntry(new Color(200, 200, 200));

            ((MechTransfer)mod).transferAgent.passthroughs.Add(Type, this);

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            base.SetTileObjectData();
        }

        public bool ShouldPassthrough(TransferUtils agent, Point16 location, Item item)
        {
            Tile tile = Main.tile[location.X, location.Y];
            return (tile.frameX == 0 || tile.frameX == 36);
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferInletItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer inlet");
            i.Tooltip.AddTranslation(LangID.English, "Picks up dropped items");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.InletPump, 1);
            r.SetResult(placeItems[0].item.type, 1);
            r.AddTile(TileID.WorkBenches);
            r.AddRecipe();
        }
    }
}