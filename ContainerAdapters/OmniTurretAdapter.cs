﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace MechTransfer.ContainerAdapters
{
    class OmniTurretAdapter
    {
        private int[] baseDamage = new int[] { 17, 25 };
        private int[] fireRate = new int[] { 11, 7 };
        private int[] shootSpeed = new int[] { 10, 10 };

        public void TakeItem(int x, int y, object slot, int amount)
        {
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            yield break;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            if (!item.consumable || item.ammo == 0 || item.shoot == 0)
                return false;

            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active())
                return false;

            int originX = x - (tile.frameX % 36) / 18;
            int originY = y - (tile.frameY % 36) / 18;

            Tile origin = Main.tile[originX, originY];
            if (origin == null || !origin.active())
                return false;

            int style = origin.frameX / 36;
           
            if (fireRate[style] != 0 && !Wiring.CheckMech(originX, originY, fireRate[style]))
                return false;

            Vector2 position = new Vector2((originX + 1) * 16, (originY + 1) * 16);

            Vector2 direction = Vector2.Zero;
            switch (origin.frameY)
            {
                case 0: direction = new Vector2(-1f, 0f); break;
                case 36: direction = new Vector2(-0.5f, -0.5f); break;
                case 72: direction = new Vector2(0f, -1f); break;
                case 108: direction = new Vector2(0.5f, -0.5f); break;
                case 144: direction = new Vector2(1f, 0f); break;
                case 180: direction = new Vector2(0.5f, 0.5f); position.Y -= 8; break;
                case 216: direction = new Vector2(-0.5f, 0.5f); position.Y -= 8; break;
            }

            Main.projectile[Projectile.NewProjectile(position, direction * shootSpeed[style], item.shoot, baseDamage[style] * (1 + item.damage / 100), item.knockBack, Main.myPlayer)].hostile = true;

            return true;

        }
    }
}