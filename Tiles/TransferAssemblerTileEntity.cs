using Stubble.Core.Classes;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MechTransfer.Tiles
{
    public class TransferAssemblerTileEntity : TransferFilterTileEntity
    {
        public enum StatusKind { Ready, Success, MissingItem, MissingStation, MissingSpace, NoRecipe }

        public StatusKind Status = StatusKind.Ready;
        public int MissingItemType = 0;
        public Item stock = new Item();

        private int timer = 0;

        public override void Update()
        {
            if (timer > 0)
            {
                timer--;
                return;
            }

            if (stock.stack > 0)
            {
                foreach (var container in ModContent.GetInstance<TransferAgent>().FindContainerAdjacent(Position.X, Position.Y))
                {
                    container.InjectItem(stock);

                    if (stock.stack < 1)
                    {
                        Status = StatusKind.Success;
                        break;
                    }
                }

                if (stock.stack > 0)
                {
                    timer = 60;
                    Status = StatusKind.MissingSpace;
                }

                if (Main.netMode == 2)
                    NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (stock.stack > 0)
                tag.Add("stck", ItemIO.Save(stock));
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("stck"))
                stock = ItemIO.Load((TagCompound)tag["stck"]);
            base.LoadData(tag);
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write((byte)Status);
            writer.Write(MissingItemType);
            ItemIO.Send(stock, writer, true);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            Status = (StatusKind)reader.ReadByte();
            MissingItemType = reader.ReadInt32();
            stock = ItemIO.Receive(reader, true);
        }
    }
}
