using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MechTransfer.Tiles
{
    public class TransferFilterTileEntity : ModTileEntity
    {
        private int _ItemId = 0;

        public int ItemId
        {
            get
            {
                return _ItemId;
            }

            set
            {
                if (value != _ItemId)
                {
                    _ItemId = value;
                    NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y, 0, 0, 0, 0);
                }
            }
        }

        public override bool ValidTile(int i, int j)
        {
            return Main.tile[i, j].active() && (Main.tile[i, j].type == mod.TileType<TransferFilterTile>());
        }

        public override TagCompound Save()
        {
            return new TagCompound() { { "ID", _ItemId } };
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("ID"))
                _ItemId = (int)tag["ID"];
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
            writer.Write(_ItemId);
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            _ItemId = reader.ReadInt32();
        }
    }
}