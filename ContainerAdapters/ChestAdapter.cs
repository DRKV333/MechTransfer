using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MechTransfer.ContainerAdapters
{
    internal class ChestAdapter : INetHandler
    {
        private Mod mod;

        public ChestAdapter(Mod mod)
        {
            this.mod = mod;
            NetRouter.AddHandler(this);
        }

        private int FindChest(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile)
                return -1;

            int originX = x;
            int originY = y;

            // TODO ???
            // if (TileLoader.IsDresser(tile.TileType))
            //     originX -= tile.TileFrameX % 54 / 18;
            // else
                originX -= tile.TileFrameX% 36 / 18;

            originY -= tile.TileFrameY % 36 / 18;

            if (!Chest.IsLocked(originX, originY))
                return Chest.FindChest(originX, originY);
            else
                return -1;
        }

        private void HandleChestItemChange(int chest)
        {
            int targetPlayer = WhichPlayerInChest(chest);
            if (targetPlayer != -1)
            {
                if (Main.netMode == 2)
                {
                    ModPacket packet = NetRouter.GetPacketTo(this, mod);
                    packet.Send(targetPlayer);
                    Main.player[targetPlayer].chest = -1;
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

            bool injectedPartial = false;

            if (item.maxStack > 1)
            {
                for (int i = 0; i < Main.chest[c].item.Length; i++)
                {
                    Item chestItem = Main.chest[c].item[i];
                    if (item.IsTheSameAs(chestItem) && chestItem.stack < chestItem.maxStack)
                    {
                        int spaceLeft = chestItem.maxStack - chestItem.stack;
                        if (spaceLeft >= item.stack)
                        {
                            chestItem.stack += item.stack;
                            item.stack = 0;
                            HandleChestItemChange(c);
                            return true;
                        }
                        else
                        {
                            item.stack -= spaceLeft;
                            chestItem.stack = chestItem.maxStack;
                            HandleChestItemChange(c);
                            injectedPartial = true;
                        }
                    }
                }
            }

            for (int i = 0; i < Main.chest[c].item.Length; i++)
            {
                if (Main.chest[c].item[i].IsAir)
                {
                    Main.chest[c].item[i] = item.Clone();
                    item.stack = 0;
                    HandleChestItemChange(c);
                    return true;
                }
            }

            return injectedPartial;
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

            Main.chest[c].item[(int)slot].stack -= amount;
            if (Main.chest[c].item[(int)slot].stack < 1)
                Main.chest[c].item[(int)slot] = new Item();

            HandleChestItemChange(c);
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            Main.LocalPlayer.chest = -1;
            Recipe.FindRecipes();

            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }
}