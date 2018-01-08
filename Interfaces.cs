using Terraria;
using Terraria.DataStructures;

namespace MechTransfer
{
    public interface ITransferTarget
    {
        ushort Type { get; }

        bool Receive(TransferUtils agent, Point16 location, Item item);
    }

    public interface ITransferPassthrough
    {
        ushort Type { get; }

        bool ShouldPassthrough(TransferUtils agent, Point16 location, Item item);
    }
}