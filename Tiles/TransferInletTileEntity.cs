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

        private Rectangle PickupArea()
        {
            return new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 48, 16);
        }

        public override void Update()
        {
            for (int i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i].active && Main.item[i].noGrabDelay == 0)
                {
                    Item item = Main.item[i];

                    if (!PickupBlacklist.Contains(item.type) && PickupArea().Intersects(item.getRect()))
                    {
                        item.stack -= mod.GetModWorld<TransferAgent>().StartTransfer(Position.X, Position.Y, item);
                        if (item.stack < 1)
                            Main.item[i] = new Item();

                        if (Main.netMode == 2)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                    }
                }
            }

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && PickupArea().Intersects(npc.Hitbox) && npc.catchItem > 0)
                {
                    bool remove = false;
                    if (npc.SpawnedFromStatue)
                    {
                        Vector2 PoofSite = npc.Center - new Vector2(20f);
                        Utils.PoofOfSmoke(PoofSite);
                        NetMessage.SendData(MessageID.PoofOfSmoke, -1, -1, null, (int)PoofSite.X, PoofSite.Y);
                        remove = true;
                    }
                    else
                    {
                        Item item = new Item();
                        item.SetDefaults(npc.catchItem);
                        if (mod.GetModWorld<TransferAgent>().StartTransfer(Position.X, Position.Y, item) > 0)
                            remove = true;
                    }
                    if (remove)
                    {
                        npc.active = false;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
                    }
                }
            }
        }
    }
}