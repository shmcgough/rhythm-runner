using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace RhythmRunner.Game
{
    class InputState
    {
        private KeyboardState currentKeyboardState = new KeyboardState();
        private KeyboardState previousKeyboardState = new KeyboardState();

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }

        public bool IsNewKeyPress(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) &&
                    previousKeyboardState.IsKeyUp(key));
        }
    }
}
