using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace MechTransfer.Tiles
{
    public class LivingHellstoneTile : SimplePlaceableTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.TouchDamageHot[Type] = 20;

            animationFrameHeight = 90;

            dustType = 6;
            soundType = 21;
            soundStyle = 1;
            mineResist = 2;

            base.SetDefaults();
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;

            if ((frameCounter > 60 && frameCounter < 70) || (frameCounter > 80))
                frame = 1;
            else
                frame = 0;

            if (frameCounter > 90)
                frameCounter = 0;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.350f;
            g = 0.150f;
            b = 0.150f;
        }

        public override void PostLoad()
        {
            PlaceItem = SimplePrototypeItem.MakePlaceable(mod, "LivingHellstoneItem", Type);
            PlaceItem.item.rare = ItemRarityID.Green;
            PlaceItem.item.mech = false;
        }


        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.HellstoneBar, 5);
            r.AddIngredient(ItemID.Obsidian, 5);
            r.AddIngredient(ItemID.LavaBucket, 1);
            r.AddTile(TileID.Anvils);
            r.SetResult(PlaceItem.item.type, 1);
            r.AddRecipe();
        }
    }
}