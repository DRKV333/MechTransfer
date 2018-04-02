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
    public class TransferOutletTile : SimpleTileObject, ITransferTarget
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

            ((MechTransfer)mod).transferAgent.targets.Add(Type, this);

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public bool Receive(TransferUtils agent, Point16 location, Item item)
        {
            int dropTarget = Item.NewItem(location.X * 16, location.Y * 16, 16, 16, item.type, item.stack, false, item.prefix);
            Main.item[dropTarget].velocity = Vector2.Zero;
            item.stack = 0;
            mod.GetModWorld<MechTransferWorld>().TripWireDelayed(location.X, location.Y, 1, 1);
            return true;
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferOutletItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer outlet");
            i.Tooltip.AddTranslation(LangID.English, "Drops item");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.OutletPump, 1);
            r.SetResult(placeItems[0].item.type, 1);
            r.AddTile(TileID.WorkBenches);
            r.AddRecipe();
        }
    }
}