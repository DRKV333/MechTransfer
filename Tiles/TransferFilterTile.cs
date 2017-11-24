using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MechTransfer.Tiles
{
    public class TransferFilterTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TransferFilterTileEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);

            drop = mod.ItemType("TransferFilterItem");
            AddMapEntry(new Color(200, 200, 200));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            mod.GetTileEntity<TransferFilterTileEntity>().Kill(i, j);
        }

        public override void RightClick(int i, int j)
        {
            if (!Main.LocalPlayer.HeldItem.IsAir)
            {
                int id = mod.GetTileEntity<TransferFilterTileEntity>().Find(i, j);
                if (id != -1)
                {
                    TransferFilterTileEntity tileEntity = (TransferFilterTileEntity)TileEntity.ByID[id];
                    tileEntity.ItemId = Main.LocalPlayer.HeldItem.type;
                    tileEntity.SyncData();
                }
            }
        }

        public override void MouseOverFar(int i, int j)
        {
            DisplayTooltip(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            DisplayTooltip(i, j);
            Main.LocalPlayer.noThrow = 2;
        }

        public void DisplayTooltip(int i, int j)
        {
            int id = mod.GetTileEntity<TransferFilterTileEntity>().Find(i, j);
            if (id == -1)
                return;
            TransferFilterTileEntity entity = (TransferFilterTileEntity)TileEntity.ByID[id];
            
            ((MechTransfer)mod).filterHoverUI.Display(entity.ItemId, "Filter:", Color.White);
        }
    }
}