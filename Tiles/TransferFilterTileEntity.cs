using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MechTransfer.Tiles
{
    public class TransferFilterTileEntity : ModTileEntity
    {
        public int ItemId = 0;

        public void SyncData()
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((int)MechTransfer.ModMessageID.FilterSyncing);
                packet.Write(ID);
                packet.Write(ItemId);
                packet.Send();
            }
        }

        public override bool ValidTile(int i, int j)
        {
            return Main.tile[i, j].active() && (Main.tile[i, j].type == mod.TileType<TransferFilterTile>());
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

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, TileChangeType.None);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
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