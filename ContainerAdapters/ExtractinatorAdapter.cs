using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    internal class ExtractinatorAdapter
    {
        public void Extract(int x, int y, int extractType)
        {
            int num = 5000;
            int num2 = 25;
            int num3 = 50;
            int num4 = -1;
            if (extractType == 3347)
            {
                num /= 3;
                num2 *= 2;
                num3 /= 2;
                num4 = 10;
            }
            int num5 = 1;
            int num6 = 0;
            if (num4 != -1 && Main.rand.Next(num4) == 0)
            {
                num6 = 3380;
                if (Main.rand.Next(5) == 0)
                {
                    num5 += Main.rand.Next(2);
                }
                if (Main.rand.Next(10) == 0)
                {
                    num5 += Main.rand.Next(3);
                }
                if (Main.rand.Next(15) == 0)
                {
                    num5 += Main.rand.Next(4);
                }
            }
            else if (Main.rand.Next(2) == 0)
            {
                if (Main.rand.Next(12000) == 0)
                {
                    num6 = 74;
                    if (Main.rand.Next(14) == 0)
                    {
                        num5 += Main.rand.Next(0, 2);
                    }
                    if (Main.rand.Next(14) == 0)
                    {
                        num5 += Main.rand.Next(0, 2);
                    }
                    if (Main.rand.Next(14) == 0)
                    {
                        num5 += Main.rand.Next(0, 2);
                    }
                }
                else if (Main.rand.Next(800) == 0)
                {
                    num6 = 73;
                    if (Main.rand.Next(6) == 0)
                    {
                        num5 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.Next(6) == 0)
                    {
                        num5 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.Next(6) == 0)
                    {
                        num5 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.Next(6) == 0)
                    {
                        num5 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.Next(6) == 0)
                    {
                        num5 += Main.rand.Next(1, 20);
                    }
                }
                else if (Main.rand.Next(60) == 0)
                {
                    num6 = 72;
                    if (Main.rand.Next(4) == 0)
                    {
                        num5 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.Next(4) == 0)
                    {
                        num5 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.Next(4) == 0)
                    {
                        num5 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.Next(4) == 0)
                    {
                        num5 += Main.rand.Next(5, 25);
                    }
                }
                else
                {
                    num6 = 71;
                    if (Main.rand.Next(3) == 0)
                    {
                        num5 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.Next(3) == 0)
                    {
                        num5 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.Next(3) == 0)
                    {
                        num5 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.Next(3) == 0)
                    {
                        num5 += Main.rand.Next(10, 25);
                    }
                }
            }
            else if (num != -1 && Main.rand.Next(num) == 0)
            {
                num6 = 1242;
            }
            else if (num2 != -1 && Main.rand.Next(num2) == 0)
            {
                num6 = Main.rand.Next(6);
                if (num6 == 0)
                {
                    num6 = 181;
                }
                else if (num6 == 1)
                {
                    num6 = 180;
                }
                else if (num6 == 2)
                {
                    num6 = 177;
                }
                else if (num6 == 3)
                {
                    num6 = 179;
                }
                else if (num6 == 4)
                {
                    num6 = 178;
                }
                else
                {
                    num6 = 182;
                }
                if (Main.rand.Next(20) == 0)
                {
                    num5 += Main.rand.Next(0, 2);
                }
                if (Main.rand.Next(30) == 0)
                {
                    num5 += Main.rand.Next(0, 3);
                }
                if (Main.rand.Next(40) == 0)
                {
                    num5 += Main.rand.Next(0, 4);
                }
                if (Main.rand.Next(50) == 0)
                {
                    num5 += Main.rand.Next(0, 5);
                }
                if (Main.rand.Next(60) == 0)
                {
                    num5 += Main.rand.Next(0, 6);
                }
            }
            else if (num3 != -1 && Main.rand.Next(num3) == 0)
            {
                num6 = 999;
                if (Main.rand.Next(20) == 0)
                {
                    num5 += Main.rand.Next(0, 2);
                }
                if (Main.rand.Next(30) == 0)
                {
                    num5 += Main.rand.Next(0, 3);
                }
                if (Main.rand.Next(40) == 0)
                {
                    num5 += Main.rand.Next(0, 4);
                }
                if (Main.rand.Next(50) == 0)
                {
                    num5 += Main.rand.Next(0, 5);
                }
                if (Main.rand.Next(60) == 0)
                {
                    num5 += Main.rand.Next(0, 6);
                }
            }
            else if (Main.rand.Next(3) == 0)
            {
                if (Main.rand.Next(5000) == 0)
                {
                    num6 = 74;
                    if (Main.rand.Next(10) == 0)
                    {
                        num5 += Main.rand.Next(0, 3);
                    }
                    if (Main.rand.Next(10) == 0)
                    {
                        num5 += Main.rand.Next(0, 3);
                    }
                    if (Main.rand.Next(10) == 0)
                    {
                        num5 += Main.rand.Next(0, 3);
                    }
                    if (Main.rand.Next(10) == 0)
                    {
                        num5 += Main.rand.Next(0, 3);
                    }
                    if (Main.rand.Next(10) == 0)
                    {
                        num5 += Main.rand.Next(0, 3);
                    }
                }
                else if (Main.rand.Next(400) == 0)
                {
                    num6 = 73;
                    if (Main.rand.Next(5) == 0)
                    {
                        num5 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.Next(5) == 0)
                    {
                        num5 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.Next(5) == 0)
                    {
                        num5 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.Next(5) == 0)
                    {
                        num5 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.Next(5) == 0)
                    {
                        num5 += Main.rand.Next(1, 20);
                    }
                }
                else if (Main.rand.Next(30) == 0)
                {
                    num6 = 72;
                    if (Main.rand.Next(3) == 0)
                    {
                        num5 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.Next(3) == 0)
                    {
                        num5 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.Next(3) == 0)
                    {
                        num5 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.Next(3) == 0)
                    {
                        num5 += Main.rand.Next(5, 25);
                    }
                }
                else
                {
                    num6 = 71;
                    if (Main.rand.Next(2) == 0)
                    {
                        num5 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        num5 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        num5 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        num5 += Main.rand.Next(10, 25);
                    }
                }
            }
            else
            {
                num6 = Main.rand.Next(8);
                if (num6 == 0)
                {
                    num6 = 12;
                }
                else if (num6 == 1)
                {
                    num6 = 11;
                }
                else if (num6 == 2)
                {
                    num6 = 14;
                }
                else if (num6 == 3)
                {
                    num6 = 13;
                }
                else if (num6 == 4)
                {
                    num6 = 699;
                }
                else if (num6 == 5)
                {
                    num6 = 700;
                }
                else if (num6 == 6)
                {
                    num6 = 701;
                }
                else
                {
                    num6 = 702;
                }
                if (Main.rand.Next(20) == 0)
                {
                    num5 += Main.rand.Next(0, 2);
                }
                if (Main.rand.Next(30) == 0)
                {
                    num5 += Main.rand.Next(0, 3);
                }
                if (Main.rand.Next(40) == 0)
                {
                    num5 += Main.rand.Next(0, 4);
                }
                if (Main.rand.Next(50) == 0)
                {
                    num5 += Main.rand.Next(0, 5);
                }
                if (Main.rand.Next(60) == 0)
                {
                    num5 += Main.rand.Next(0, 6);
                }
            }

            int tempTargetX = Player.tileTargetX;
            Player.tileTargetX = x;

            int tempTargetY = Player.tileTargetY;
            Player.tileTargetY = y;

            ItemLoader.ExtractinatorUse(ref num6, ref num5, extractType);
            if (num6 > 0)
            {
                Item.NewItem(x * 16, y * 16, 1, 1, num6, num5, false, -1, false, false);
            }

            Player.tileTargetX = tempTargetX;
            Player.tileTargetY = tempTargetY;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            int extType = ItemID.Sets.ExtractinatorMode[item.type];
            if (extType >= 0)
            {
                Extract(x, y, extType);
                item.stack -= 1;
                return true;
            }
            return false;
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            yield break;
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
        }
    }
}