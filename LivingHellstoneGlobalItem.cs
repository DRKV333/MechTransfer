using MechTransfer.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MechTransfer
{
    internal class LivingHellstoneGlobalItem : GlobalItem
    {
        private int burnTime = 0;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        // I hope this not multithreaded.
        private List<Point> entityEdgePoints = new List<Point>();

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (item.type == Mod.PlaceItemType<LivingHellstoneTile>())
                return;

            Collision.GetEntityEdgeTiles(entityEdgePoints, item);

            bool resetBurnTime = true;

            foreach (var p in entityEdgePoints)
            {
                if (p.X > 0 && p.X < Main.maxTilesX && p.Y > 0 && p.Y < Main.maxTilesY &&
                    Main.tile[p.X, p.Y].HasTile && Main.tile[p.X, p.Y].TileType == ModContent.TileType<LivingHellstoneTile>())
                {
                    if (++burnTime > 0)
                    {
                        Dust.NewDust(item.position, item.width, item.height, 6);
                    }
                    if (burnTime > 60)
                    {
                        item.active = false;
                    }

                    resetBurnTime = false;
                    break;
                }
            }

            entityEdgePoints.Clear();

            if (resetBurnTime)
                burnTime = 0;
        }
    }
}