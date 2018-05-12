using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MechTransfer
{
    static class NetRouter
    {
        private class TypeSpecificComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return x.GetType().FullName == y.GetType().FullName;
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        private static byte prefix = 0;
        private static bool bigIds = false;
        private static bool inited = false;
        private static INetHandler[] handlers;
        private static Dictionary<INetHandler, int> handlersInverse;
        private static List<INetHandler> loadList = new List<INetHandler>();

        public static void AddHandler(INetHandler handler)
        {
            if (inited)
                throw new InvalidOperationException();

            loadList.Add(handler);
        }

        public static void Init(byte prefix)
        {
            if (inited)
                throw new InvalidOperationException();

            NetRouter.prefix = prefix;
            handlers = loadList.OrderBy(x => x.GetType().FullName).Distinct(new TypeSpecificComparer<INetHandler>()).ToArray();
            handlersInverse = new Dictionary<INetHandler, int>();
            for (int i = 0; i < handlers.Length; i++)
            {
                handlersInverse.Add(handlers[i], i);
            }
            loadList = null;
            inited = true;
            if (handlers.Length > 256)
                bigIds = true;
        }

        public static void RouteMessage(BinaryReader reader, int WhoAmI)
        {
            if (!inited)
                throw new InvalidOperationException();

            int target;
            if (bigIds)
                target = reader.ReadUInt16();
            else
                target = reader.ReadByte();

            handlers[target].HandlePacket(reader, WhoAmI);
        }

        public static ModPacket GetPacketTo(INetHandler target, Mod mod)
        {
            int id;
            if (!handlersInverse.TryGetValue(target, out id))
                throw new ArgumentException();

            ModPacket packet = mod.GetPacket();

            if (prefix != 0)
                packet.Write(prefix);

            if (bigIds)
                packet.Write((UInt16)id);
            else
                packet.Write((byte)id);

            return packet;
        }
    }
}
