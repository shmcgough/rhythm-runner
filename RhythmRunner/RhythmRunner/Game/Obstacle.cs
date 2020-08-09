using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RhythmRunner.Game
{
    class Obstacle
    {
        private Texture2D blockSprite;
        private Texture2D wallSprite;
        private Texture2D hoverBlockSprite;
        private bool wasCollidedWith = false;
        private Texture2D sprite;
        private int spriteIndex;
        private Vector2 position;

        private float velocity = 600f;

        public static readonly int SCROLL_SPEED = 10;
        public static float SPAWN_POSITION;
        public static readonly int GROUND = 0;
        public static readonly int AIR = 1;
        public static readonly int WALL = 2;

        public Obstacle(int spriteNum)
        {
            switch (spriteNum)
            {
                case 0:
                    sprite = blockSprite;
                    break;
                case 1:
                    sprite = wallSprite;
                    break;
                case 2:
                    sprite = hoverBlockSprite;
                    break;
            }
        }

        public Obstacle(int spriteNum, Vector2 position)
        {
            switch (spriteNum)
            {
                case 0:
                    sprite = blockSprite;
                    spriteIndex = 0;
                    break;
                case 1:
                    sprite = wallSprite;
                    spriteIndex = 1;
                    break;
                case 2:
                    sprite = hoverBlockSprite;
                    spriteIndex = 2;
                    break;
            }

            this.position = position;
            SPAWN_POSITION = this.position.X;
        }

        public Obstacle(Texture2D obstacle, Vector2 position, int spriteNum)
        {
            sprite = obstacle;
            this.position = position;
            spriteIndex = spriteNum;
            SPAWN_POSITION = this.position.X;
        }

        public void LoadContent(ContentManager content)
        {
            blockSprite = content.Load<Texture2D>("Sprite/obstacle");
            wallSprite = content.Load<Texture2D>("Sprite/obstacle");
            hoverBlockSprite = content.Load<Texture2D>("Sprite/wall");
        }

        public void ResetPosition()
        {
            position.X = SPAWN_POSITION;
        }

        public void Update(GameTime gameTime)
        {
            if (!wasCollidedWith)
            {
                float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                position.X -= velocity * time;

                CheckCollision();
            }
            ScoreTracker.Update(gameTime);
            if (position.X + sprite.Width < 0)
            {
                if (!wasCollidedWith)
                {
                    ScoreTracker.IncrementScore();
                }
                wasCollidedWith = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (!wasCollidedWith && position.X < 1280 + sprite.Width)
                sb.Draw(sprite, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // helpers

        private void CheckCollision()
        {
            Rectangle playerRectangle = new Rectangle((int)Player.position.X + 33, (int)Player.position.Y, 33, (int)Player.sprite.Height);
            Rectangle kickRectangle = new Rectangle((int)Player.position.X + 66, (int)Player.position.Y + 80, 33, (int)Player.sprite.Height);
            Rectangle obstacleRectangle = new Rectangle((int)position.X, (int)position.Y, (int)sprite.Width, (int)sprite.Height - 25);

            if (Player.IsSliding)
            {
                playerRectangle = new Rectangle((int)Player.position.X, (int)Player.position.Y + 86, (int)Player.sprite.Height, 33);
            }
            
            /**************/

            if (spriteIndex == 1 && Player.HasKicked && kickRectangle.Intersects(obstacleRectangle)) // check if wall was kicked
            {
                if (!wasCollidedWith)
                {
                    ScoreTracker.IncrementObstaclesHit();
                }
                wasCollidedWith = true;
            }
            else if (playerRectangle.Intersects(obstacleRectangle))
            {
                Player.HasCollided = true;

                if (!wasCollidedWith)
                {
                    ScoreTracker.IncrementObstaclesHit();
                }

                wasCollidedWith = true;                
            }
        }
    }
}