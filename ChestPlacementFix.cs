using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer
{
    internal class ChestPlacementFix : GlobalTile
    {
        private List<int> noChestTiles = new List<int>();

        public override bool CanPlace(int i, int j, int type)
        {
            if (TileID.Sets.BasicChest[type] || TileID.Sets.BasicChestFake[type] || TileLoader.IsDresser(type))
            {
                Tile bottom = Main.tile[i, j + 1];
                if (bottom != null && bottom.active() && noChestTiles.Contains(bottom.type))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddNoChestTile(int type)
        {
            noChestTiles.Add(type);
        }
    }
}