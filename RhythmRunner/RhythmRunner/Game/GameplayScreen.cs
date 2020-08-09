using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NAudio.Dsp;
using System.Diagnostics;
using RhythmRunner.Audio;

namespace RhythmRunner.Game
{
    public class GameplayScreen : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private static int NUM_OF_STARS = 200;
        private static int SAMPLE_WINDOW = 1024;
        private int screenWidth;
        private int screenHeight;

        private MainMenuScreen menuScreen;
        private FileBrowserScreen fileBrowser;
        private Texture2D smallPlatformTexture;
        private Texture2D mediumPlatformTexture;
        private Texture2D largePlatformTexture;
        private Texture2D wallTexture;
        private Texture2D loading;
        private Rectangle sky;
        private Texture2D obstacleSprite;
        private Texture2D hoverObstacleSprite;

        private RhythmicStars stars;

        private Player player;
        
        private LevelGenerator levelGen;

        private WaveFile wave;
        private List<float> onsets = new List<float>();
        public static bool LevelIsGenerated;
        private bool isLoading;
        public static bool HasPlayed = false;

        public GameplayScreen()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // 1280 720 is a 16:9 ratio 
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            graphics.ApplyChanges();

            GameStateManager.CurrentGameState = GameStateManager.GameState.Menu;
            LevelIsGenerated = false;
            isLoading = true;

            menuScreen = new MainMenuScreen(graphics);
            fileBrowser = new FileBrowserScreen();

            screenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;

            base.Initialize();            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            obstacleSprite = Content.Load<Texture2D>("Sprites/obstacle");
            smallPlatformTexture = Content.Load<Texture2D>("Sprites/plat1");
            mediumPlatformTexture = Content.Load<Texture2D>("Textures/ground");
            largePlatformTexture = Content.Load<Texture2D>("Sprites/plat3");
            wallTexture = Content.Load<Texture2D>("Sprites/wall");
            loading = Content.Load<Texture2D>("Textures/loading");
            hoverObstacleSprite = Content.Load<Texture2D>("Sprites/hoverObstacle");

            HowToPlayScreen.LoadContent(Content);
            ScoreTracker.LoadContent(Content);
            fileBrowser.LoadContent(Content);
            menuScreen.LoadContent(Content);            
            
            sky = new Rectangle(0, 0, 1280, 720 - mediumPlatformTexture.Height);
            stars = new RhythmicStars(sky, NUM_OF_STARS);
            stars.LoadContent(Content);            
            
            player = new Player(new Vector2(350, screenHeight - mediumPlatformTexture.Height - 160 + 25));
            player.LoadContent(Content);
        }

        protected override void UnloadContent()
        {

        }

        #region Update Methods

        protected override void Update(GameTime gameTime)
        {
            switch (GameStateManager.CurrentGameState)
            {
                case GameStateManager.GameState.Menu:
                    menuScreen.Update(gameTime);
                    break;
                case GameStateManager.GameState.FileBrowser:
                    fileBrowser.Update(gameTime);
                    break;
                case GameStateManager.GameState.Playing:
                    GenerateLevel();
                    isLoading = false;
                    if (LevelGenerator.LevelHasStarted)
                    {
                        if (MediaPlayer.State != MediaState.Playing && !HasPlayed)
                        {
                            HasPlayed = true;
                            MediaPlayer.Play(fileBrowser.Song);
                        }
                        else if (MediaPlayer.State != MediaState.Playing && HasPlayed)
                        {
                            GameStateManager.CurrentGameState = GameStateManager.GameState.ScoreScreen;
                        }
                    }
                    //if (MediaPlayer.State == MediaState.Playing)
                    //{
                        levelGen.Update(gameTime);
                    //}

                    UpdateInput();
                    stars.Update();
                    player.Update(gameTime);
                    break;
                case GameStateManager.GameState.HowToPlay:
                    HowToPlayScreen.Update(gameTime);
                    break;
                case GameStateManager.GameState.ScoreScreen:
                    ScoreTracker.Update(gameTime);
                    break;
                case GameStateManager.GameState.Exit:
                    this.Exit();
                    break;
            }

            base.Update(gameTime);
        }
        
        private void UpdateInput()
        {
            KeyboardState ks = Keyboard.GetState();

            if(ks.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            
        }

        #endregion

        #region Draw Methods

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            switch (GameStateManager.CurrentGameState)
            {
                case GameStateManager.GameState.Menu:
                    menuScreen.Draw(spriteBatch);
                    break;
                case GameStateManager.GameState.FileBrowser:
                    fileBrowser.Draw(spriteBatch);
                    break;
                case GameStateManager.GameState.Playing:
                    if (isLoading)
                    {
                        spriteBatch.Draw(loading, new Vector2((screenWidth / 2) - loading.Width + 150, (screenHeight / 2) - loading.Height), null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        DrawSprites();
                        DrawScenery();


                        spriteBatch.Draw(mediumPlatformTexture, new Vector2(0, 720 - mediumPlatformTexture.Height), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.02f);
                        spriteBatch.Draw(mediumPlatformTexture, new Vector2(300, 720 - mediumPlatformTexture.Height), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.02f);
                        spriteBatch.Draw(mediumPlatformTexture, new Vector2(600, 720 - mediumPlatformTexture.Height), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.02f);
                        spriteBatch.Draw(mediumPlatformTexture, new Vector2(900, 720 - mediumPlatformTexture.Height), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.02f);
                        spriteBatch.Draw(mediumPlatformTexture, new Vector2(1200, 720 - mediumPlatformTexture.Height), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.02f);
                        

                        //if (MediaPlayer.State == MediaState.Playing)
                        //{
                            levelGen.Draw(spriteBatch);
                        //}

                        ScoreTracker.Draw(spriteBatch);
                    }
                    
                    break;
                case GameStateManager.GameState.HowToPlay:
                    HowToPlayScreen.Draw(spriteBatch);
                    break;
                case GameStateManager.GameState.ScoreScreen:
                    ScoreTracker.Draw(spriteBatch);
                    break;
                case GameStateManager.GameState.Exit:
                    this.Exit();
                    break;
            }
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawScenery()
        {
            stars.Draw(spriteBatch);            
            
        }

        private void DrawSprites()
        {
            player.Draw(spriteBatch);
        }

        #endregion

        private void GenerateLevel()
        {
            if (!LevelIsGenerated)
            {
                //LomontFFT fft = new LomontFFT();
                //FastFourierTransform.FFT(
                //string theWAVE = "D:\\Documents\\College\\4th Year\\FYP\\RhythmRunner\\Game Assets\\music\\testWAVEfile1.wav";
                string theWAVE = "FINDMEEE.wav";                
                wave = new WaveFile();
                wave.ReadWaveHeader(theWAVE);
                wave.ReadWaveData(theWAVE);

                float[] amplitudes = wave.AverageLeftAndRightSamples();
                float[] frequencies = new float[amplitudes.Length];

                float[] firstSlice1024 = new float[SAMPLE_WINDOW];
                float[] secondSlice1024 = new float[SAMPLE_WINDOW];
                float[] combinedSlices = new float[SAMPLE_WINDOW];

                SpectralFlux specFlx = new SpectralFlux(1024);
                Threshold threshold = new Threshold(10, 1.5f);

                int fftIterations = amplitudes.Length / SAMPLE_WINDOW;

                for (int i = 0; i < fftIterations; i++)
                {
                    for (int j = 0; j < SAMPLE_WINDOW; j++)
                    {
                        if (j % 2 == 0)
                        {
                            firstSlice1024[j] = amplitudes[j + (SAMPLE_WINDOW * i)];
                            secondSlice1024[j] = 0;
                        }
                        else
                        {
                            firstSlice1024[j] = 0;
                            secondSlice1024[j] = amplitudes[j + (SAMPLE_WINDOW * i)];
                        }
                    }

                    fft.FFT(firstSlice1024, true);
                    fft.FFT(secondSlice1024, true);

                    for (int j = 0; j < SAMPLE_WINDOW; j++)
                    {
                        if (j % 2 == 0)
                        {
                            frequencies[j + (SAMPLE_WINDOW * i)] = firstSlice1024[j];
                        }
                        else
                        {
                            frequencies[j + (SAMPLE_WINDOW * i)] = secondSlice1024[j];
                        }
                    }
                    
                    // combine the slices back together
                    for (int j = 0; j < SAMPLE_WINDOW; j++)
                    {
                        if (j % 2 == 0)
                        {
                            combinedSlices[j] = firstSlice1024[j];
                        }
                        else
                        {
                            combinedSlices[j] = secondSlice1024[j];
                        }
                    }
                    firstSlice1024 = new double[SAMPLE_WINDOW];
                    secondSlice1024 = new double[SAMPLE_WINDOW];
                    specFlx.CalculateSpectralFlux(combinedSlices);
                }

                threshold.CalculateThreshold(specFlx.SpectralFluxes);

                Peak peak = new Peak(threshold.Thresholds, specFlx.SpectralFluxes);

                peak.CheckForThresholdBreaks();
                peak.FindPeaks();

                float onset = 0;
                for (int i = 0; i < peak.Peaks.Count; i++)
                {
                    if (peak.Peaks[i] > 0)
                    {
                        onset = (float)i * ((float)SAMPLE_WINDOW / (float)44100);
                        onsets.Add(onset);
                    } // all needs to be taken out of update
                }

                levelGen = new LevelGenerator(peak.Peaks, mediumPlatformTexture, obstacleSprite, wallTexture, hoverObstacleSprite);
                levelGen.LoadContent(Content);
                LevelIsGenerated = true;
                isLoading = false;
            }
        }

    }
}