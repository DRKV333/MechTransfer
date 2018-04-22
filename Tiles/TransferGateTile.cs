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
    public class TransferGateTile : SimpleTileObject, ITransferPassthrough
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

            mod.GetModWorld<TransferAgent>().passthroughs.Add(Type, this);
            mod.GetTile<TransferPipeTile>().connectedTiles.Add(Type);

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.StyleHorizontal = true;
        }

        public override void HitWire(int i, int j)
        {
            if (Main.tile[i, j].frameY == 0)
            {
                Main.tile[i, j].frameY = 18;
            }
            else
            {
                Main.tile[i, j].frameY = 0;
            }

            if (Main.netMode == 2)
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
        }

        public bool ShouldPassthrough(Point16 location, Item item)
        {
            return Main.tile[location.X, location.Y].frameY == 0;
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferGateItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer gate");
            i.Tooltip.AddTranslation(LangID.English, "Place in line with Transfer pipe to toggle the item flow with wire");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Actuator, 1);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(placeItems[0], 1);
            r.AddRecipe();
        }
    }
}