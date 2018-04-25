using Terraria.ModLoader;

namespace MechTransfer.Tiles.Simple
{
    public abstract class SimpleTile : ModTile
    {
        public virtual void PostLoad()
        {
        }

        public virtual void Addrecipes()
        {
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            return false;
        }
    }
}