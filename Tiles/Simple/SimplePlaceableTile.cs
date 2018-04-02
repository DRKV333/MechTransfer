using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using MechTransfer.Items;
using Terraria;

namespace MechTransfer.Tiles.Simple
{
    public abstract class SimplePlaceableTile : SimpleTile
    {
        public ModItem placeItem { get; protected set; }

        public override void PostLoad()
        {
            mod.AddItem(LoadPlaceItem(), placeItem);
        }

        public override void SetDefaults()
        {
            drop = placeItem.item.type;
        }

        public abstract string LoadPlaceItem();
    }
}
