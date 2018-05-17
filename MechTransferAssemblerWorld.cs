using MechTransfer.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace MechTransfer
{
    internal class MechTransferAssemblerWorld : ModWorld
    {
        Texture2D pixel;

        public override void Initialize()
        {
            pixel = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });
        }

        public override void PostDrawTiles()
        {
            if (!WiresUI.Settings.DrawWires)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            int screenLeft = (int)((Main.screenPosition.X) / 16f - 1f);
            int screenRight = (int)((Main.screenPosition.X + (float)Main.screenWidth) / 16f) + 2;
            int screenTop = (int)((Main.screenPosition.Y) / 16f - 1f);
            int screenBottom = (int)((Main.screenPosition.Y + (float)Main.screenHeight) / 16f) + 5;
            if (screenLeft < 0)
                screenLeft = 0;
            if (screenRight > Main.maxTilesX)
                screenRight = Main.maxTilesX;
            if (screenTop < 0)
                screenTop = 0;
            if (screenBottom > Main.maxTilesY)
                screenBottom = Main.maxTilesY;

            for (int x = screenLeft; x < screenRight; x++)
            {
                for (int y = screenTop; y < screenBottom; y++)
                {
                    if (Main.tile[x, y] != null && Main.tile[x, y].active() && Main.tile[x, y].type == mod.TileType<TransferAssemblerTile>())
                    {
                        DrawRectFast(x * 16 - 80 - (int)Main.screenPosition.X, y * 16 - 80 - (int)Main.screenPosition.Y, 176, 176);
                    }
                }
            }

            Main.spriteBatch.End();
        }

        void DrawRectFast(int left, int top, int height, int width)
        {
            if (Main.LocalPlayer.gravDir == -1)
                top = Main.screenHeight - top - height;

            Main.spriteBatch.Draw(pixel, new Rectangle(left, top, width, 2), null, Color.LightSeaGreen);
            Main.spriteBatch.Draw(pixel, new Rectangle(left, top + height, width, 2), null, Color.LightSeaGreen);
            Main.spriteBatch.Draw(pixel, new Rectangle(left, top, 2, height), null, Color.LightSeaGreen);
            Main.spriteBatch.Draw(pixel, new Rectangle(left + width, top, 2, height), null, Color.LightSeaGreen);
        }
    }
}