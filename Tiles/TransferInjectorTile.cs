using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferInjectorTile : ModTile, ITransferTarget
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);

            drop = mod.ItemType("TransferInjectorItem");
            AddMapEntry(new Color(200, 200, 200));

            ((MechTransfer)mod).transferAgent.RegisterTarget(this);
        }

        public bool Receive(TransferUtils agent, Point16 location, Item item)
        {
            bool success = false;

            foreach (var container in agent.FindContainerAdjacent(location.X, location.Y))
            {
                if (container.InjectItem(item))
                    success = true;

                if (item.stack < 1)
                    break;
            }

            if(success)
                mod.GetModWorld<MechTransferWorld>().TripWireDelayed(location.X, location.Y, 1, 1);

            return success;
        }
    }
}