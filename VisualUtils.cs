using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer
{
    public static class VisualUtils
    {
        private const float dustVelocity = 1.5f;

        private static MechTransfer mod = (MechTransfer)ModLoader.GetMod("MechTransfer");

        public static void UnwindVisuals(Dictionary<Point16, byte> visited, Point16 startPoint)
        {
            Point16 p = startPoint;

            while (visited.ContainsKey(p))
            {
                TransferUtils.Direction dir = (TransferUtils.Direction)visited[p];
                visited[p] = (byte)TransferUtils.Direction.stop; //Stops multiple particles, if multiple containers receive

                switch (dir)
                {
                    case TransferUtils.Direction.up: p = new Point16(p.X, p.Y - 1); break;
                    case TransferUtils.Direction.down: p = new Point16(p.X, p.Y + 1); break;
                    case TransferUtils.Direction.left: p = new Point16(p.X - 1, p.Y); break;
                    case TransferUtils.Direction.right: p = new Point16(p.X + 1, p.Y); break;
                    case TransferUtils.Direction.stop: return;
                }

                if (Main.netMode == 0)
                {
                    CreateVisual(p, dir);
                }
                else
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((byte)MechTransfer.ModMessageID.CreateDust);
                    packet.WritePackedVector2(p.ToVector2());
                    packet.Write((byte)dir);
                    packet.Send();
                }
            }
        }

        public static void CreateVisual(Point16 point, TransferUtils.Direction dir)
        {
            Vector2 location = new Vector2(point.X * 16 + 8, point.Y * 16 + 8);
            Vector2 velocity = Vector2.Zero;

            switch (dir)
            {
                case TransferUtils.Direction.up: velocity.Y = dustVelocity; break;
                case TransferUtils.Direction.down: velocity.Y = -dustVelocity; break;
                case TransferUtils.Direction.left: velocity.X = dustVelocity; break;
                case TransferUtils.Direction.right: velocity.X = -dustVelocity; break;
                case TransferUtils.Direction.stop: return;
            }

            Dust dust = Dust.NewDustPerfect(location, DustID.Silver, velocity);
            dust.noGravity = true;

            if (Main.xMas)
            {
                if (point.X % 2 == point.Y % 2)
                    dust.color = Color.Red;
                else
                    dust.color = Color.LightGreen;
            }

            if (Main.halloween)
            {
                if (point.X % 2 == point.Y % 2)
                    dust.color = Color.MediumPurple;
                else
                    dust.color = Color.Orange;
            }
        }

    }
}
