using MechTransfer.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferAssemblerTile : FilterableTile<TransferAssemblerTileEntity>
    {
        private ItemInventory inventory = new ItemInventory();
        private List<Recipe> recipes = new List<Recipe>();

        private Dictionary<int, int[]> tileRemap = new Dictionary<int, int[]>() {
            { 302,  new int[]{ 17 } },
            { 77,  new int[]{ 17 } },
            { 133,  new int[]{ 17, 77 } },
            { 134,  new int[]{ 16 } },
            { 354,  new int[]{ 14 } },
            { 469,  new int[]{ 14 } },
            { 355,  new int[]{ 13, 14 } },
        };

        public override void SetDefaults()
        {
            AddMapEntry(MapColors.Input, GetPlaceItem(0).DisplayName);

            base.SetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            base.SetTileObjectData();
        }

        public override void HitWire(int i, int j)
        {
            if (Main.netMode == 1)
                return;

            TransferAssemblerTileEntity entity;
            if (!TryGetEntity(i, j, out entity))
                return;

            if (entity.item.IsAir || entity.stock.stack > 0)
                return;

            inventory.Clear();
            foreach (var c in ModContent.GetInstance<TransferAgent>().FindContainerAdjacent(i, j))
            {
                inventory.RegisterContainer(c);
            }

            bool foundRecipe = false;
            for (int r = 0; r < Recipe.maxRecipes && !Main.recipe[r].createItem.IsAir; r++)
            {
                if (Main.recipe[r].createItem.type == entity.item.type &&
                    (entity.selectedRecipeHash == 0 || RecipeUtils.HashRecipe(Main.recipe[r]) == entity.selectedRecipeHash))
                {
                    foundRecipe = true;
                    if (TryMakeRecipe(Main.recipe[r], entity))
                        break;
                }
            }

            if (!foundRecipe)
                entity.Status = TransferAssemblerTileEntity.StatusKind.NoRecipe;

            inventory.Clear();

            NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, entity.ID, entity.Position.X, entity.Position.Y);
        }

        private bool TryMakeRecipe(Recipe recipe, TransferAssemblerTileEntity entity)
        {
            bool alchemy;
            if (!SearchStation(recipe, entity.Position.X, entity.Position.Y, out alchemy))
            {
                entity.Status = TransferAssemblerTileEntity.StatusKind.MissingStation;
                return false;
            }

            for (int i = 0; i < Recipe.maxRequirements && !recipe.requiredItem[i].IsAir; i++)
            {
                if (!inventory.TryTakeIngredient(recipe, recipe.requiredItem[i]))
                {
                    entity.Status = TransferAssemblerTileEntity.StatusKind.MissingItem;
                    entity.MissingItemType = recipe.requiredItem[i].type;
                    return false;
                }
            }

            Item clone = recipe.createItem.Clone();

            if (!clone.IsAir)
            {
                //these can potentially cause issues in some cases, might have to remove them
                RecipeHooks.OnCraft(clone, recipe);
                ItemLoader.OnCraft(clone, recipe);
            }

            entity.stock = clone;
            entity.Status = TransferAssemblerTileEntity.StatusKind.Success;
            inventory.Commit(alchemy);

            return true;
        }

        private bool SearchStation(Recipe recipe, int x, int y, out bool alchemy)
        {
            alchemy = false;

            bool[] tileOk = new bool[Recipe.maxRequirements];
            bool waterOk = !recipe.needWater;
            bool honeyOk = !recipe.needHoney;
            bool lavaOk = !recipe.needLava;
            bool snowOk = !recipe.needSnowBiome;

            for (int i = x - 5; i <= x + 5; i++)
            {
                for (int j = y - 5; j <= y + 5; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.active())
                    {
                        for (int z = 0; z < Recipe.maxRequirements && recipe.requiredTile[z] != -1; z++)
                        {
                            ModTile modTile = TileLoader.GetTile(tile.type);

                            if ((recipe.requiredTile[z] == tile.type) ||
                                (tileRemap.ContainsKey(tile.type) && tileRemap[tile.type].Contains(recipe.requiredTile[z])) ||
                                (modTile != null && modTile.adjTiles.Contains(recipe.requiredTile[z])))
                            {
                                tileOk[z] = true;
                            }

                            //easier than reimplementing the zone finding logic
                            if (tile.type == TileID.SnowBlock || tile.type == TileID.IceBlock || tile.type == TileID.HallowedIce || tile.type == TileID.FleshIce || tile.type == TileID.CorruptIce)
                                snowOk = true;

                            if (tile.type == TileID.AlchemyTable && recipe.alchemy)
                                alchemy = true;

                            //can't access TileLoader.HookAdjTiles, so if a mod uses that, it won't work
                        }
                    }

                    if (tile != null && tile.liquid > 200)
                    {
                        switch (tile.liquidType())
                        {
                            case 0: waterOk = true; break;
                            case 2: honeyOk = true; break;
                            case 1: lavaOk = true; break;
                        }
                    }
                }
            }

            if (!waterOk || !honeyOk || !lavaOk || !snowOk)
                return false;

            for (int i = 0; i < Recipe.maxRequirements && recipe.requiredTile[i] != -1; i++)
            {
                if (!tileOk[i])
                    return false;
            }

            return true;
        }

        public override void DisplayTooltip(int i, int j)
        {
            if (!TryGetEntity(i, j, out TransferAssemblerTileEntity entity))
            {
                return;
            }

            if (Main.keyState.IsKeyDown(Keys.LeftAlt) && PlayerInput.ScrollWheelDeltaForUI != 0 && recipes.Count > 0)
            {
                Main.PlaySound(SoundID.MenuTick);
                HandleWheel(PlayerInput.ScrollWheelDelta / 120, entity);
                entity.SyncData();
            }

            ((MechTransfer)mod).assemblerHoverUI.Display(entity.item, HoverText(entity), HoverColor(entity), entity.selectedRecipeHash, recipes);
        }

        private void HandleWheel(int delta, TransferAssemblerTileEntity entity)
        {
            if (entity.selectedRecipeHash == 0)
            {
                if (delta < 0)
                    entity.selectedRecipeHash = RecipeUtils.HashRecipe(recipes[0]);
                else
                    entity.selectedRecipeHash = RecipeUtils.HashRecipe(recipes[recipes.Count - 1]);
            }
            else
            {
                int[] hashes = new int[recipes.Count];

                for (int r = 0; r < hashes.Length; r++)
                    hashes[r] = RecipeUtils.HashRecipe(recipes[r]);

                int index = -1;

                for (int c = 0; c < hashes.Length; c++)
                    if (hashes[c] == entity.selectedRecipeHash)
                    {
                        index = c;
                        break;
                    }

                if (index == -1)
                {
                    entity.selectedRecipeHash = 0;
                    HandleWheel(delta, entity);
                    return;
                }

                if (delta < 0)
                {
                    index++;
                }
                else
                {
                    index--;
                }

                if (index < 0 || index >= hashes.Length)
                    entity.selectedRecipeHash = 0;
                else
                    entity.selectedRecipeHash = hashes[index];
            }
        }

        public override string HoverText(TransferAssemblerTileEntity entity)
        {
            switch (entity.Status)
            {
                case TransferAssemblerTileEntity.StatusKind.Ready:
                    return "Ready";

                case TransferAssemblerTileEntity.StatusKind.Success:
                    return "Success";

                case TransferAssemblerTileEntity.StatusKind.MissingItem:
                    return string.Format("Missing ingredient ({0})", ItemNameById(entity.MissingItemType));

                case TransferAssemblerTileEntity.StatusKind.MissingStation:
                    return "Missing crafting station";

                case TransferAssemblerTileEntity.StatusKind.MissingSpace:
                    return string.Format("Cant deposit ({0} x{1})", entity.stock.Name, entity.stock.stack);

                case TransferAssemblerTileEntity.StatusKind.NoRecipe:
                    return "No recipe found";

                default:
                    return "How?!?";
            }
        }

        public override Color HoverColor(TransferAssemblerTileEntity entity)
        {
            switch (entity.Status)
            {
                case TransferAssemblerTileEntity.StatusKind.Ready:
                    return Color.Yellow;

                case TransferAssemblerTileEntity.StatusKind.Success:
                    return Color.Green;

                case TransferAssemblerTileEntity.StatusKind.MissingItem:
                case TransferAssemblerTileEntity.StatusKind.MissingStation:
                case TransferAssemblerTileEntity.StatusKind.MissingSpace:
                case TransferAssemblerTileEntity.StatusKind.NoRecipe:
                    return Color.Red;

                default:
                    return Color.CornflowerBlue;
            }
        }

        public static string ItemNameById(int id)
        {
            if (id == 0)
                return "";

            LocalizedText name = Lang.GetItemName(id);
            if (name == LocalizedText.Empty)
                return string.Format("Unknown item #{0}", id);
            return name.Value;
        }

        public override void PostLoad()
        {
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(mod, "TransferAssemblerItem", Type, 16, 16, 0, Item.sellPrice(0, 1, 0, 0));
            PlaceItems[0].item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Cog, 10);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(PlaceItems[0], 1);
            r.AddRecipe();
        }
    }
}