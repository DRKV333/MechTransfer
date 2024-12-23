using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MechTransfer.Tiles.Simple
{
    /// <summary>
    /// Any concrete class that extends from this one should be [Autoload(false)],
    /// because they are loaded manually in the Mod class.
    /// Unfortunately I don't see way to do this in one place.
    /// </summary>
    public abstract class SimpleTile : ModTile
    {
        public virtual void PostLoad()
        {
        }

        public virtual void AddRecipes()
        {
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [];
        }
    }
}