using MechTransfer.Tiles.Simple;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MechTransfer.Tiles
{
    public class TransferFilterTileEntity : SimpleTileEntity, INetHandler
    {
        public Item item = new Item();

        public void SyncData()
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = NetRouter.GetPacketTo(ModContent.GetInstance<TransferFilterTileEntity>(), mod);
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

        public override void PostLoadPrototype()
        {
            NetRouter.AddHandler(this);
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            if (Main.netMode != 2)
                return;

            TransferFilterTileEntity FilterEntity = (TransferFilterTileEntity)ByID[reader.ReadInt32()];
            FilterEntity.item = ItemIO.Receive(reader);
            NetMessage.SendData(MessageID.TileEntitySharing, -1, WhoAmI, null, FilterEntity.ID, FilterEntity.Position.X, FilterEntity.Position.Y);
        }
    }
}