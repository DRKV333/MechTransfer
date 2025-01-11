using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MechTransfer.Tiles
{
    public class TransferAssemblerTileEntity : TransferFilterTileEntity
    {
        public enum StatusKind { Ready, Success, MissingItem, MissingStation, MissingSpace, NoRecipe }

        public StatusKind Status = StatusKind.Ready;
        public int MissingItemType = 0;
        public Item stock = new Item();
        public Recipe recipe { get; private set; }

        public Item Result => item;
        private List<Item> ingredients = null;
        public List<Item> Ingredients => recipe != null ? recipe.requiredItem : ingredients;
        public List<int> Groups => recipe != null ? recipe.acceptedGroups : [];
        public List<int> Stations => recipe != null ? recipe.requiredTile : [];

        private int timer = 0;

        public override void Update()
        {
            if (timer > 0)
            {
                timer--;
                return;
            }

            if (stock.stack > 0)
            {
                foreach (var container in ModContent.GetInstance<TransferAgent>().FindContainerAdjacent(Position.X, Position.Y))
                {
                    container.InjectItem(stock);

                    if (stock.stack < 1)
                    {
                        Status = StatusKind.Success;
                        break;
                    }
                }

                if (stock.stack > 0)
                {
                    timer = 60;
                    Status = StatusKind.MissingSpace;
                }

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
            }
        }

        public bool MatchRecipe(Recipe other)
        {
            if (item.IsAir || Ingredients == null)
                return false;
            
            if (recipe != null)
                return recipe.Equals(other);
            
            if (Result.type != other.createItem.type ||
                Result.stack != other.createItem.stack ||
                Ingredients.Count != other.requiredItem.Count)
            {
                return false;
            }
            
            for (int i = 0; i < Ingredients.Count; i++)
            {
                if (Ingredients[i].type != other.requiredItem[i].type ||
                    Ingredients[i].stack != other.requiredItem[i].stack)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Uses the already set item field to select a new recipe depending on the requested mode.
        /// mode 0 = first, mode 1 = match, mode 2 = next after match.
        /// Only in mode 2, nothing will happen if no recipe is found
        /// If a recipe is found but the requested mode is not satisfied, the first found recipe is set instead.
        /// </summary>
        /// <param name="mode">Defines the search mode. Defaults to 0</param>
        public void SetRecipe(int mode = 0)
        {
            if (item.IsAir)
            {
                recipe = null;
                Status = StatusKind.Ready;
                return;
            }
            else if (item.ModItem != null && item.ModItem.Name == "UnloadedItem")
            {
                Status = StatusKind.NoRecipe;
                return;
            }

            Recipe firstRecipe = null; // we default to the first recipe in the list if the loop fails (mode 1 or higher)
            bool foundRecipe = false; // in mode 2 we need to first find the currently set recipe. Only when found we save the next valid recipe
            for (int r = 0; r < Recipe.maxRecipes && !Main.recipe[r].createItem.IsAir; r++)
            {
                Recipe currRecipe = Main.recipe[r];
                if (!currRecipe.Disabled && currRecipe.createItem.type == item.type)
                {
                    bool valid = true;
                    foreach (Condition condition in currRecipe.Conditions)
                    {
                        if (!condition.IsMet())
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        firstRecipe ??= currRecipe;
                        
                        if (mode == 0) //in mode 0, we found our first recipe so we end it here.
                            break;
                        
                        if (MatchRecipe(currRecipe))
                        {
                            if (mode == 1) //in mode 1, we found a match so we end it here
                            {
                                recipe = currRecipe;
                                Status = StatusKind.Ready;
                                return; 
                            }
                            foundRecipe = true; //in mode 2, we found our match so we remember this fact for later
                        }
                        else if (foundRecipe) // in mode 2, we found a valid recipe after our match so we end it here
                        {
                            recipe = currRecipe;
                            Status = StatusKind.Ready;
                            return;
                        }
                    }
                }
            }
            if (mode != 1) //we change recipe only in mode 0 and 2
            {
                if (firstRecipe != null) //we have found A recipe but not the CURRENTLY SET recipe. we save our first find
                {
                    recipe = firstRecipe;
                    Status = StatusKind.Ready;
                    return;
                }
                //nothing was found at all
                recipe = null;
                ingredients = null;
            }

            Status = StatusKind.NoRecipe;
        }

        public void ReloadRecipe() => SetRecipe(1);
        
        public void SetNextRecipe() => SetRecipe(2);

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag.Add("ingrds", Ingredients);
            tag.Add("stck", ItemIO.Save(stock));
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            
            if (tag.ContainsKey("ingrds"))
            {
                ingredients = tag.Get<List<Item>>("ingrds");
                ReloadRecipe();
            }
            else
            {
                Status = StatusKind.NoRecipe;
            }

            stock = ItemIO.Load((TagCompound)tag["stck"]);
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write((byte)Status);
            writer.Write(MissingItemType);
            ItemIO.Send(stock, writer, true);
            writer.Write(Ingredients == null ? -1 : Ingredients.Count);

            if (Ingredients != null)
            {
                foreach (Item ingredient in Ingredients)
                {
                    ItemIO.Send(ingredient, writer, true);
                }
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            Status = (StatusKind)reader.ReadByte();
            MissingItemType = reader.ReadInt32();
            stock = ItemIO.Receive(reader, true);
            List<Item> ingrds = [];

            int num = reader.ReadInt32();
            if (num >= 0)
            {
                for (int i = 0; i < num; i++)
                {
                    ingrds.Add(ItemIO.Receive(reader, true));
                }
            }
            else
            {
                ingredients = null;
            }
            ingredients = ingrds;
            ReloadRecipe();
        }

        public override void SyncData()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = NetRouter.GetPacketTo(ModContent.GetInstance<TransferAssemblerTileEntity>(), Mod);
                packet.Write(ID);
                ItemIO.Send(item, packet);
                packet.Write(Ingredients == null ? -1 : Ingredients.Count);
                if (Ingredients != null)
                {
                    foreach (Item ingredient in Ingredients)
                        ItemIO.Send(ingredient, packet);
                }
                packet.Send();
            }
        }

        public override void HandlePacket(BinaryReader reader, int WhoAmI)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            TransferAssemblerTileEntity FilterEntity = (TransferAssemblerTileEntity)ByID[reader.ReadInt32()];
            FilterEntity.item = ItemIO.Receive(reader);
            List<Item> ingrds = [];

            int num = reader.ReadInt32();
            if (num >= 0)
            {
                for (int i = 0; i < num; i++)
                {
                    ingrds.Add(ItemIO.Receive(reader));
                }

                FilterEntity.ingredients = ingrds;
            }
            else
            {
                FilterEntity.ingredients = null;
            }

            ReloadRecipe();
            NetMessage.SendData(MessageID.TileEntitySharing, -1, WhoAmI, null, FilterEntity.ID, FilterEntity.Position.X, FilterEntity.Position.Y);
        }
    }
}
