using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferOutletTile : ModTile, ITransferTarget
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);

            drop = mod.ItemType("TransferOutletItem");
            AddMapEntry(new Color(200, 200, 200));

            ((MechTransfer)mod).transferAgent.targets.Add(Type, this);
        }

        public bool Receive(TransferUtils agent, Point16 location, Item item)
        {
            int dropTarget = Item.NewItem(location.X * 16, location.Y * 16, item.width, item.height, item.type, item.stack, false, item.prefix);
            Main.item[dropTarget].velocity = Vector2.Zero;
            item.stack = 0;
            return true;
        }
    }
}