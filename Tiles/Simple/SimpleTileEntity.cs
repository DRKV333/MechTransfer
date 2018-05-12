using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.Tiles.Simple
{
    public class SimpleTileEntity : ModTileEntity
    {
        internal static Dictionary<int, int[]> validTiles;// :/

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3, TileChangeType.None);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile != null && tile.active() && validTiles[Type].Contains(tile.type);
        }

        public virtual void PostLoadPrototype()
        {
        }

        public override bool Autoload(ref string name)
        {
            return false;
        }
    }
}