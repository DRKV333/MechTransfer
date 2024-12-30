using MechTransfer.Items;
using MechTransfer.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    [Autoload(false)]
    public class TransferAssemblerTile : FilterableTile<TransferAssemblerTileEntity>
    {
        private ItemInventory inventory = new ItemInventory();

        private Dictionary<int, int[]> tileRemap = new Dictionary<int, int[]>() {
            { 302,  new int[]{ 17 } },
            { 77,  new int[]{ 17 } },
            { 133,  new int[]{ 17, 77 } },
            { 134,  new int[]{ 16 } },
            { 354,  new int[]{ 14 } },
            { 469,  new int[]{ 14 } },
            { 355,  new int[]{ 13, 14 } },
        };

        public override void PostSetDefaults()
        {
            AddMapEntry(MapColors.Input, GetPlaceItem(0).DisplayName);

            base.PostSetDefaults();
        }

        protected override void SetTileObjectData()
        {
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            base.SetTileObjectData();
        }
        public override void DisplayTooltip(int i, int j)
        {
            if (TryGetEntity(i, j, out TransferAssemblerTileEntity entity))
                ModContent.GetInstance<UIModSystem>().assemblerHoverUI.Display(entity.Result, entity.Ingredients, entity.Stations, HoverText(entity), HoverColor(entity));
        }
        //TODO consider opening a recipe select list
        public override bool RightClick(int i, int j)
        {
            if (!Main.LocalPlayer.HeldItem.IsAir)
            {
                if (TryGetEntity(i, j, out TransferAssemblerTileEntity tileEntity))
                {
                    tileEntity.item.stack = 1;
                    bool sameItem = tileEntity.item.type == Main.LocalPlayer.HeldItem.type;
                    tileEntity.item = Main.LocalPlayer.HeldItem.Clone();
                    if (sameItem)
                    {
                        tileEntity.SetNextRecipe();
                    }
                    else
                    {
                        tileEntity.SetRecipe();
                    }
                    tileEntity.SyncData();
                    return true;
                }
            }
            return false;
        }

        public override void HitWire(int i, int j)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (!TryGetEntity(i, j, out TransferAssemblerTileEntity entity))
                return;

            if (entity.item.IsAir || entity.recipe is null || entity.stock.stack > 0)
                return;

            inventory.Clear();
            foreach (var c in ModContent.GetInstance<TransferAgent>().FindContainerAdjacent(i, j))
            {
                inventory.RegisterContainer(c);
            }

            TryMakeRecipe(entity.recipe, entity);

            inventory.Clear();

            NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, entity.ID, entity.Position.X, entity.Position.Y);
        }

        private bool TryMakeRecipe(Recipe recipe, TransferAssemblerTileEntity entity)
        {
            if (!SearchStation(recipe, entity.Position.X, entity.Position.Y, out bool alchemy))
            {
                entity.Status = TransferAssemblerTileEntity.StatusKind.MissingStation;
                return false;
            }

            for (int i = 0; i < recipe.requiredItem.Count && !recipe.requiredItem[i].IsAir; i++)
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
                // TODO:
                // Not sure what to call instead of RecipeHooks.OnCraft and ItemLoader.OnCraft
                // RecipeLoader.OnCraft maybe? Nebula: yeah probably
            }

            entity.stock = clone;
            entity.Status = TransferAssemblerTileEntity.StatusKind.Success;
            inventory.Commit(alchemy);

            return true;
        }

        private bool SearchStation(Recipe recipe, int x, int y, out bool alchemy)
        {
            alchemy = false;

            // Conditions are tied to LocalPlayer, and there's like a million of them.
            // We're gonna check them on recipe setup directly, so no work to do here besides station tiles.

            bool[] tileOk = new bool[recipe.requiredTile.Count];

            for (int i = x - 5; i <= x + 5; i++)
            {
                for (int j = y - 5; j <= y + 5; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.HasTile)
                    {
                        for (int z = 0; z < recipe.requiredTile.Count && recipe.requiredTile[z] != -1; z++)
                        {
                            
                            ModTile modTile = TileLoader.GetTile(tile.TileType);

                            if ((recipe.requiredTile[z] == tile.TileType) ||
                                (tileRemap.ContainsKey(tile.TileType) && tileRemap[tile.TileType].Contains(recipe.requiredTile[z])) ||
                                (modTile != null && modTile.AdjTiles.Contains(recipe.requiredTile[z])))
                            {
                                tileOk[z] = true;
                            }

                            //easier than reimplementing the zone finding logic
                            // if (tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock || tile.TileType == TileID.HallowedIce || tile.TileType == TileID.FleshIce || tile.TileType == TileID.CorruptIce)
                            //     snowOk = true;

                            // if (tile.TileType == TileID.AlchemyTable && recipe.alchemy)
                            //     alchemy = true;

                            //can't access TileLoader.HookAdjTiles, so if a mod uses that, it won't work
                        }
                    }
                }
            }

            for (int i = 0; i < recipe.requiredTile.Count && recipe.requiredTile[i] != -1; i++)
            {
                if (!tileOk[i])
                    return false;
            }

            return true;
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
            PlaceItems[0] = SimplePrototypeItem.MakePlaceable(Mod, "TransferAssemblerItem", Type, 16, 16, 0, Item.sellPrice(0, 1, 0, 0));
            PlaceItems[0].Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            Recipe r = Recipe.Create(PlaceItems[0].Item.type, 1);
            r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
            r.AddIngredient(ItemID.Cog, 10);
            r.AddTile(TileID.WorkBenches);
            r.Register();
        }
    }
}