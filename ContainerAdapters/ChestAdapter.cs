using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace MechTransfer.ContainerAdapters
{
    internal class ChestAdapter
    {
        private int FindChest(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active())
                return -1;

            int originX = x;
            int originY = y;
            if (tile.frameX % 36 != 0)
                originX--;
            if (tile.frameY != 0)
                originY--;

            if (!Chest.isLocked(originX, originY))
                return Chest.FindChest(originX, originY);
            else
                return -1;
        }

        private void HandleChestItemChange(int chest, int slot)
        {
            int targetPlayer = WhichPlayerInChest(chest);
            if (targetPlayer != -1)
            {
                if (Main.netMode == 2)
                {
                    Item item = Main.chest[chest].item[slot];
                    NetMessage.SendData(MessageID.SyncChestItem, targetPlayer, -1, null, chest, slot, item.stack, item.prefix, item.type);
                }
                else if (Main.netMode == 0)
                    Recipe.FindRecipes();
            }
        }

        public static int WhichPlayerInChest(int i)
        {
            for (int j = 0; j < 255; j++)
            {
                if (Main.player[j].chest == i)
                {
                    return j;
                }
            }
            return -1;
        }

        public bool InjectItem(int x, int y, Item item)
        {
            int c = FindChest(x, y);

            if (c == -1)
                return false;

            if (item.maxStack > 1)
            {
                for (int i = 0; i < Main.chest[c].item.Length; i++)
                {
                    Item chestItem = Main.chest[c].item[i];
                    if (item.IsTheSameAs(chestItem) && chestItem.stack < chestItem.maxStack)
                    {
                        chestItem.stack++;
                        HandleChestItemChange(c, i);
                        return true;
                    }
                }
            }

            for (int i = 0; i < Main.chest[c].item.Length; i++)
            {
                if (Main.chest[c].item[i].IsAir)
                {
                    Main.chest[c].item[i] = item;
                    HandleChestItemChange(c, i);
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            int c = FindChest(x, y);

            if (c == -1)
                yield break;

            for (int i = 0; i < Main.chest[c].item.Length; i++)
            {
                yield return new Tuple<Item, object>(Main.chest[c].item[i], i);
            }
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
            int c = FindChest(x, y);

            if (c == -1)
                return;

            TransferUtils.EatItem(ref Main.chest[c].item[(int)slot], amount);
            HandleChestItemChange(c, (int)slot);
        }
    }
}