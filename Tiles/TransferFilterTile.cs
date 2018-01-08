using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferFilterTile : FilterableTile, ITransferPassthrough
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TransferFilterTileEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200));

            hoverText = "Item allowed:";

            ((MechTransfer)mod).transferAgent.passthroughs.Add(Type, this);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
            {
                mod.GetTileEntity<TransferFilterTileEntity>().Kill(i, j);
                if (!noItem)
                {
                    if (Main.tile[i, j].frameY == 0)
                        Item.NewItem(i * 16, j * 16, 16, 16, mod.ItemType("TransferFilterItem"));
                    else
                        Item.NewItem(i * 16, j * 16, 16, 16, mod.ItemType("InverseTransferFilterItem"));
                }
            }
        }

        public bool ShouldPassthrough(TransferUtils agent, Point16 location, Item item)
        {
            int id = mod.GetTileEntity<TransferFilterTileEntity>().Find(location.X, location.Y);
            if (id == -1)
                return false;
            TransferFilterTileEntity entity = (TransferFilterTileEntity)TileEntity.ByID[id];

            if (Main.tile[location.X, location.Y].frameY == 0)
                return entity.ItemId == item.type;
            else
                return entity.ItemId != item.type;
        }
    }
}