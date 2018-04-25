using MechTransfer.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace MechTransfer
{
    internal class MechTransferAssemblerWorld : ModWorld
    {
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
                        Vector2 start = new Vector2(x * 16 - 81, y * 16 - 81);
                        Vector2 end = new Vector2(x * 16 + 97, y * 16 + 97);
                        Utils.DrawRectangle(Main.spriteBatch, start, end, Color.LightSeaGreen, Color.LightSeaGreen, 2f);
                    }
                }
            }

            Main.spriteBatch.End();
        }
    }
}