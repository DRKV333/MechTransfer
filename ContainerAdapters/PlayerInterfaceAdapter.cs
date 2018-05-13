using MechTransfer.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MechTransfer.ContainerAdapters
{
    internal class PlayerInterfaceAdapter : INetHandler
    {
        private enum SlotKind { Inventory, Armor, Dye, MiscEquip, MiscDye }

        private class PlayerSlot
        {
            public SlotKind kind;
            public int slot;
            public int owner;
        }

        private Mod mod;

        public PlayerInterfaceAdapter(Mod mod)
        {
            this.mod = mod;
            NetRouter.AddHandler(this);
        }

        private int FindInside(int x, int y)
        {
            int Left = (x * 16) + 8;
            int Bottom = (y + 1) * 16;

            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                Rectangle hitbox = player.Hitbox;
                if (player.active)
                {
                    if (Math.Abs(hitbox.Left - Left) < 8 && Math.Abs(hitbox.Bottom - Bottom) < 8)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
        {
            Point16 origin = mod.GetTile<PlayerInterfaceTile>().GetOrigin(x, y);
            int p = FindInside(origin.X, origin.Y);

            if (p == -1)
                yield break;

            IEnumerable<Tuple<Item, object>>[] inventories = new IEnumerable<Tuple<Item, object>>[]
            {
                EnumerateRange(p, SlotKind.Inventory, 0, 58),
                EnumerateRange(p, SlotKind.Armor, 0, 8 + Main.player[p].extraAccessorySlots),
                EnumerateRange(p, SlotKind.Armor, 10, 8 + Main.player[p].extraAccessorySlots),
                EnumerateRange(p, SlotKind.Dye, 0, 8 + Main.player[p].extraAccessorySlots),
                EnumerateRange(p, SlotKind.MiscDye),
                EnumerateRange(p, SlotKind.MiscEquip)
            };

            foreach (var item in inventories)
            {
                foreach (var item2 in item)
                {
                    yield return item2;
                }
            }

        }

        private IEnumerable<Tuple<Item,object>> EnumerateRange(int player, SlotKind kind, int from = 0, int length = -1)
        {           
            Item[] items = GetInventoryFromKind(player, kind);

            if (length == -1)
                length = items.Length - from;

            for (int i = from; i < length + from; i++)
            {
                if (!items[i].favorited)
                    yield return new Tuple<Item, object>(items[i], new PlayerSlot() { kind = kind, owner = player, slot = i });
            }
        }

        public void TakeItem(int x, int y, object slot, int amount)
        {
            PlayerSlot playerSlot = (PlayerSlot)slot;

            Player player = Main.player[playerSlot.owner];
            Item[] targetInventory = GetInventoryFromKind(playerSlot.owner, playerSlot.kind);

            targetInventory[playerSlot.slot].stack -= amount;
            if (targetInventory[playerSlot.slot].stack < 1)
                targetInventory[playerSlot.slot] = new Item();

            HandleSlotChange(playerSlot.owner, playerSlot.kind, playerSlot.slot);
        }

        //I should be handling achievements here, like in ItemSlot.SwapEquip, but meh... :/
        public bool InjectItem(int x, int y, Item item)
        {
            Point16 origin = mod.GetTile<PlayerInterfaceTile>().GetOrigin(x, y);
            int p = FindInside(origin.X, origin.Y);

            if (p == -1)
                return false;

            int oldStack = item.stack;

            if(item.dye > 0)
            {
                InjectToRange(p, SlotKind.Dye, item, false, 0, 8 + Main.player[p].extraAccessorySlots);
                if (item.stack > 0)
                    InjectToRange(p, SlotKind.MiscDye, item, false);
            }
            else if(Main.projHook[item.shoot])
            {
                InjectToSpecificSlot(p, SlotKind.MiscEquip, 4, item);
            }
            else if(item.mountType != -1)
            {
                if (MountID.Sets.Cart[item.mountType])
                    InjectToSpecificSlot(p, SlotKind.MiscEquip, 2, item);
                else
                    InjectToSpecificSlot(p, SlotKind.MiscEquip, 3, item);
            }
            else if(item.buffType > 0)
            {
                if(Main.lightPet[item.buffType])
                    InjectToSpecificSlot(p, SlotKind.MiscEquip, 1, item);
                else if(Main.vanityPet[item.buffType])
                    InjectToSpecificSlot(p, SlotKind.MiscEquip, 0, item);
            }
            else if(item.headSlot != -1)
            {
                InjectArmor(p, item, 0, 10);
            }
            else if (item.bodySlot != -1)
            {
                InjectArmor(p, item, 1, 11);
            }
            else if (item.legSlot != -1)
            {
                InjectArmor(p, item, 2, 12);
            }
            else if(item.accessory)
            {
                InjectAccessory(p, item);
            }
            else if(MoneyValue(item.type) > 0)
            {
                //I should probably compact coins here
                InjectToRange(p, SlotKind.Inventory, item, true, 50, 4);
            }
            else if(item.ammo != 0)
            {
                InjectToRange(p, SlotKind.Inventory, item, true, 54, 4);
            }

            if(item.stack > 0)
                InjectToRange(p, SlotKind.Inventory, item, true, 0, 50);

            return item.stack < oldStack;
        }

        //Spiced up version of Chest adapter inject
        private void InjectToRange(int player, SlotKind targetKind, Item item, bool stackable = true, int from = 0, int length = -1)
        {
            Item[] inventory = GetInventoryFromKind(player, targetKind);

            if (length == -1)
                length = inventory.Length - from;

            if (item.maxStack > 1 && stackable)
            {
                for (int i = from; i < length + from; i++)
                {
                    Item invItem = inventory[i];
                    if (item.IsTheSameAs(invItem) && invItem.stack < invItem.maxStack)
                    {
                        int spaceLeft = invItem.maxStack - invItem.stack;
                        if (spaceLeft >= item.stack)
                        {
                            invItem.stack += item.stack;
                            item.stack = 0;
                            HandleSlotChange(player, targetKind, i);
                            return;
                        }
                        else
                        {
                            item.stack -= spaceLeft;
                            invItem.stack = invItem.maxStack;
                            HandleSlotChange(player, targetKind, i);
                        }
                    }
                }
            }

            for (int i = from; i < length + from; i++)
            {
                if (inventory[i].IsAir)
                {
                    inventory[i] = item.Clone();
                    if (stackable)
                    {
                        item.stack = 0;
                        HandleSlotChange(player, targetKind, i);
                        return;
                    }
                    else
                    {
                        inventory[i].stack = 1;
                        item.stack--;
                        HandleSlotChange(player, targetKind, i);
                        if (item.stack < 1)
                            return;
                    }
                }
            }
        }

        private void InjectToSpecificSlot(int player, SlotKind targetKind, int slot, Item item)
        {
            Item[] targetInventory = GetInventoryFromKind(player, targetKind);
            if (targetInventory[slot].IsAir)
            {
                targetInventory[slot] = item.Clone();
                targetInventory[slot].stack = 1;
                item.stack--;
                HandleSlotChange(player, targetKind, slot);
            }
        }

        private void InjectArmor(int player, Item item, int regular, int vanity)
        {
            int opt1;
            int opt2;

            if (item.vanity)
            {
                opt1 = vanity;
                opt2 = regular;
            }
            else
            {
                opt1 = regular;
                opt2 = vanity;
            }

            InjectToSpecificSlot(player, SlotKind.Armor, opt1, item);
            if (item.stack > 0)
                InjectToSpecificSlot(player, SlotKind.Armor, opt2, item);
        }

        private void InjectAccessory(int player, Item item)
        {
            Item[] inventory = Main.player[player].armor;

            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].IsTheSameAs(item))
                    return;
                if (i < 10 && item.wingSlot > 0 && inventory[i].wingSlot > 0)
                    return;
            }

            int start1;
            int start2;

            if (item.vanity)
            {
                start1 = 13;
                start2 = 3;
            }
            else
            {
                start1 = 3;
                start2 = 13;
            }

            int len = 5 + Main.player[player].extraAccessorySlots;

            InjectToRange(player, SlotKind.Armor, item, false, start1, len);
            if(item.stack > 0)
                InjectToRange(player, SlotKind.Armor, item, false, start2, len);

        }

        private int MoneyValue(int type)
        {
            switch (type)
            {
                case ItemID.CopperCoin: return 1;
                case ItemID.SilverCoin: return 100;
                case ItemID.GoldCoin: return 10000;
                case ItemID.PlatinumCoin: return 1000000;
                default: return 0;
            }
        }

        private Item[] GetInventoryFromKind(int player, SlotKind kind)
        {
            Player p = Main.player[player];
            switch (kind)
            {
                case SlotKind.Inventory: return p.inventory;
                case SlotKind.Armor: return p.armor;
                case SlotKind.Dye: return p.dye;
                case SlotKind.MiscEquip: return p.miscEquips;
                case SlotKind.MiscDye: return p.miscDyes;
                default: return null;
            }
        }

        private void HandleSlotChange(int owner, SlotKind kind, int slot)
        {
            if (Main.netMode == 2)
            {
                ModPacket packet = NetRouter.GetPacketTo(this, mod);
                packet.Write(owner);
                packet.Write((byte)kind);
                packet.Write(slot);

                Item[] targetInventory = GetInventoryFromKind(owner, kind);
                ItemIO.Send(targetInventory[slot], packet, true, true);

                packet.Send();
            }
            else if (Main.netMode == 0)
            {
                Recipe.FindRecipes();
            }
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            int owner = reader.ReadInt32();
            SlotKind kind = (SlotKind)reader.ReadByte();
            int slot = reader.ReadInt32();

            Item[] targetInventory = GetInventoryFromKind(owner, kind);
            ItemIO.Receive(targetInventory[slot], reader, true, true);
        }
    }
}
