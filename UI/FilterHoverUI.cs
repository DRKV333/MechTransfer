using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace MechTransfer.UI
{
    public class FilterHoverUI : UIState
    {
        public bool visible = false;

        private UIPanel panel;
        private UIText itemText;
        private UIText titleText;
        private Item displayItem = new Item();

        public override void OnInitialize()
        {
            titleText = new UIText("Not set");
            titleText.Left.Set(5, 0);
            titleText.Top.Set(5, 0);
            Append(titleText);

            panel = new UIPanel();
            panel.Left.Set(0, 0);
            panel.Top.Set(25, 0);
            panel.Width.Set(200, 0);
            panel.Height.Set(Main.inventoryBackTexture.Height * 0.5f * Main.UIScale + 10, 0);

            itemText = new UIText("N/A");
            itemText.Left.Set(Main.inventoryBackTexture.Width * 0.5f * Main.UIScale, 0);
            itemText.Top.Set(0, 0);
            panel.Append(itemText);

            Append(panel);
        }

        public void Display(int type, string text, Color textColor)
        {
            visible = true;

            titleText.SetText(text);
            titleText.TextColor = textColor;

            displayItem.SetDefaults(type);
            itemText.SetText(type == 0 ? "Not set" : displayItem.Name);
            itemText.TextColor = type == 0 ? Color.Red : ItemRarity.GetColor(displayItem.rare);
            panel.Width.Pixels = itemText.MinWidth.Pixels + Main.inventoryBackTexture.Width * 0.5f * Main.UIScale + 20;

            Left.Pixels = Main.mouseX + 10;
            Top.Pixels = Main.mouseY + 10;

            Recalculate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;

            base.Draw(spriteBatch);

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 0.5f;
            ItemSlot.Draw(spriteBatch, ref displayItem, ItemSlot.Context.InventoryItem, new Vector2(Left.Pixels + 5, Top.Pixels + 30));
            Main.inventoryScale = oldScale;

            visible = false;
        }
    }
}