using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles.Simple
{
    public abstract class SimpleTileObject : SimpleTile
    {
        //Actually, It's not simple at all...

        protected ModItem[] PlaceItems = new ModItem[1];

        public TileObjectData tileObjectData { get; protected set; }
        protected bool simpleHeigth;
        protected bool OneByOne;

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            SetTileObjectData();
            TileObjectData.addTile(Type);

            tileObjectData = TileObjectData.GetTileData(Type, 0);

            if (tileObjectData.Width == 1 && tileObjectData.Height == 1)
                OneByOne = true;

            if (tileObjectData.CoordinateHeights.Distinct().Count() == 1)
                simpleHeigth = true;
        }

        public ModItem GetPlaceItem(int kind)
        {
            return PlaceItems[kind];
        }

        public int GetStyle(Tile tile)
        {
            return GetDropKind(tile.frameX, tile.frameY);
        }

        public virtual int GetDropKind(int Fx, int Fy)
        {
            return 0;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            DropItem(i, j, frameX, frameY);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (OneByOne && !effectOnly && !noItem)
            {
                Tile tile = Main.tile[i, j];
                DropItem(i, j, tile.frameX, tile.frameY);
            }
        }

        protected virtual void DropItem(int i, int j, int Fx, int Fy)
        {
            Item.NewItem(new Rectangle(i * 16, j * 16, 16, 16), PlaceItems[GetDropKind(Fx, Fy)].item.type);
        }

        public Point16 GetOrigin(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return GetOrigin(x, y, tile.frameX, tile.frameY);
        }

        public Point16 GetOrigin(int x, int y, int Fx, int Fy)
        {
            if (OneByOne)
                return new Point16(x, y);

            int xOffset = Fx % tileObjectData.CoordinateFullWidth / tileObjectData.CoordinateWidth - tileObjectData.Origin.X;
            int yOffset = 0;

            if (simpleHeigth)
            {
                yOffset = Fy % tileObjectData.CoordinateFullHeight / tileObjectData.CoordinateHeights[0];
            }
            else
            {
                int FullY = Fy % tileObjectData.CoordinateFullHeight;

                for (int i = 0; i < tileObjectData.CoordinateHeights.Length && FullY >= tileObjectData.CoordinateHeights[i]; i++)
                {
                    FullY -= tileObjectData.CoordinateHeights[i];
                    yOffset++;
                }
            }
            yOffset -= tileObjectData.Origin.Y;

            return new Point16(x - xOffset, y - yOffset);
        }

        protected abstract void SetTileObjectData();
    }
}