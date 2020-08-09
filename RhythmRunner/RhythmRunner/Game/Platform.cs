using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RhythmRunner.Game
{
    class Platform
    {
        public static readonly int SCROLL_SPEED = 10;

        private Texture2D sprite;
        private Vector2 position;


        public Platform()
        {

        }

        public Platform(Texture2D sprite)
        {
            this.sprite = sprite;
        }

        public Platform(Texture2D sprite, Vector2 position)
        {
            this.sprite = sprite;
            this.position = position;
        }

        public void Update()
        {
            position.X -= SCROLL_SPEED;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.02f);
        }        
        
        public Vector2 Position
        {
            set { position = value; }
        }
    }
}
