﻿using MechTransfer.Tiles.Simple;
using Microsoft.Xna.Framework;
using Terraria;

namespace MechTransfer.Tiles
{
    public abstract class FilterableTile<T> : SimpleTETile<T> where T : TransferFilterTileEntity
    {
        public override void RightClick(int i, int j)
        {
            if (!Main.LocalPlayer.HeldItem.IsAir)
            {
                T tileEntity;
                if (TryGetEntity(i, j, out tileEntity))
                {
                    tileEntity.item = Main.LocalPlayer.HeldItem.Clone();
                    tileEntity.item.stack = 1;
                    tileEntity.SyncData();
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
            T entity;
            if (TryGetEntity(i, j, out entity))
                ((MechTransfer)mod).filterHoverUI.Display(entity.item, HoverText(entity), HoverColor(entity));
        }

        public virtual string HoverText(T entity)
        {
            return "Filter:";
        }

        public virtual Color HoverColor(T entity)
        {
            return Color.White;
        }
    }
}