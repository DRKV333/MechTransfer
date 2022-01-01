using MechTransfer.ContainerAdapters;
using MechTransfer.Items;
using MechTransfer.Tiles;
using MechTransfer.Tiles.Simple;
using MechTransfer.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using EnumerateItemsDelegate = System.Func<int, int, System.Collections.Generic.IEnumerable<System.Tuple<Terraria.Item, object>>>;
using InjectItemDelegate = System.Func<int, int, Terraria.Item, bool>;
using TakeItemDelegate = System.Action<int, int, object, int>;

namespace MechTransfer
{
    public class MechTransfer : Mod
    {
        private const string callErorPrefix = "MechTransfer Call() error: ";
        private const string registerAdapter = "RegisterAdapter";
        private const string registerAdapterReflection = "RegisterAdapterReflection";

        private GameInterfaceLayer interfaceLayer;

        public FilterHoverUI filterHoverUI;
        public AssemblerHoverUI assemblerHoverUI;

        private List<Action> simpleTileAddRecipequeue;

        private Mod modMagicStorage = null;
        private Mod modMagicStorageExtra = null;

        public MechTransfer()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override object Call(params object[] args)
        {
            if ((args[0] as string) == registerAdapter)
            {
                if (args.Length != 5)
                {
                    this.Logger.Error(callErorPrefix + "Invalid number of arguments at " + registerAdapter);
                    return null;
                }

                ContainerAdapterDefinition definition = new ContainerAdapterDefinition();

                if (!(args[1] is InjectItemDelegate))
                {
                    this.Logger.Error(callErorPrefix + "Invalid argument 2 InjectItem at " + registerAdapter);
                    return null;
                }

                definition.InjectItem = args[1] as InjectItemDelegate;

                if (!(args[2] is EnumerateItemsDelegate))
                {
                    this.Logger.Error(callErorPrefix + "Invalid argument 3 EnumerateItems at " + registerAdapter);
                    return null;
                }

                definition.EnumerateItems = args[2] as EnumerateItemsDelegate;

                if (!(args[3] is TakeItemDelegate))
                {
                    this.Logger.Error(callErorPrefix + "Invalid argument 4 TakeItem at " + registerAdapter);
                    return null;
                }

                definition.TakeItem = args[3] as TakeItemDelegate;

                if (!(args[4] is int[]))
                {
                    this.Logger.Error(callErorPrefix + "Invalid argument 5 TileType at " + registerAdapter);
                    return null;
                }

                foreach (var type in (int[])args[4])
                {
                    if (!ModContent.GetInstance<TransferAgent>().ContainerAdapters.ContainsKey(type))
                        ModContent.GetInstance<TransferAgent>().ContainerAdapters.Add(type, definition);
                }
                return definition;
            }
            if ((args[0] as string) == registerAdapterReflection)
            {
                try
                {
                    ContainerAdapterDefinition definition = new ContainerAdapterDefinition();

                    Type type = args[1].GetType();

                    ParameterExpression paramX = Expression.Parameter(typeof(int));
                    ParameterExpression paramY = Expression.Parameter(typeof(int));

                    MethodInfo inject = type.GetMethod("InjectItem", new Type[] { typeof(int), typeof(int), typeof(Item) });
                    ParameterExpression paramInjectItem = Expression.Parameter(typeof(Item));
                    InjectItemDelegate injectLambda = Expression.Lambda<InjectItemDelegate>(
                        Expression.Call(Expression.Constant(args[1]), inject, paramX, paramY, paramInjectItem),
                        paramX, paramY, paramInjectItem).Compile();

                    MethodInfo enumerate = type.GetMethod("EnumerateItems", new Type[] { typeof(int), typeof(int) });
                    EnumerateItemsDelegate enumerateLambda = Expression.Lambda<EnumerateItemsDelegate>(
                        Expression.Call(Expression.Constant(args[1]), enumerate, paramX, paramY),
                        paramX, paramY).Compile();

                    MethodInfo take = type.GetMethod("TakeItem", new Type[] { typeof(int), typeof(int), typeof(object), typeof(int) });
                    ParameterExpression paramTakeIdentifier = Expression.Parameter(typeof(object));
                    ParameterExpression paramTakeAmount = Expression.Parameter(typeof(int));
                    TakeItemDelegate takeLambda = Expression.Lambda<TakeItemDelegate>(
                        Expression.Call(Expression.Constant(args[1]), take, paramX, paramY, paramTakeIdentifier, paramTakeAmount),
                        paramX, paramY, paramTakeIdentifier, paramTakeAmount).Compile();

                    definition.InjectItem = injectLambda;
                    definition.EnumerateItems = enumerateLambda;
                    definition.TakeItem = takeLambda;

                    if (!(args[2] is int[]))
                    {
                        this.Logger.Error(callErorPrefix + "Invalid argument 5 TileType at " + registerAdapterReflection);
                        return null;
                    }

                    foreach (var t in (int[])args[2])
                    {
                        if (!ModContent.GetInstance<TransferAgent>().ContainerAdapters.ContainsKey(t))
                            ModContent.GetInstance<TransferAgent>().ContainerAdapters.Add(t, definition);
                    }
                    return definition;
                }
                catch (Exception e)
                {
                    this.Logger.Error(callErorPrefix + "An exception has occurred while loading adapter at " + registerAdapterReflection);
                    this.Logger.Error(e.Message);
                    return null;
                }
            }
            this.Logger.Error(callErorPrefix + "Invalid command");
            return null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetRouter.RouteMessage(reader, whoAmI);
        }

        public override void Load()
        {
            modMagicStorage = ModLoader.GetMod("MagicStorage");
            modMagicStorageExtra = ModLoader.GetMod("MagicStorageExtra");

            Assembly asm = Assembly.GetExecutingAssembly();
            simpleTileAddRecipequeue = new List<Action>();
            Dictionary<Type, List<int>> TEValid = new Dictionary<Type, List<int>>();
            SimpleTileEntity.validTiles = new Dictionary<int, int[]>();
            foreach (var item in asm.GetTypes())
            {
                if (item.IsAbstract)
                    continue;

                if (item.IsSubclassOf(typeof(SimpleTile)))
                {
                    SimpleTile tile = (SimpleTile)Activator.CreateInstance(item);
                    AddTile(item.Name, tile, item.FullName.Replace('.', '/'));
                    tile.PostLoad();
                    simpleTileAddRecipequeue.Add(new Action(tile.AddRecipes));

                    Type TEType;
                    if (IsTETile(item, out TEType))
                    {
                        if (!TEValid.ContainsKey(TEType))
                            TEValid.Add(TEType, new List<int>());

                        TEValid[TEType].Add(tile.Type);
                    }
                }
            }

            foreach (var item in asm.GetTypes())
            {
                if (item.IsSubclassOf(typeof(SimpleTileEntity)))
                {
                    SimpleTileEntity TE = (SimpleTileEntity)Activator.CreateInstance(item);
                    AddTileEntity(item.Name, TE);

                    if (TEValid.ContainsKey(item))
                    {
                        SimpleTileEntity.validTiles.Add(TE.Type, TEValid[item].ToArray());
                    }
                    else
                    {
                        SimpleTileEntity.validTiles.Add(TE.Type, new int[0]);
                    }

                    TE.PostLoadPrototype();
                }
            }

            if (!Main.dedServ)
                LoadUI();
        }

        private bool IsTETile(Type type, out Type TEType)
        {
            List<Type> bases = new List<Type>();

            Type baseType = type.BaseType;
            while (baseType != typeof(object))
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(SimpleTETile<>))
                    break;
                baseType = baseType.BaseType;
            }

            if (baseType == typeof(object))
            {
                TEType = null;
                return false;
            }
            else
            {
                TEType = type.GetMethod("GetEntity").ReturnType;
                return true;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(x => x.Name == "Vanilla: Mouse Text") + 1;
            layers.Insert(index, interfaceLayer);

            if (filterHoverUI.visible)
            {
                layers.Find(x => x.Name == "Vanilla: Interact Item Icon").Active = false;
            }
        }

        public override void PostSetupContent()
        {
            LoadAdapters();
        }

        private void LoadUI()
        {
            filterHoverUI = new FilterHoverUI();
            filterHoverUI.Activate();

            assemblerHoverUI = new AssemblerHoverUI();
            assemblerHoverUI.Activate();

            interfaceLayer = new LegacyGameInterfaceLayer("MechTransfer: UI",
                                                            delegate
                                                            {
                                                                filterHoverUI.Draw(Main.spriteBatch);
                                                                assemblerHoverUI.Draw(Main.spriteBatch);
                                                                return true;
                                                            },
                                                            InterfaceScaleType.UI);
            UIHooks.Load(this);
        }

        private void LoadAdapters()
        {
            //Item frame
            ItemFrameAdapter itemFrameAdapter = new ItemFrameAdapter();
            Call(registerAdapterReflection, itemFrameAdapter, new int[] { TileID.ItemFrame });

            //Snowball launcher
            SnowballLauncherAdapter snowballLauncherAdapter = new SnowballLauncherAdapter();
            Call(registerAdapterReflection, snowballLauncherAdapter, new int[] { TileID.SnowballLauncher });

            //Cannon
            CannonAdapter cannonAdapter = new CannonAdapter();
            Call(registerAdapterReflection, cannonAdapter, new int[] { TileID.Cannon });

            //Crystal stand
            CrystalStandAdapter crystalStandAdapter = new CrystalStandAdapter();
            Call(registerAdapterReflection, crystalStandAdapter, new int[] { TileID.ElderCrystalStand });

            //Weapon rack
            WeaponRackAdapter weaponRackAdapter = new WeaponRackAdapter();
            Call(registerAdapterReflection, weaponRackAdapter, new int[] { TileID.WeaponsRack });

            //Omni turret
            OmniTurretAdapter omniTurretAdapter = new OmniTurretAdapter(this);
            Call(registerAdapterReflection, omniTurretAdapter, new int[] { ModContent.TileType<OmniTurretTile>() });

            //Extractinator
            ExtractinatorAdapter extractinatorAdapter = new ExtractinatorAdapter();
            Call(registerAdapterReflection, extractinatorAdapter, new int[] { TileID.Extractinator });

            //Player interface
            PlayerInterfaceAdapter playerInterfaceAdapter = new PlayerInterfaceAdapter(this);
            Call(registerAdapterReflection, playerInterfaceAdapter, new int[] { ModContent.TileType<PlayerInterfaceTile>() });

            if (modMagicStorage != null)
            {
                //Magic storage interface
                MagicStorageInterfaceAdapter magicStorageInterfaceAdapter = new MagicStorageInterfaceAdapter();
                Call(registerAdapterReflection, magicStorageInterfaceAdapter, new int[] { ModContent.TileType<MagicStorageInterfaceTile>() });
            }

            if (modMagicStorageExtra != null)
            {
                //Magic storage extra interface
                MagicStorageExtraInterfaceAdapter magicStorageExtraInterfaceAdapter = new MagicStorageExtraInterfaceAdapter();
                Call(registerAdapterReflection, magicStorageExtraInterfaceAdapter, new int[] { ModContent.TileType<MagicStorageExtraInterfaceTile>() });
            }
        }

        //This needs to be called from SetupRecipies, because chests are made in SetupContent.
        private void LoadChestAdapters()
        {
            ChestAdapter chestAdapter = new ChestAdapter(this);
            List<int> chestTypes = new List<int>();
            for (int i = 0; i < TileLoader.TileCount; i++)
            {
                if (TileID.Sets.BasicChest[i] || TileID.Sets.BasicChestFake[i] || TileLoader.IsDresser(i))
                {
                    chestTypes.Add(i);
                }
            }
            Call(registerAdapterReflection, chestAdapter, chestTypes.ToArray());
        }

        public override void AddRecipes()
        {
            foreach (var item in simpleTileAddRecipequeue)
            {
                item();
            }
            simpleTileAddRecipequeue = null;

            if (modMagicStorage != null)
            {
                //Magic storage interface
                ModRecipe r = new ModRecipe(this);
                r.AddIngredient(modMagicStorage.ItemType("StorageComponent"));
                r.AddRecipeGroup("MagicStorage:AnyDiamond", 1);
                r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
                r.AddTile(TileID.WorkBenches);
                r.SetResult(ItemType("MagicStorageInterfaceItem"));
                r.AddRecipe();
            }

            if (modMagicStorageExtra != null)
            {
                //Magic storage extra interface
                ModRecipe r = new ModRecipe(this);
                r.AddIngredient(modMagicStorageExtra.ItemType("StorageComponent"));
                r.AddRecipeGroup("MagicStorageExtra:AnyDiamond", 1);
                r.AddIngredient(ModContent.ItemType<PneumaticActuatorItem>(), 1);
                r.AddTile(TileID.WorkBenches);
                r.SetResult(ItemType("MagicStorageExtraInterfaceItem"));
                r.AddRecipe();
            }

            LoadChestAdapters();
        }

        public override void PostAddRecipes()
        {
            NetRouter.Init(0);
        }

        public override void Unload()
        {
            NetRouter.Unload();
            UIHooks.Unload();
        }
    }

    public static class ModExtensions
    {
        public static ModItem GetPlaceItem<T>(this Mod mod) where T : SimplePlaceableTile
        {
            SimplePlaceableTile tile = ModContent.GetInstance<T>();
            return tile.PlaceItem;
        }

        public static ModItem GetPlaceItem<T>(this Mod mod, int kind) where T : SimpleTileObject
        {
            SimpleTileObject tile = ModContent.GetInstance<T>();
            return tile.GetPlaceItem(kind);
        }

        public static int PlaceItemType<T>(this Mod mod) where T : SimplePlaceableTile
        {
            return GetPlaceItem<T>(mod).item.type;
        }

        public static int PlaceItemType<T>(this Mod mod, int style) where T : SimpleTileObject
        {
            return GetPlaceItem<T>(mod, style).item.type;
        }
    }
}