using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using MechTransfer.Tiles.Simple;

namespace MechTransfer.Tiles
{
    public class TransferFilterTileEntity : SimpleTileEntity
    {
        public int ItemId = 0;

        public void SyncData()
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)MechTransfer.ModMessageID.FilterSyncing);
                packet.Write(ID);
                packet.Write(ItemId);
                packet.Send();
            }
        }

        public override TagCompound Save()
        {
            return new TagCompound() { { "ID", ItemId } };
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("ID"))
                ItemId = (int)tag["ID"];
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write((Int32)ItemId);
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            ItemId = reader.ReadInt32();
        }
    }
}