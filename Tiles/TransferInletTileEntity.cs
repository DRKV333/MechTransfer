using MechTransfer.Tiles.Simple;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace MechTransfer.Tiles
{
    public class TransferInletTileEntity : SimpleTileEntity
    {
        public static HashSet<int> PickupBlacklist = new HashSet<int>();

        public override void Update()
        {
            for (int i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i].active && Main.item[i].noGrabDelay == 0)
                {
                    Item item = Main.item[i];

                    if (!PickupBlacklist.Contains(item.type) && new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 48, 16).Intersects(item.getRect()))
                    {
                        item.stack -= mod.GetModWorld<TransferAgent>().StartTransfer(Position.X, Position.Y, item);
                        if (item.stack < 1)
                            Main.item[i] = new Item();

                        if (Main.netMode == 2)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                    }
                }
            }
        }
    }
}