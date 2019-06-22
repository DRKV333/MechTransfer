using Microsoft.Xna.Framework;

namespace MechTransfer
{
    public static class MapColors
    {
        public static Color FillMid { get { return new Color(90, 90, 90); } }
        public static Color FillDark { get { return new Color(56, 56, 56); } }
        public static Color FillLight { get { return new Color(127, 127, 127); } }

        public static Color Input { get { return new Color(107, 0, 0); } }
        public static Color Output { get { return new Color(0, 107, 0); } }
        public static Color Passthrough { get { return new Color(255, 104, 0); } }
    }
}
