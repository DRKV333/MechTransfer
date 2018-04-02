using MechTransfer.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferFilterTile : FilterableTile<TransferFilterTileEntity>, ITransferPassthrough
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

            ((MechTransfer)mod).transferAgent.passthroughs.Add(Type, this);

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            base.SetTileObjectData();
        }

        public bool ShouldPassthrough(TransferUtils agent, Point16 location, Item item)
        {
            TransferFilterTileEntity TE;
            if (TryGetEntity(location.X, location.Y, out TE))
            {
                if (Main.tile[location.X, location.Y].frameY == 0)
                    return TE.ItemId == item.type;
                else
                    return TE.ItemId != item.type;
            }
            return false;
        }

        public override string HoverText(TransferFilterTileEntity entity)
        {
            Tile tile = Main.tile[entity.Position.X, entity.Position.Y];
            if (tile.frameY == 0)
                return "Item allowed:";
            else
                return "Item restricted:";
        }

        public override int GetDropKind(int Fx, int Fy)
        {
            return Fy / tileObjectData.CoordinateFullHeight;
        }

        public override void PostLoad()
        {
            placeItems = new ModItem[2];

            //Filter
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferFilterItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer filter (whitelist)");
            i.Tooltip.AddTranslation(LangID.English, "Place in line with Transfer pipe\nRight click with item in hand to set filter");
            placeItems[0] = i;

            //InverseFilter
            i = new SimplePlaceableItem();
            i.placeType = Type;
            i.style = 1;
            mod.AddItem("InverseTransferFilterItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer filter (blacklist)");
            i.Tooltip.AddTranslation(LangID.English, "Place in line with Transfer pipe\nRight click with item in hand to set filter");
            placeItems[1] = i;
        }

        public override void Addrecipes()
        {
            //Filter
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Actuator, 1);
            r.AddIngredient(ItemID.ItemFrame, 1);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(placeItems[0], 1);
            r.AddRecipe();

            //InverseFilter
            r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Actuator, 1);
            r.AddIngredient(ItemID.ItemFrame, 1);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(placeItems[1], 1);
            r.AddRecipe();
        }
    }
}