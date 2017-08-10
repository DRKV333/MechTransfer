using Terraria;

namespace MechTransfer.Tiles
{
    public class TransferAssemblerTileEntity : TransferFilterTileEntity
    {
        public enum StatusKind { Ready, Success, MissingItem, MissingStation, MissingSpace }

        public StatusKind Status = StatusKind.Ready;
        public int MissingItemType = 0;

        public override bool ValidTile(int i, int j)
        {
            return Main.tile[i, j].active() && (Main.tile[i, j].type == mod.TileType<TransferAssemblerTile>());
        }
    }
}