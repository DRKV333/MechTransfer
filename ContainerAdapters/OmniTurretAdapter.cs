using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    internal class OmniTurretAdapter : INetHandler
    {
        private Mod mod;

        private int[] baseDamage = new int[] { 17, 25, 50 };
        private int[] fireRate = new int[] { 11, 7, 0 };
        private int[] shootSpeed = new int[] { 10, 10, 20 };

        public OmniTurretAdapter(Mod mod)
        {
            this.mod = mod;
            NetRouter.AddHandler(this);
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            yield break;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            if (item.ammo == 0 || item.shoot == 0 || item.ammo == AmmoID.Rocket)
                return false;

            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile)
                return false;

            int originX = x - (tile.TileFrameX % 36) / 18;
            int originY = y - (tile.TileFrameY % 36) / 18;

            Tile origin = Main.tile[originX, originY];
            if (origin == null || !origin.HasTile)
                return false;

            int style = origin.TileFrameX / 36;

            if (fireRate[style] != 0 && !Wiring.CheckMech(originX, originY, fireRate[style]))
                return false;

            Vector2 position = new Vector2((originX + 1) * 16, (originY + 1) * 16);

            Vector2 direction = Vector2.Zero;
            switch (origin.TileFrameY)
            {
                case 0: direction = new Vector2(-1f, 0f); break;
                case 38: direction = new Vector2(-0.5f, -0.5f); break;
                case 76: direction = new Vector2(0f, -1f); break;
                case 114: direction = new Vector2(0.5f, -0.5f); break;
                case 152: direction = new Vector2(1f, 0f); break;
                case 190: direction = new Vector2(0.5f, 0.5f); position.Y -= 8; break;
                case 228: direction = new Vector2(-0.5f, 0.5f); position.Y -= 8; break;
            }

            SoundEngine.PlaySound(SoundID.Item11, position);

            Projectile proj = Main.projectile[Projectile.NewProjectile(new MechTransferEntitySource(), position, direction * shootSpeed[style], item.shoot, baseDamage[style] * (1 + item.damage / 100), item.knockBack, Main.myPlayer)];
            proj.hostile = true;

            if (Main.netMode == 2)
            {
                ModPacket packet = NetRouter.GetPacketTo(this, mod);
                packet.Write((Int16)proj.identity);
                packet.Write((byte)proj.owner);
                packet.Send();
            }

            if (item.consumable)
                item.stack--;

            return true;
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            int identity = reader.ReadInt16();
            int owner = reader.ReadByte();
            for (int i = 0; i < 1000; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.owner == owner && proj.identity == identity && proj.active)
                {
                    proj.hostile = true;
                    break;
                }
            }
        }
    }
}