using System;
using System.Collections.Generic;
using RhythmRunner.Game.MenuSystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace RhythmRunner.Game
{
    class MainMenuScreen
    {
        private GraphicsDeviceManager graphics;
        private SpriteFont font;
        private Texture2D logo;
        private Song titleSong;
        private int screenWidth;
        private RhythmicStars stars;
        private Menu mainMenu;
        private InputState input;

        public MainMenuScreen(GraphicsDeviceManager graphics)
        {
            mainMenu = new Menu();
            mainMenu.Add(new MenuEntry("Browse for Music"));
            mainMenu.Add(new MenuEntry("How to Play"));
            mainMenu.Add(new MenuEntry("Exit"));
            this.graphics = graphics;
            screenWidth = this.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            stars = new RhythmicStars(new Rectangle(0, 0, 1280, 720), 200);
            input = new InputState();
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("gamefont");
            logo = content.Load<Texture2D>("Textures/rr");
            titleSong = content.Load<Song>("Audio/06. Those Galloping Hordes - Tow Or Flax");
            stars.LoadContent(content);
        }

        public void Update(GameTime time)
        {
            input.Update(time);

            if (MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(titleSong);

            mainMenu.Update(time);
            stars.Update();

            if (input.IsNewKeyPress(Keys.Enter))
            {
                if (mainMenu.SelectedEntry == 0)
                {
                    GameStateManager.CurrentGameState = GameStateManager.GameState.FileBrowser;
                }
                else if (mainMenu.SelectedEntry == 1)
                {
                    GameStateManager.CurrentGameState = GameStateManager.GameState.HowToPlay;
                }
                else if (mainMenu.SelectedEntry == 2)
                {
                    GameStateManager.CurrentGameState = GameStateManager.GameState.Exit;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            mainMenu.Draw(sb, font, new Vector2((screenWidth / 2) + 100, logo.Height + 250));
            sb.Draw(logo, new Vector2((screenWidth / 2) - (logo.Width / 2) + 75, 200f), null, Color.Red, 0f, new Vector2(0), 1f, SpriteEffects.None, 0.3f);
            stars.Draw(sb);
        }
    }
}
