using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MechTransfer
{
    internal class ButtonDelayWorld : ModSystem
    {
        private Point16? buttonPosition = null;
        private int delay;

        private const int StartDelay = 30;

        public void setPoint(Point16 p)
        {
            buttonPosition = p;
            delay = StartDelay;
        }

        public bool isPoint(Point16 p, int w, int h)
        {
            if (!buttonPosition.HasValue)
                return false;

            int xOffset = p.X - buttonPosition.Value.X;
            int yOffset = p.Y - buttonPosition.Value.Y;

            return xOffset >= 0 && yOffset >= 0 && xOffset < w && yOffset < h;
        }

        public override void PostDrawTiles()
        {
            delay--;
            if (delay < 1)
                buttonPosition = null;
        }
    }
}