using System;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace MechTransfer.Tiles.Simple
{
    public abstract class SimpleTETile<TEntity> : SimpleTileObject where TEntity : SimpleTileEntity
    {
        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEntity>().Hook_AfterPlacement, -1, 0, false);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (OneByOne && !effectOnly)
                mod.GetTileEntity<TEntity>().Kill(i, j);
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Point16 origin = GetOrigin(i, j, frameX, frameY);
            mod.GetTileEntity<TEntity>().Kill(origin.X, origin.Y);
            base.KillMultiTile(i, j, frameX, frameY);
        }

        public TEntity GetEntity(int x, int y)
        {
            TileEntity TE;
            if (!TileEntity.ByPosition.TryGetValue(GetOrigin(x, y), out TE))
            {
                throw new ArgumentException("No tile entity at location");
            }
            return (TEntity)TE;
        }

        public bool TryGetEntity(int x, int y, out TEntity entity)
        {
            int id = mod.GetTileEntity<TEntity>().Find(x, y);
            if (id == -1)
            {
                entity = null;
                return false;
            }
            else
            {
                entity = (TEntity)TileEntity.ByID[id];
                return true;
            }
        }
    }
}