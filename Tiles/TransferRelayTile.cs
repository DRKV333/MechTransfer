using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferRelayTile : ModTile, ITransferTarget
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200));

            ((MechTransfer)mod).transferAgent.targets.Add(Type, this);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 16, mod.ItemType("TransferRelayItem"));
        }

        public bool Receive(TransferUtils agent, Point16 location, Item item)
        {
            Tile tile = Main.tile[location.X, location.Y];

            if (tile.frameX == 0)
            {
                int decrement = agent.StartTransfer(location.X + 1, location.Y, item);
                item.stack -= decrement;
                if (decrement != 0)
                {
                    mod.GetModWorld<MechTransferWorld>().TripWireDelayed(location.X, location.Y, 2, 1);
                    return true;
                }
            }
            else if (tile.frameX == 54)
            {
                int decrement = agent.StartTransfer(location.X - 1, location.Y, item);
                item.stack -= decrement;
                if (decrement != 0)
                {
                    mod.GetModWorld<MechTransferWorld>().TripWireDelayed(location.X - 1, location.Y, 2, 1);
                    return true;
                }
            }

            return false;
        }
    }
}