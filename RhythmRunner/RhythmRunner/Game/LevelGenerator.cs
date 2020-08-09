using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace RhythmRunner.Game
{
    class LevelGenerator
    {
        public static readonly float SAMPLE_RATE = 44100;
        public static readonly float SAMPLE_WINDOW = 1024;
        public static readonly int SCROLL_SPEED = 10;
        public static readonly int REACTION_BUFFER = 450;                   // Leway of where the beats time up with on the screen
        public static readonly int SECOND_OF_PIXELS = SCROLL_SPEED * 60;    // how many pixels the object moves multiplied by the amount of frames in a second (60).

        private int obstacleCounter = 0;
        
        private double timer = 0;
        private double interval = 1;
        public static bool levelHasStarted = false;

        private int currentNumInCountdown = 4;

        private List<Platform> platforms;
        private List<Obstacle> obstacles;

        private Texture2D oneTexture;
        private Texture2D twoTexture;
        private Texture2D threeTexture;
        private Texture2D goTexture;
        private Texture2D currentlyDrawnCountdown;

        public LevelGenerator(List<float> peaks, Texture2D plat1, Texture2D obstacle, Texture2D wall, Texture2D hoverObstacle)
        {
            // Get rid of first 2-3 seconds of onsets for a grace period
            for (int i = 0; i < 100; i++)
            {
                peaks[i] = 0;
            }

            // Make peaks that are too close together 0.            
            for (int i = 0; i < peaks.Count - 12; i++)
            {
                if (peaks[i] > 0)
                {
                    for (int j = 1; j < 12; j++)
                        peaks[j + i] = 0;
                }
            }

            platforms = new List<Platform>();
            obstacles = new List<Obstacle>();
            Random r = new Random();

            int platformDraws = peaks.Count / 21; // platforms are half a second worth of pixels in length
            
            // Calculate number of platforms neede
            for (int i = 0; i < platformDraws; i++)
            {
                platforms.Add(new Platform(plat1, new Vector2(i * (SECOND_OF_PIXELS / 2), 720 - plat1.Height)));
            }

            // Level gen w/o platform variation
            for (int i = 0; i < peaks.Count; i++)
            {
                if (peaks[i] > 0)
                {
                    float obstacleOccurenceInPixels = ((float)i * (SAMPLE_WINDOW / SAMPLE_RATE)) * SECOND_OF_PIXELS + REACTION_BUFFER;

                    if (obstacleCounter == 0)
                    {
                        obstacles.Add(new Obstacle(obstacle, new Vector2(obstacleOccurenceInPixels - 25, 720 - (plat1.Height + obstacle.Height) + 25), 0));
                        obstacleCounter++;
                    }
                    else if (obstacleCounter == 3)
                    {
                        obstacleCounter = 0;
                        obstacles.Add(new Obstacle(obstacle, new Vector2(obstacleOccurenceInPixels - 25, 720 - (plat1.Height + obstacle.Height) + 25), 0));
                        obstacleCounter++;
                    }
                    else
                    {
                        int random = r.Next(1, 3);
                        if (random == 1)
                        {
                            obstacles.Add(new Obstacle(hoverObstacle, new Vector2(obstacleOccurenceInPixels - 35, 720 - (plat1.Height + (obstacle.Height * 3) - 25)), 2));
                            obstacleCounter++;
                        }
                        else
                        {
                            obstacles.Add(new Obstacle(wall, new Vector2(obstacleOccurenceInPixels - 25, 720 - (plat1.Height + wall.Height) + 25), 1));
                            obstacleCounter++;
                        }
                    }
                }
            }
        }

        public LevelGenerator()
        {
            
        }

        public void LoadContent(ContentManager content)
        {
            oneTexture = content.Load<Texture2D>("Textures/1");
            twoTexture = content.Load<Texture2D>("Textures/2");
            threeTexture = content.Load<Texture2D>("Textures/3");
            goTexture = content.Load<Texture2D>("Textures/go");
        }

        public void Update(GameTime gameTime)
        {
            //foreach (Platform p in platforms)
            //{
            //    p.Update();
            //}
            if (!levelHasStarted)
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds;

                if (timer >= interval)
                {
                    currentNumInCountdown--;
                    levelHasStarted = (currentNumInCountdown == 0) ? true : false;
                    timer = 0;
                }
            }
            else
            {                
                if (MediaPlayer.State == MediaState.Playing)
                {
                    foreach (Obstacle o in obstacles)
                    {
                        o.Update(gameTime);
                    }
                    
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            //foreach (Platform p in platforms)
            //{
            //    p.Draw(sb);
            //}
            if (!levelHasStarted)
            {
                switch (currentNumInCountdown)
                {
                    case 4:
                    case 3:
                        currentlyDrawnCountdown = threeTexture;
                        break;
                    case 2:
                        currentlyDrawnCountdown = twoTexture;
                        break;
                    case 1:
                        currentlyDrawnCountdown = oneTexture;
                        break;
                    case 0:
                        currentlyDrawnCountdown = goTexture;
                        break;
                }
                sb.Draw(currentlyDrawnCountdown, new Vector2(480f, 250f), null, Color.Red, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 0.3f);
            }
            else
            {
                foreach (Obstacle o in obstacles)
                {
                    o.Draw(sb);
                }
            }
        }

        public static bool LevelHasStarted
        {
            get { return levelHasStarted; }
            set { levelHasStarted = value; }
        }
    }
}
