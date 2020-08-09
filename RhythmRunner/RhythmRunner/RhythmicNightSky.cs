using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RhythmRunner
{
    // this class is being removed
    class RhythmicNightSky
    {
        MediaLibrary _library;
        GraphicsDevice graphicsDevice;
        Song _song;
        Texture2D _skyTexture;
        VisualizationData _visData;
        Color[] _colorData;

        public RhythmicNightSky(GraphicsDevice graphicsDevice, Song song)
        {
            MediaPlayer.IsVisualizationEnabled = true;
            _visData = new VisualizationData();
            _library = new MediaLibrary();
            //_song = library.Songs[982]; // some random song for testing
            _song = song;
            
            this.graphicsDevice = graphicsDevice;
            
            _skyTexture = new Texture2D(graphicsDevice, _visData.Frequencies.Count, _visData.Samples.Count);
            _colorData = new Color[_skyTexture.Height * _skyTexture.Width];
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                MediaPlayer.Play(_song);

            
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D line)
        {
            // Had to use the 7 parameter draw here to get access to the layerDepth arguement to allow this to be put behind all the other sprites
            //spriteBatch.Draw(_skyTexture, graphicsDevice.Viewport.Bounds, null, Color.White, 0.0f, new Vector2(0), SpriteEffects.None, 0.02f);


            
        }
    }
}
