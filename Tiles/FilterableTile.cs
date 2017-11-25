using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MechTransfer.Tiles
{
    public abstract class FilterableTile : ModTile
    {
        protected string hoverText = "Filter:";

        public override void RightClick(int i, int j)
        {
            if (!Main.LocalPlayer.HeldItem.IsAir)
            {
                TileEntity tileEntity;
                if(TileEntity.ByPosition.TryGetValue(new Point16(i,j), out tileEntity))
                {
                    TransferFilterTileEntity transferFilter = tileEntity as TransferFilterTileEntity;
                    if(transferFilter != null)
                    {
                        transferFilter.ItemId = Main.LocalPlayer.HeldItem.type;
                        transferFilter.SyncData();
                    }
                }
            }
        }

        public override void MouseOverFar(int i, int j)
        {
            DisplayTooltip(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            DisplayTooltip(i, j);
            Main.LocalPlayer.noThrow = 2;
        }

        public virtual void DisplayTooltip(int i, int j)
        {
            int id = mod.GetTileEntity<TransferFilterTileEntity>().Find(i, j);
            if (id == -1)
                return;
            TransferFilterTileEntity entity = (TransferFilterTileEntity)TileEntity.ByID[id];

            ((MechTransfer)mod).filterHoverUI.Display(entity.ItemId, hoverText, Color.White);
        }
    }
}
