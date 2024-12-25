using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace MechTransfer.ContainerAdapters
{
    internal class SnowballLauncherAdapter
    {
        public void TakeItem(int x, int y, object slot, int amount)
        {
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            yield break;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            if (item.type != ItemID.Snowball)
                return false;

            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile)
                return false;

            int originX = x - tile.TileFrameX % 54 / 18;
            int originY = y - tile.TileFrameY % 54 / 18;

            if (!Wiring.CheckMech(originX, originY, 10))
                return false;

            SoundEngine.PlaySound(SoundID.Item11, new Vector2(x * 16, y * 16));

            float velocityX = Main.rand.Next(85, 105);
            float velocityY = Main.rand.Next(-35, 11);

            Vector2 position = new Vector2((float)((originX + 2) * 16 - 8), (float)((originY + 2) * 16 - 8));

            if (tile.TileFrameX / 54 == 0)
            {
                velocityX *= -1f;
                position.X -= 12f;
            }
            else
            {
                position.X += 12f;
            }

            float velocity = (12f + (float)Main.rand.Next(450) * 0.01f) / (float)Math.Sqrt((double)(velocityX * velocityX + velocityY * velocityY));
            velocityX *= velocity;
            velocityY *= velocity;

            Projectile.NewProjectile(null, position.X, position.Y, velocityX, velocityY, ProjectileID.SnowBallFriendly, 35, 3.5f, Main.myPlayer, 0, 0);

            item.stack--;
            return true;
        }
    }
}