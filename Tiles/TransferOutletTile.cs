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
    public class TransferOutletTile : SimpleTileObject, ITransferTarget
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));

            mod.GetModWorld<TransferAgent>().targets.Add(Type, this);
            mod.GetTile<TransferPipeTile>().connectedTiles.Add(Type);

            base.SetDefaults();
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
                    int id = NPC.NewNPC(location.X * 16, location.Y * 16, item.makeNPC);
                    Main.npc[id].velocity = Main.rand.NextVector2Circular(3f, 3f);
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, id);
                }
            }
            else
            {
                int dropTarget = Item.NewItem(location.X * 16, location.Y * 16, 16, 16, item.type, item.stack, false, item.prefix);
                Main.item[dropTarget].velocity = Vector2.Zero;
            }
            item.stack = 0;
            mod.GetModWorld<TransferAgent>().TripWireDelayed(location.X, location.Y, 1, 1);
            return true;
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferOutletItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer outlet");
            i.DisplayName.AddTranslation(Terraria.Localization.GameCulture.Chinese, "物品丢弃口");
            i.Tooltip.AddTranslation(LangID.English, "Drops item");
            i.Tooltip.AddTranslation(Terraria.Localization.GameCulture.Chinese, "抛出物品");
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