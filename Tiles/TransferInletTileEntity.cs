using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.Tiles
{
    public class TransferInletTileEntity : ModTileEntity
    {
        public override bool ValidTile(int i, int j)
        {
            return Main.tile[i, j].active() && Main.tile[i, j].type == mod.TileType<TransferInletTile>() && Main.tile[i, j].frameX == 18;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3, TileChangeType.None);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override void Update()
        {
            for (int i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i].active && Main.item[i].noGrabDelay == 0)
                {
                    Item item = Main.item[i];

                    if (!((MechTransfer)mod).PickupBlacklist.Contains(item.type) && new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 48, 16).Intersects(item.getRect()))
                    {
                        TransferUtils.EatWorldItem(i, ((MechTransfer)mod).transferAgent.StartTransfer(Position.X, Position.Y, item));
                    }
                }
            }
        }
    }
}