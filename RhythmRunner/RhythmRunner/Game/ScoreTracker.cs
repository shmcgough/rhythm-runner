using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RhythmRunner.Game
{
    class ScoreTracker
    {
        private static int obstaclesHit = 0;
        private static int score = 0;
        private static Texture2D titleImage;
        private static SpriteFont font;
        private static Song song;
        private static int screenHeight;
        private static int screenWidth;
        private static RhythmicStars stars;
        private static InputState input;

        public static void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("gamefont");
            titleImage = content.Load<Texture2D>("Textures/score");
            song = content.Load<Song>("Audio/04 so long, long songtitles");
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            stars = new RhythmicStars(new Rectangle(0, 0, screenWidth, screenHeight), 200);
            stars.LoadContent(content);
            input = new InputState();
        }

        public static void IncrementObstaclesHit()
        {
            obstaclesHit++;
        }

        public static void IncrementScore()
        {
            if  (Player.HasCollided)
            {
                score += 10;
            }
        }

        public static void Update(GameTime gameTime)
        {
            input.Update(gameTime);
            if (input.IsNewKeyPress(Keys.Enter) || input.IsNewKeyPress(Keys.Escape))
            {
                LevelGenerator.LevelHasStarted = false;
                GameplayScreen.LevelIsGenerated = false;
                GameplayScreen.HasPlayed = false;
                GameStateManager.CurrentGameState = GameStateManager.GameState.Menu;
            }
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(song);
            }

            if (GameStateManager.CurrentGameState == GameStateManager.GameState.ScoreScreen)
            {
                stars.Update();
            } 
        }

        public static void Draw(SpriteBatch sb)
        {
            if (GameStateManager.CurrentGameState == GameStateManager.GameState.Playing)
            {
                sb.DrawString(font, "Score: " + score, Vector2.Zero, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                sb.Draw(titleImage, new Vector2((1280 / 2) - titleImage.Width + 175, 100), Color.Red);
                sb.DrawString(font, "Your Score: " + score, new Vector2(550, 300), Color.White);
                sb.DrawString(font, "Obstacles hit: " + obstaclesHit, new Vector2(550, 350), Color.White);
                stars.Draw(sb);
            }
        }
    }
}
