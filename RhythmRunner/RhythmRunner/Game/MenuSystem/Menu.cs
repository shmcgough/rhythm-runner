using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RhythmRunner.Game.MenuSystem
{
    class Menu
    {
        private List<MenuEntry> menuEntries;
        private string title;

        private int selectedEntry;
        private InputState input;
        
        public Menu()
        {
            menuEntries = new List<MenuEntry>();
            selectedEntry = 0;
            title = "";
            input = new InputState();
        }

        public Menu(string title)
        {
            menuEntries = new List<MenuEntry>();
            selectedEntry = 0;
            this.title = title;
            input = new InputState();
        }

        public void Update(GameTime time)
        {
            input.Update(time);

            if (input.IsNewKeyPress(Keys.W) || input.IsNewKeyPress(Keys.Up))
                selectedEntry--;

            if (input.IsNewKeyPress(Keys.S) || input.IsNewKeyPress(Keys.Down))
                selectedEntry++;
            
            if (selectedEntry < 0)
                selectedEntry = menuEntries.Count - 1;

            if (selectedEntry >= menuEntries.Count)
                selectedEntry = 0;
        }

        public void Add(MenuEntry menuEntry)
        {
            menuEntries.Add(menuEntry);
        }

        public void Clear()
        {
            menuEntries.Clear();
        }

        public void Draw(SpriteBatch sb, SpriteFont font, Vector2 position)
        {
            sb.DrawString(font, title, position, Color.White);
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = (i == selectedEntry);
                menuEntries[i].Position = new Vector2(position.X, position.Y + (i * 25) + 25);
                menuEntries[i].Font = font;
                menuEntries[i].Draw(isSelected, sb);
            }
        }

        public List<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
            set { menuEntries = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public int SelectedEntry
        {
            get { return selectedEntry; }
            set { selectedEntry = value; }
        }
    }
}
