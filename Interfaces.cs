using Terraria;
using Terraria.DataStructures;

namespace MechTransfer
{
    public interface ITransferTarget
    {
        ushort Type { get; }

        bool Receive(Point16 location, Item item);
    }

    public interface ITransferPassthrough
    {
        ushort Type { get; }

        bool ShouldPassthrough(Point16 location, Item item);
    }
}