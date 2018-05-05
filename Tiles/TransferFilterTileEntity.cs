using MechTransfer.Tiles.Simple;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MechTransfer.Tiles
{
    public class TransferFilterTileEntity : SimpleTileEntity
    {
        public Item item = new Item();

        public void SyncData()
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)MechTransfer.ModMessageID.FilterSyncing);
                packet.Write(ID);
                packet.WriteItem(item);
                packet.Send();
            }
        }

        public override TagCompound Save()
        {
            return new TagCompound() { { "Item", ItemIO.Save(item) } };
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("Item"))
            {
                item = ItemIO.Load((TagCompound)tag["Item"]);
            }
            else if (tag.ContainsKey("ID"))
            {
                item.SetDefaults((int)tag["ID"]);
            }
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            ItemIO.Send(item, writer);
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            item = ItemIO.Receive(reader);
        }
    }
}