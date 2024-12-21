using MechTransfer.ContainerAdapters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MechTransfer
{
    public class TransferAgent : ModSystem, INetHandler
    {
        private class delayedTrip
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }

        public enum Direction { up, down, left, right, stop }

        private HashSet<Point16> TargetTriggered = new HashSet<Point16>(); //stops infinite recursion
        private int running = 0;
        private List<delayedTrip> wireTriggerQ = new List<delayedTrip>();

        public Dictionary<int, ContainerAdapterDefinition> ContainerAdapters = new Dictionary<int, ContainerAdapterDefinition>();
        public int unconditionalPassthroughType = 0;
        public Dictionary<int, ITransferTarget> targets = new Dictionary<int, ITransferTarget>();
        public Dictionary<int, ITransferPassthrough> passthroughs = new Dictionary<int, ITransferPassthrough>();

        public TransferAgent() : base()
        {
            NetRouter.AddHandler(this);
        }

        public int StartTransfer(int startX, int startY, Item item)
        {
            running++;

            int olstack = item.stack;
            Item clone = item.Clone();

            try
            {
                SearchForTarget(startX, startY, clone);
            }
            catch (Exception e)
            {
                Main.NewText("An exception has occurred at MechTransfer.TransferUtils.SearchForTarget (Please look at the log in Documents/My Games/Terraria/ModLoader/Logs)", Color.Red);
                Mod.Logger.Error("\nMechTransfer has logged an exception:");
                Mod.Logger.Error(e.ToString());
                Mod.Logger.Error("Please report it on the MechTransfer forum page.\n");
            }
            finally
            {
                running--;
                if (running == 0)
                {
                    TargetTriggered.Clear();
                }
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
                    if (tile == null || !tile.HasTile)
                        continue;

                    //checking for targets
                    ITransferTarget target;
                    if (!TargetTriggered.Contains(searchP) && targets.TryGetValue(tile.TileType, out target))
                    {
                        TargetTriggered.Add(searchP);
                        if (target.Receive(searchP, IToTransfer))
                            VisualUtils.UnwindVisuals(visited, searchP);
                    }

                    if (IToTransfer.stack == 0)
                        return;

                    //checking for pipes
                    if (tile.TileType == unconditionalPassthroughType)
                    {
                        searchQ.Enqueue(searchP);
                    }
                    else
                    {
                        ITransferPassthrough passthrough;
                        if (passthroughs.TryGetValue(tile.TileType, out passthrough))
                        {
                            if (passthrough.ShouldPassthrough(searchP, IToTransfer))
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
            if (tile != null && tile.HasTile && ContainerAdapters.TryGetValue(tile.TileType, out c))
                return c.GetAdapter(x, y);
            return null;
        }

        public bool IsContainer(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return (tile != null && tile.HasTile && ContainerAdapters.ContainsKey(tile.TileType));
        }

        //trigger wire on the new update, to stop infinite wire loops
        public void TripWireDelayed(int x, int y, int width, int height)
        {
            if (wireTriggerQ.Any(i => i.x == x && i.y == y))
                return;

            delayedTrip trip = new delayedTrip();
            trip.x = x;
            trip.y = y;
            trip.width = width;
            trip.height = height;
            wireTriggerQ.Add(trip);
        }

        public override void PostUpdateWorld()
        {
            List<delayedTrip> temp = wireTriggerQ;
            wireTriggerQ = new List<delayedTrip>();

            foreach (var item in temp)
            {
                Wiring.TripWire(item.x, item.y, item.width, item.height);
            }
            temp.Clear();
        }

        public void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            if (Main.netMode != 1)
                return;

            VisualUtils.CreateVisual(reader.ReadPackedVector2().ToPoint16(), (Direction)reader.ReadByte());
        }
    }
}