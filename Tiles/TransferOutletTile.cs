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
    [Autoload(false)]
    public class TransferOutletTile : SimpleTileObject, ITransferTarget
    {
        public override void PostSetDefaults()
        {
            AddMapEntry(MapColors.Output, GetPlaceItem(0).DisplayName);

            ModContent.GetInstance<TransferAgent>().targets.Add(Type, this);
            ModContent.GetInstance<TransferPipeTile>().connectedTiles.Add(Type);

            base.PostSetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public bool Receive(Point16 location, Item item)
        {
            if (item.makeNPC > 0)
            {
                for (int i = 0; i < item.stack; i++)
                {
                    int id = NPC.NewNPC(null, location.X * 16, location.Y * 16, item.makeNPC);
                    Main.npc[id].velocity = Main.rand.NextVector2Circular(3f, 3f);
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, id);
                }
            }
            else
            {
                int dropTarget = Item.NewItem(null, location.X * 16, location.Y * 16, 16, 16, item.type, item.stack, false, item.prefix);
                Main.item[dropTarget].velocity = Vector2.Zero;
            }
            item.stack = 0;
            ModContent.GetInstance<TransferAgent>().TripWireDelayed(location.X, location.Y, 1, 1);
            return true;
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "TransferOutletItem", Type);
        }

        public override void AddRecipes()
        {
            Recipe r = Recipe.Create(PlaceItems[0].Item.type, 1);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.OutletPump, 1);
            r.AddTile(TileID.WorkBenches);
            r.Register();
        }
    }
}