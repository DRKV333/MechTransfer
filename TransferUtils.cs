using MechTransfer.ContainerAdapters;
using MechTransfer.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MechTransfer
{
    public class TransferUtils
    {
        public enum Direction { up, down, left, right, stop }

        private Mod mod;
        private HashSet<Point16> TargetTriggered = new HashSet<Point16>(); //stops infinite recursion
        private int running = 0;

        internal Dictionary<int, ContainerAdapterDefinition> ContainerAdapters = new Dictionary<int, ContainerAdapterDefinition>();
        internal int unconditionalPassthroughType = 0;
        private Dictionary<int, ITransferTarget> targets = new Dictionary<int, ITransferTarget>();
        private Dictionary<int, ITransferPassthrough> passthroughs = new Dictionary<int, ITransferPassthrough>();

        public TransferUtils(MechTransfer mod)
        {
            this.mod = mod;
        }

        public void RegisterTarget(ITransferTarget target)
        {
            targets.Add(target.Type, target);
        }

        public void RegisterPassthrough(ITransferPassthrough passthrough)
        {
            passthroughs.Add(passthrough.Type, passthrough);
        }
        public static void EatWorldItem(int id, int eatNumber = 1)
        {
            EatItem(ref Main.item[id], eatNumber);

            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, id);
        }

        public static void EatItem(ref Item item, int eatNumber)
        {
            if (item.stack > eatNumber)
                item.stack -= eatNumber;
            else
                item = new Item();
        }

        public int StartTransfer(int startX, int startY, Item item)
        {
            running++;
            
            int olstack = item.stack;
            Item clone = item.Clone();
            SearchForTarget(startX, startY, clone);

            running--;

            if(running == 0)
            {
                TargetTriggered.Clear();
            }

            return olstack - clone.stack;
        }

        private void SearchForTarget(int startX, int startY, Item IToTransfer)
        {
            Queue<Point16> searchQ = new Queue<Point16>();
            Dictionary<Point16, byte> visited = new Dictionary<Point16, byte>();

            searchQ.Enqueue(new Point16(startX, startY));
            visited.Add(new Point16(startX, startY), (byte)Direction.stop);

            while (searchQ.Count > 0)
            {
                Point16 c = searchQ.Dequeue();

                for (int i = 0; i < 4; i++)
                {
                    Point16 searchP = new Point16();
                    Direction dir = 0;

                    switch (i)
                    {
                        case 0: searchP = new Point16(c.X + 1, c.Y); dir = Direction.left; break;
                        case 1: searchP = new Point16(c.X - 1, c.Y); dir = Direction.right; break;
                        case 2: searchP = new Point16(c.X, c.Y + 1); dir = Direction.up; break;
                        case 3: searchP = new Point16(c.X, c.Y - 1); dir = Direction.down; break;
                    }

                    if (visited.ContainsKey(searchP))
                        continue;
                    visited.Add(searchP, (byte)dir);// 00000VDD

                    Tile tile = Main.tile[searchP.X, searchP.Y];
                    if (tile == null || !tile.active())
                        continue;

                    //checking for targets
                    ITransferTarget target;
                    if(!TargetTriggered.Contains(searchP) && targets.TryGetValue(tile.type, out target))
                    {
                        TargetTriggered.Add(searchP);
                        if (target.Receive(this, searchP, IToTransfer))
                            VisualUtils.UnwindVisuals(visited, searchP);
                    }


                    if (IToTransfer.stack == 0)
                        return;

                    //checking for pipes
                    if (tile.type == unconditionalPassthroughType)
                    {
                        searchQ.Enqueue(searchP);
                    }
                    else
                    {
                        ITransferPassthrough passthrough;
                        if (passthroughs.TryGetValue(tile.type, out passthrough))
                        {
                            if (passthrough.ShouldPassthrough(this, searchP, IToTransfer))
                                searchQ.Enqueue(searchP);
                        }
                    }
                }
            }
        }

        //search order: Up, Down, Left, Right
        public IEnumerable<ContainerAdapter> FindContainerAdjacent(int x, int y)
        {
            ContainerAdapter c;

            c = FindContainer(x, y - 1);
            if (c != null)
                yield return c;
            c = FindContainer(x, y + 1);
            if (c != null)
                yield return c;
            c = FindContainer(x - 1, y);
            if (c != null)
                yield return c;
            c = FindContainer(x + 1, y);
            if (c != null)
                yield return c;
        }

        public ContainerAdapter FindContainer(int x, int y)
        {
            ContainerAdapterDefinition c;
            Tile tile = Main.tile[x, y];
            if (tile != null && tile.active() && ContainerAdapters.TryGetValue(tile.type, out c))
                return c.GetAdapter(x, y);
            return null;
        }

        public static string ItemNameById(int id)
        {
            if (id == 0)
                return "";

            LocalizedText name = Lang.GetItemName(id);
            if (name == LocalizedText.Empty)
                return string.Format("Unknown item #{0}", id);
            return name.Value;
        }
    }
}