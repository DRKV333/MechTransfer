using System.IO;
using Terraria;
using Terraria.ID;
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

        public override bool ValidTile(int i, int j)
        {
            return Main.tile[i, j].active() && (Main.tile[i, j].type == mod.TileType<TransferAssemblerTile>());
        }

        public override void Update()
        {
            if (timer > 0)
            {
                timer--;
                return;
            }

            if(stock.stack > 0)
            {
                foreach (var container in ((MechTransfer)mod).transferAgent.FindContainerAdjacent(Position.X, Position.Y))
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

        public override TagCompound Save()
        {
            TagCompound tags = base.Save();
            tags.Add("stck", ItemIO.Save(stock));
            return tags;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("stck"))
                stock = ItemIO.Load((TagCompound)tag["stck"]);
            base.Load(tag);
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            base.NetSend(writer, lightSend);
            writer.Write((byte)Status);
            writer.Write(MissingItemType);
            writer.WriteItem(stock, true);
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            base.NetReceive(reader, lightReceive);
            Status = (StatusKind)reader.ReadByte();
            MissingItemType = reader.ReadInt32();
            stock = reader.ReadItem(true);
        }
    }
}