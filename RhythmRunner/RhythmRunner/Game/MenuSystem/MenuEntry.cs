using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RhythmRunner.Game.MenuSystem
{
    class MenuEntry
    {
        private string text;
        private Vector2 position;
        private SpriteFont font; 

        public MenuEntry(string text)
        {
            this.text = text;
        }

        public void Draw(bool isSelected, SpriteBatch sb)
        {
            Color color = isSelected ? Color.Red : Color.White;
            sb.DrawString(font, text, position, color);
        }

        public int GetWidth()
        {
            return Font.LineSpacing;
        }

        public int GetHeight()
        {
            return (int)Font.MeasureString(Text).X;
        }

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
