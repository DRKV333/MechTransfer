using Terraria;

namespace MechTransfer
{
    internal static class TMLCompatExtensions
    {
        public static bool IsTheSameAs(this Item tile, Item other)
        {
            if (tile.netID == other.netID)
            {
                return tile.type == other.type;
            }
            return false;
        }
    }
}
