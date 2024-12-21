using MechTransfer.Items;
using MechTransfer.Tiles.Simple;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace MechTransfer.Tiles
{
    [Autoload(false)]
    public class LivingHellstoneTile : SimplePlaceableTile
    {
        public override void PostSetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.TouchDamageHot[Type] = true;

            AnimationFrameHeight = 90;

            DustType = 6;
            HitSound = SoundID.Tink;
            MineResist = 2;

            base.PostSetDefaults();
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
            PlaceItem = SimplePrototypeItem.MakePlaceable(Mod, "LivingHellstoneItem", Type);
            PlaceItem.Item.rare = ItemRarityID.Green;
            PlaceItem.Item.mech = false;
        }


        public override void AddRecipes()
        {
            Recipe r = Recipe.Create(PlaceItem.Item.type, 1);
            r.AddIngredient(ItemID.HellstoneBar, 5);
            r.AddIngredient(ItemID.Obsidian, 5);
            r.AddIngredient(ItemID.LavaBucket, 1);
            r.AddTile(TileID.Anvils);
            r.Register();
        }
    }
}