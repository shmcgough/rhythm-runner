using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RhythmRunner.Game
{
    class HowToPlayScreen
    {
        private static SpriteFont font;
        private static InputState input;

        public static void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("gamefont");
            input = new InputState();
        }

        public static void Update(GameTime gameTime)
        {
            input.Update(gameTime);
            if (input.IsNewKeyPress(Keys.Enter))
            {
                GameStateManager.CurrentGameState = GameStateManager.GameState.Menu;
            }
        }

        public static void Draw(SpriteBatch sb)
        {
            sb.DrawString(font, "Slide under blue blocks using S or down on the arrow keys.", Vector2.Zero, Color.White);
            sb.DrawString(font, "Jump over blocks using the spacebar.", new Vector2(0,50), Color.White);
            sb.DrawString(font, "Kick down yellow walls using D or right on the arrow keys.", new Vector2(0, 100), Color.White);
            sb.DrawString(font, "Press Enter to go back to the title screen.", new Vector2(0, 150), Color.White);
        }
    }
}
