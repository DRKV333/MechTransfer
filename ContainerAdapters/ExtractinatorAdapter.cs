using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    internal class ExtractinatorAdapter
    {
        public const int ChlorophyteExtractinatorID = TileID.ChlorophyteExtractinator; 

        // TODO: Check if this needs to be updated.
        public void Extract(int x, int y, int extractType, int extractinatorBlockType)
        {
            int amberRate = 5000;
            int gemRate = 25;
            int gemAmberRate = 50;
            int fossilRate = -1;
            int oldShoe = -1;
            int moss = -1;
            int coin = 1;
            switch (extractType)
            {
                /*
                case 1:
                */
                case ItemID.DesertFossil:
                    amberRate /= 3;
                    gemRate *= 2;
                    gemAmberRate = 20;
                    fossilRate = 10;
                    break;
                /*
                case 2:
                */
                case ItemID.OldShoe:
                    amberRate = -1;
                    gemRate = -1;
                    gemAmberRate = -1;
                    fossilRate = -1;
                    oldShoe = 1;
                    coin = -1;
                    break;
                /*
                case 3:
                */
                case ItemID.LavaMoss:
                    amberRate = -1;
                    gemRate = -1;
                    gemAmberRate = -1;
                    fossilRate = -1;
                    oldShoe = -1;
                    coin = -1;
                    moss = 1;
                    break;
            }

            int resultType;
            int resultStack = 1;
            if (fossilRate != -1 && Main.rand.NextBool(fossilRate))
            {
                resultType = 3380;
                if (Main.rand.NextBool(5))
                    resultStack += Main.rand.Next(2);

                if (Main.rand.NextBool(10))
                    resultStack += Main.rand.Next(3);

                if (Main.rand.NextBool(15))
                    resultStack += Main.rand.Next(4);
            }
            else if (coin != -1 && Main.rand.NextBool(2))
            {
                if (Main.rand.NextBool(12000))
                {
                    resultType = 74;
                    if (Main.rand.NextBool(14))
                        resultStack += Main.rand.Next(0, 2);

                    if (Main.rand.NextBool(14))
                        resultStack += Main.rand.Next(0, 2);

                    if (Main.rand.NextBool(14))
                        resultStack += Main.rand.Next(0, 2);
                }
                else if (Main.rand.NextBool(800))
                {
                    resultType = 73;
                    if (Main.rand.NextBool(6))
                        resultStack += Main.rand.Next(1, 21);

                    if (Main.rand.NextBool(6))
                        resultStack += Main.rand.Next(1, 21);

                    if (Main.rand.NextBool(6))
                        resultStack += Main.rand.Next(1, 21);

                    if (Main.rand.NextBool(6))
                        resultStack += Main.rand.Next(1, 21);

                    if (Main.rand.NextBool(6))
                        resultStack += Main.rand.Next(1, 20);
                }
                else if (Main.rand.NextBool(60))
                {
                    resultType = 72;
                    if (Main.rand.NextBool(4))
                        resultStack += Main.rand.Next(5, 26);

                    if (Main.rand.NextBool(4))
                        resultStack += Main.rand.Next(5, 26);

                    if (Main.rand.NextBool(4))
                        resultStack += Main.rand.Next(5, 26);

                    if (Main.rand.NextBool(4))
                        resultStack += Main.rand.Next(5, 25);
                }
                else
                {
                    resultType = 71;
                    if (Main.rand.NextBool(3))
                        resultStack += Main.rand.Next(10, 26);

                    if (Main.rand.NextBool(3))
                        resultStack += Main.rand.Next(10, 26);

                    if (Main.rand.NextBool(3))
                        resultStack += Main.rand.Next(10, 26);

                    if (Main.rand.NextBool(3))
                        resultStack += Main.rand.Next(10, 25);
                }
            }
            else if (amberRate != -1 && Main.rand.NextBool(amberRate))
            {
                resultType = 1242;
            }
            else if (oldShoe != -1)
            {
                resultType = ((!Main.rand.NextBool(4)) ? 2674 : ((!Main.rand.NextBool(3)) ? 2006 : ((Main.rand.NextBool(3)) ? 2675 : 2002)));
            }
            else if (moss != -1 && extractinatorBlockType == 642)
            {
                if (Main.rand.NextBool(10))
                {
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            resultType = 4354;
                            break;
                        case 1:
                            resultType = 4389;
                            break;
                        case 2:
                            resultType = 4377;
                            break;
                        case 3:
                            resultType = 5127;
                            break;
                        default:
                            resultType = 4378;
                            break;
                    }
                }
                else
                {
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            resultType = 4349;
                            break;
                        case 1:
                            resultType = 4350;
                            break;
                        case 2:
                            resultType = 4351;
                            break;
                        case 3:
                            resultType = 4352;
                            break;
                        default:
                            resultType = 4353;
                            break;
                    }
                }
            }
            else if (moss != -1)
            {
                switch (Main.rand.Next(5))
                {
                    case 0:
                        resultType = 4349;
                        break;
                    case 1:
                        resultType = 4350;
                        break;
                    case 2:
                        resultType = 4351;
                        break;
                    case 3:
                        resultType = 4352;
                        break;
                    default:
                        resultType = 4353;
                        break;
                }
            }
            else if (gemRate != -1 && Main.rand.NextBool(gemRate))
            {
                switch (Main.rand.Next(6))
                {
                    case 0:
                        resultType = 181;
                        break;
                    case 1:
                        resultType = 180;
                        break;
                    case 2:
                        resultType = 177;
                        break;
                    case 3:
                        resultType = 179;
                        break;
                    case 4:
                        resultType = 178;
                        break;
                    default:
                        resultType = 182;
                        break;
                }

                if (Main.rand.NextBool(20))
                    resultStack += Main.rand.Next(0, 2);

                if (Main.rand.NextBool(30))
                    resultStack += Main.rand.Next(0, 3);

                if (Main.rand.NextBool(40))
                    resultStack += Main.rand.Next(0, 4);

                if (Main.rand.NextBool(50))
                    resultStack += Main.rand.Next(0, 5);

                if (Main.rand.NextBool(60))
                    resultStack += Main.rand.Next(0, 6);
            }
            else if (gemAmberRate != -1 && Main.rand.NextBool(gemAmberRate))
            {
                resultType = 999;
                if (Main.rand.NextBool(20))
                    resultStack += Main.rand.Next(0, 2);

                if (Main.rand.NextBool(30))
                    resultStack += Main.rand.Next(0, 3);

                if (Main.rand.NextBool(40))
                    resultStack += Main.rand.Next(0, 4);

                if (Main.rand.NextBool(50))
                    resultStack += Main.rand.Next(0, 5);

                if (Main.rand.NextBool(60))
                    resultStack += Main.rand.Next(0, 6);
            }
            else if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool(5000))
                {
                    resultType = 74;
                    if (Main.rand.NextBool(10))
                        resultStack += Main.rand.Next(0, 3);

                    if (Main.rand.NextBool(10))
                        resultStack += Main.rand.Next(0, 3);

                    if (Main.rand.NextBool(10))
                        resultStack += Main.rand.Next(0, 3);

                    if (Main.rand.NextBool(10))
                        resultStack += Main.rand.Next(0, 3);

                    if (Main.rand.NextBool(10))
                        resultStack += Main.rand.Next(0, 3);
                }
                else if (Main.rand.NextBool(400))
                {
                    resultType = 73;
                    if (Main.rand.NextBool(5))
                        resultStack += Main.rand.Next(1, 21);

                    if (Main.rand.NextBool(5))
                        resultStack += Main.rand.Next(1, 21);

                    if (Main.rand.NextBool(5))
                        resultStack += Main.rand.Next(1, 21);

                    if (Main.rand.NextBool(5))
                        resultStack += Main.rand.Next(1, 21);

                    if (Main.rand.NextBool(5))
                        resultStack += Main.rand.Next(1, 20);
                }
                else if (Main.rand.NextBool(30))
                {
                    resultType = 72;
                    if (Main.rand.NextBool(3))
                        resultStack += Main.rand.Next(5, 26);

                    if (Main.rand.NextBool(3))
                        resultStack += Main.rand.Next(5, 26);

                    if (Main.rand.NextBool(3))
                        resultStack += Main.rand.Next(5, 26);

                    if (Main.rand.NextBool(3))
                        resultStack += Main.rand.Next(5, 25);
                }
                else
                {
                    resultType = 71;
                    if (Main.rand.NextBool(2))
                        resultStack += Main.rand.Next(10, 26);

                    if (Main.rand.NextBool(2))
                        resultStack += Main.rand.Next(10, 26);

                    if (Main.rand.NextBool(2))
                        resultStack += Main.rand.Next(10, 26);

                    if (Main.rand.NextBool(2))
                        resultStack += Main.rand.Next(10, 25);
                }
            }
            else if (extractinatorBlockType == 642)
            {
                switch (Main.rand.Next(14))
                {
                    case 0:
                        resultType = 12;
                        break;
                    case 1:
                        resultType = 11;
                        break;
                    case 2:
                        resultType = 14;
                        break;
                    case 3:
                        resultType = 13;
                        break;
                    case 4:
                        resultType = 699;
                        break;
                    case 5:
                        resultType = 700;
                        break;
                    case 6:
                        resultType = 701;
                        break;
                    case 7:
                        resultType = 702;
                        break;
                    case 8:
                        resultType = 364;
                        break;
                    case 9:
                        resultType = 1104;
                        break;
                    case 10:
                        resultType = 365;
                        break;
                    case 11:
                        resultType = 1105;
                        break;
                    case 12:
                        resultType = 366;
                        break;
                    default:
                        resultType = 1106;
                        break;
                }

                if (Main.rand.NextBool(20))
                    resultStack += Main.rand.Next(0, 2);

                if (Main.rand.NextBool(30))
                    resultStack += Main.rand.Next(0, 3);

                if (Main.rand.NextBool(40))
                    resultStack += Main.rand.Next(0, 4);

                if (Main.rand.NextBool(50))
                    resultStack += Main.rand.Next(0, 5);

                if (Main.rand.NextBool(60))
                    resultStack += Main.rand.Next(0, 6);
            }
            else
            {
                switch (Main.rand.Next(8))
                {
                    case 0:
                        resultType = 12;
                        break;
                    case 1:
                        resultType = 11;
                        break;
                    case 2:
                        resultType = 14;
                        break;
                    case 3:
                        resultType = 13;
                        break;
                    case 4:
                        resultType = 699;
                        break;
                    case 5:
                        resultType = 700;
                        break;
                    case 6:
                        resultType = 701;
                        break;
                    default:
                        resultType = 702;
                        break;
                }

                if (Main.rand.NextBool(20))
                    resultStack += Main.rand.Next(0, 2);

                if (Main.rand.NextBool(30))
                    resultStack += Main.rand.Next(0, 3);

                if (Main.rand.NextBool(40))
                    resultStack += Main.rand.Next(0, 4);

                if (Main.rand.NextBool(50))
                    resultStack += Main.rand.Next(0, 5);

                if (Main.rand.NextBool(60))
                    resultStack += Main.rand.Next(0, 6);
            }

            int tempTargetX = Player.tileTargetX;
            Player.tileTargetX = x;
            int tempTargetY = Player.tileTargetY;
            Player.tileTargetY = y;

            ItemLoader.ExtractinatorUse(ref resultType, ref resultStack, extractType, extractinatorBlockType);
            if (resultType > 0)
            {
                Item.NewItem(new MechTransferEntitySource(), x * 16, y * 16, 1, 1, resultType, resultStack, false, -1, false, false);
            }

            Player.tileTargetX = tempTargetX;
            Player.tileTargetY = tempTargetY;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            int extType = ItemID.Sets.ExtractinatorMode[item.type];
            int extTileType = Main.tile[x, y].TileType;
            bool isChlExtractinator = extTileType == ChlorophyteExtractinatorID;
            if (isChlExtractinator && 
                ItemTrader.ChlorophyteExtractinator.TryGetTradeOption(item, out var option))
            {
                item.stack -= option.TakingItemStack;
                if (item.stack <= 0)
                    item.TurnToAir();

                int itemType = option.GivingITemType;
                int stack = option.GivingItemStack;
                Item.NewItem(new MechTransferEntitySource(), x * 16, y * 16, 1, 1, itemType, stack, noBroadcast: false, -1, false, false);
                return true;
            }
            else if (extType >= 0)
            {
                Extract(x, y, extType, extTileType);
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