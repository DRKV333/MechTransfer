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
                ModPacket packet = NetRouter.GetPacketTo(ModContent.GetInstance<TransferFilterTileEntity>(), Mod);
                packet.Write(ID);
                ItemIO.Send(item, packet);
                packet.Send();
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["item"] = ItemIO.Save(item);
        }

        public override void LoadData(TagCompound tag)
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

        public override void NetSend(BinaryWriter writer)
        {
            ItemIO.Send(item, writer);
        }

        public override void NetReceive(BinaryReader reader)
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