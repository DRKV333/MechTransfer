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
            // TODO: TileLoader.IsDresser?
            if (TileID.Sets.BasicChest[type] || TileID.Sets.BasicChestFake[type])
            {
                Tile bottom = Main.tile[i, j + 1];
                if (bottom != null && bottom.HasTile && noChestTiles.Contains(bottom.TileType))
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