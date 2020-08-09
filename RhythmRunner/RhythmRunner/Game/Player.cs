using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace RhythmRunner.Game
{
    class Player
    {
        public static Texture2D sprite;
        public static Vector2 position;
        public static bool HasCollided;

        double timer = 0;
        double collisionTimer = 0;
        double interval = 140;
        double collisionInterval = 1500;
        int currentFrame = 0;
        Rectangle spriteRect;


        int spriteWidth;
        int spriteHeight;

        private int jumpSpeed;
        private float onGroundYPos;
        private bool hasJumped;

        private static bool isSliding;
        private static bool hasKicked;
        

        private static readonly int JUMP_SPEED = -14; // negative numbers scroll up the screen

        public Player(Vector2 position)
        {
            Player.position = position;

            jumpSpeed = 0;
            onGroundYPos = position.Y;
            hasJumped = false;
            isSliding = false;
            hasKicked = false;
            HasCollided = false;
        }

        public void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>("Sprites/spritesheet");
            spriteWidth = sprite.Width / 5;
            spriteHeight = sprite.Height;
            spriteRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
        }

        public void Update(GameTime gameTime)
        {
            if (hasKicked)
            {
                currentFrame = 2;
            }
            else if (isSliding)
            {
                currentFrame = 4;

            }
            else if (hasJumped)
            {
                currentFrame = 3;
            }
            else
            {
                timer += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timer >= interval)
                {
                    timer = 0;
                    if (currentFrame < 1)
                        currentFrame++;
                    else
                        currentFrame = 0;
                }
            }

            spriteRect.X = currentFrame * spriteWidth;

            UpdateInput();

            if (HasCollided)
            {
                collisionTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (collisionTimer >= collisionInterval)
                {
                    collisionTimer = 0;
                    HasCollided = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = HasCollided ? Color.Red : Color.White;
            if (isSliding)
            {
                spriteBatch.Draw(sprite,
                                    new Rectangle((int)(position.X), (int)(position.Y + spriteWidth + 30), spriteWidth, spriteHeight),
                                    spriteRect, color, MathHelper.ToRadians(270f), Vector2.Zero, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(sprite, position, spriteRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        #region Properties

        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public static bool IsSliding
        {
            get { return Player.isSliding; }
            set { Player.isSliding = value; }
        }

        public static bool HasKicked
        {
            get { return Player.hasKicked; }
            set { Player.hasKicked = value; }
        }

        #endregion

        #region Helper methods

        private void UpdateInput()
        {
            // Jumping
            if (hasJumped)
            {
                position.Y += jumpSpeed;
                jumpSpeed++;
                if (position.Y >= onGroundYPos)
                {
                    position.Y = onGroundYPos;
                    hasJumped = Keyboard.GetState().IsKeyDown(Keys.Space) ? true : false; // this is so the player can't continuously jump when holding the Space bar
                }
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    hasJumped = true;
                    jumpSpeed = JUMP_SPEED;
                }
            }

            // Sliding            
            isSliding = (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S)) ? true : false;

            // Kick
            hasKicked = (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D)) ? true : false;
        }

        #endregion
    }
}