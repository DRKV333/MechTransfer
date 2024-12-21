using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.Tiles.Simple
{
    [Autoload(false)]
    public class SimpleTileEntity : ModTileEntity
    {
        internal static Dictionary<int, int[]> validTiles;// :/

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3, TileChangeType.None);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile != null && tile.HasTile && validTiles[Type].Contains(tile.TileType);
        }

        public virtual void PostLoadPrototype()
        {
        }
    }
}