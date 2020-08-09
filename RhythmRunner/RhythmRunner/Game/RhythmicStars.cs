using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace RhythmRunner.Game
{
    class RhythmicStars
    {
        private List<Point> starPointsList;                // A collection of points, which represent the position of the stars
        private Random random;

        private Point starPoint;                           // A point object that is used to represent where a star will appear on screen
        private Point randPoint;                           // A point object that is used to hold the random point that is generated

        private Rectangle boundaries;                      // The boundary that the stars will appear within
        private Texture2D star;                            // The star sprite

        private VisualizationData visData;

        /// <summary>
        /// Stars constructor. Instantiates fields. Generates a list of stars and stores them in _starPointsList
        /// </summary>
        /// <param name="boundaries">The Texture2D object that the stars are to be drawn on</param>
        /// <param name="star">The star sprite</param>
        /// <param name="numOfStars">Number of stars to generate, cannot be greater than number of samples being processed in a cycle</param>
        
        public RhythmicStars(Rectangle boundaries, int numOfStars)
        {
            MediaPlayer.IsVisualizationEnabled = true;
            visData = new VisualizationData();
            starPointsList = new List<Point>();
            random = new Random();
            this.boundaries = boundaries;

            for (int i = 0; i < numOfStars; i++)
            {
                starPoint = GenerateRandomStar();
                starPointsList.Add(starPoint);
            }
        }

        public void LoadContent(ContentManager content)
        {
            star = content.Load<Texture2D>("Sprites/bigStar");
        }

        /// <summary>
        /// Returns a point in the ranges of boundaries Left, Right, Top and Bottom bounds
        /// </summary>
        /// <returns>A point object assigned with a random X and Y value within boundaries bounds</returns>
        
        private Point GenerateRandomStar()
        {
            int randX = random.Next(boundaries.Left, boundaries.Right);
            int randY = random.Next(boundaries.Top, boundaries.Bottom);
            randPoint = new Point(randX, randY);

            return randPoint;
        }

        // Update() and Draw() need to be called in the main game loops

        public void Update()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.GetVisualizationData(visData);
            }

            for (int i = 0; i < starPointsList.Count; i++)
            {
                if (starPointsList[i].X < 0)
                {
                    // If a star leaves the left side of the screen make it reapear on the other side somewhere else on the Y axis
                    starPointsList[i] = new Point(boundaries.Right, random.Next(boundaries.Top, boundaries.Bottom));
                }
                else
                {
                    // Move all stars one X coordinate to the left, need to slow this down somehow
                    starPointsList[i] = new Point(starPointsList[i].X - 1, starPointsList[i].Y);
                }                
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                for (int i = 0; i < starPointsList.Count; i++)
                {
                    if (visData.Samples[i] >= 0.65f)
                        sb.Draw(star, new Rectangle(starPointsList[i].X, starPointsList[i].Y, star.Width, star.Height), null, Color.White, 0.0f, new Vector2(0), SpriteEffects.None, 0.2f);
                    else if (visData.Samples[i] >= 0.5f)
                        sb.Draw(star, new Rectangle(starPointsList[i].X, starPointsList[i].Y, star.Width, star.Height), null, Color.Yellow, 0.0f, new Vector2(0), SpriteEffects.None, 0.2f);
                    else if(visData.Samples[i] >= 0.3f)
                        sb.Draw(star, new Rectangle(starPointsList[i].X, starPointsList[i].Y, star.Width, star.Height), null, Color.White, 0.0f, new Vector2(0), SpriteEffects.None, 0.2f);
                    else
                        sb.Draw(star, new Rectangle(starPointsList[i].X, starPointsList[i].Y, 1, 1), new Rectangle(0, 0, 1, 1), Color.White, 0.0f, new Vector2(0), SpriteEffects.None, 0.2f);
                }
            }
            else
            {
                for (int i = 0; i < starPointsList.Count; i++)
                    sb.Draw(star, new Rectangle(starPointsList[i].X, starPointsList[i].Y, 1, 1), new Rectangle(0, 0, 1, 1), Color.White, 0.0f, new Vector2(0), SpriteEffects.None, 0.2f);
            }
        }
    }
}