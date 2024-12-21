using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace MechTransfer.UI
{
    public class FilterHoverUI : UIState
    {
        public bool visible = false;

        private UIPanel panel;
        private UIText itemText;
        private UIText titleText;
        private Item[] fakeInv = new Item[11];

        private const string NotSetTextKey = "Mods.MechTransfer.UI.Hover.NotSet";

        public override void OnInitialize()
        {
            fakeInv[10] = new Item();

            titleText = new UIText(Language.GetTextValue(NotSetTextKey));
            titleText.Left.Set(5, 0);
            titleText.Top.Set(5, 0);
            Append(titleText);

            panel = new UIPanel();
            panel.Left.Set(0, 0);
            panel.Top.Set(25, 0);
            panel.Width.Set(200, 0);
            panel.Height.Set(TextureAssets.InventoryBack.Value.Height * 0.5f * Main.UIScale + 10, 0);

            itemText = new UIText(Language.GetTextValue(NotSetTextKey));
            itemText.Left.Set(TextureAssets.InventoryBack.Value.Width * 0.5f * Main.UIScale, 0);
            itemText.Top.Set(0, 0);
            panel.Append(itemText);

            Append(panel);
        }

        public void Display(Item type, string text, Color textColor)
        {
            visible = true;

            titleText.SetText(text);
            titleText.TextColor = textColor;

            fakeInv[10] = type;
            itemText.SetText(type.IsAir ? Language.GetTextValue(NotSetTextKey) : fakeInv[10].Name);
            itemText.TextColor = type.IsAir ? Color.Red : ItemRarity.GetColor(fakeInv[10].rare);
            panel.Width.Pixels = itemText.MinWidth.Pixels + TextureAssets.InventoryBack.Value.Width * 0.5f * Main.UIScale + 20;

            Vector2 pos = Vector2.Transform(Main.MouseScreen, Main.GameViewMatrix.TransformationMatrix);

            Left.Pixels = (pos.X + 10) / Main.UIScale;
            Top.Pixels = (pos.Y + 10) / Main.UIScale;

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;

            base.Draw(spriteBatch);

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 0.5f;
            ItemSlot.Draw(spriteBatch, fakeInv, ItemSlot.Context.InventoryItem, 10, new Vector2(Left.Pixels + 5, Top.Pixels + 30));
            Main.inventoryScale = oldScale;

            visible = false;
        }
    }
}