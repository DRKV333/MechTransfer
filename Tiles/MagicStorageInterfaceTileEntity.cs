using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MechTransfer.Tiles
{
    class MagicStorageInterfaceTileEntity : ModTileEntity
    {
        public int[] selectedTypes = new int[10];

        public void SyncData()
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)MechTransfer.ModMessageID.InterfaceSyncing);
                packet.Write(ID);
                for (int i = 0; i < selectedTypes.Length; i++)
                {
                    packet.Write(selectedTypes[i]);
                }
                packet.Send();
            }
        }

        public override bool ValidTile(int i, int j)
        {
            return Main.tile[i, j].active() && Main.tile[i, j].type == mod.TileType<MagicStorageInterfaceTile>() && Main.tile[i, j].frameX == 0 && Main.tile[i, j].frameY == 0;
        }

        public override TagCompound Save()
        {
            return new TagCompound() { { "IDs", selectedTypes } };
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("IDs"))
                selectedTypes = (int[])tag["IDs"];
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
            for (int i = 0; i < selectedTypes.Length; i++)
            {
                writer.Write(selectedTypes[i]);
            }
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            for (int i = 0; i < selectedTypes.Length; i++)
            {
                selectedTypes[i] = reader.ReadInt32();
            }
        }
        
    }
}
