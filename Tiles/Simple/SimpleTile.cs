using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace MechTransfer.Tiles.Simple
{
    public abstract class SimpleTile : ModTile
    {
        public virtual void PostLoad() { }

        public virtual void Addrecipes() { }

        public override bool Autoload(ref string name, ref string texture)
        {
            return false;
        }
    }
}
