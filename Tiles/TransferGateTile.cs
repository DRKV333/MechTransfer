﻿using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
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
            AddMapEntry(MapColors.Passthrough, GetPlaceItem(0).DisplayName);

            ModContent.GetInstance<TransferAgent>().passthroughs.Add(Type, this);
            ModContent.GetInstance<TransferPipeTile>().connectedTiles.Add(Type);

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
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(mod, "TransferGateItem", Type);
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Actuator, 1);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(PlaceItems[0], 1);
            r.AddRecipe();
        }
    }
}