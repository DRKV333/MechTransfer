using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace MechTransfer.ContainerAdapters
{
    internal class CrystalStandAdapter
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
            if (item.type != ItemID.DD2ElderCrystal)
                return false;

            if (DD2Event.Ongoing || NPC.AnyNPCs(NPCID.DD2EterniaCrystal) || Main.pumpkinMoon || Main.snowMoon)
                return false;

            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active())
                return false;

            if (DD2Event.WouldFailSpawningHere(x, y))
            {
                DD2Event.FailureMessage(-1);
                return false;
            }
            else
            {
                DD2Event.SummonCrystal(x, y);
                return true;
            }
        }
    }
}