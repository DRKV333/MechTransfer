using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using MechTransfer.Tiles.Simple;
using MechTransfer.Items;
using Terraria.ID;

namespace MechTransfer.Tiles
{
    public class TransferPipeTile : SimpleTileObject
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            Main.tileFrameImportant[Type] = false;

            ((MechTransfer)mod).transferAgent.unconditionalPassthroughType = Type;
            
            AddMapEntry(new Color(200, 200, 200));
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        }

        public override void PostLoad()
        {
            SimplePlaceableItem i = new SimplePlaceableItem();
            i.placeType = Type;
            mod.AddItem("TransferPipeItem", i);
            i.DisplayName.AddTranslation(LangID.English, "Transfer pipe");
            i.Tooltip.AddTranslation(LangID.English, "Used to connect item transfer devices");
            placeItems[0] = i;
        }

        public override void Addrecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(mod.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.IronBar, 1);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(placeItems[0].item.type, 25);
            r.AddRecipe();
        }
    }
}