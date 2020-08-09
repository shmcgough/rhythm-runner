using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RhythmRunner.Game.MenuSystem;
using RhythmRunner.Audio;
using System.Reflection;

namespace RhythmRunner.Game
{
    class FileBrowserScreen
    {
        private SpriteFont font;

        private string[] directories;
        private string[] fileEntries;

        private Menu fileBrowser;
        private InputState input;

        private string currentDirectory;

        private ContentManager content;

        private Song song;

        #region Methods

        public FileBrowserScreen()
        {
            directories = Directory.GetLogicalDrives();
            fileEntries = new string[0];
            fileBrowser = new Menu("Logical Drives");
            input = new InputState();
            currentDirectory = "";
            TrimDriveSlashes();
            PopulateMenu();
        }

        public void LoadContent(ContentManager content)
        {
            this.content = content;
            font = this.content.Load<SpriteFont>("gamefont");
        }

        public void Update(GameTime gameTime)
        {
            fileBrowser.Update(gameTime);
            HandleInput(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {
            fileBrowser.Draw(sb, font, Vector2.Zero);
        }

        private void HandleInput(GameTime gameTime)
        {
            input.Update(gameTime);

            if (input.IsNewKeyPress(Keys.Enter))
            {
                // put in check to see if its mp3 first, if not directory / nothing else should be seen except for directories and mp3s
                string fileName = fileBrowser.MenuEntries[fileBrowser.SelectedEntry].Text;
                if (fileName.EndsWith(".mp3"))
                {
                    // mp3 conversion for data
                    string fileAndDirectory = currentDirectory + "\\" + fileName;
                    AudioDecoder.ConvertMp3(fileAndDirectory, "FINDMEEE.wav");


                    // feed song to xna mediaplayer
                    //Uri uriStreaming = new Uri("file:///" + fileAndDirectory);
                    //song = Song.FromUri("UserChosenSong", uriStreaming);

                    /* 
                     * NOTE
                     * Song.FromUri has a known bug and cant be used when there are spaces in the string.
                     * Below is the hack solution provided to this bug
                     */

                    // feed song to xna mediaplayer
                    var ctor = typeof(Song).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                                                            new[] { typeof(string), typeof(string), typeof(int) }, null);

                    song = (Song)ctor.Invoke(new object[] { "name", "FINDMEEE.wav"/*fileAndDirectory*/, 0 });

                    // exit out of this class and continue the game
                    if (MediaPlayer.State == MediaState.Playing)
                        MediaPlayer.Stop();

                    GameStateManager.CurrentGameState = GameStateManager.GameState.Playing;
                }
                else
                {
                    string selectedItem = fileBrowser.MenuEntries[fileBrowser.SelectedEntry].Text;

                    // No previous directory has been chosen and users first selection is NOT ..
                    if (currentDirectory.CompareTo("") == 0 && selectedItem.CompareTo("..") != 0)
                    {
                        currentDirectory = selectedItem + "\\";
                        fileBrowser.Title = currentDirectory;
                    }
                    else if (selectedItem.CompareTo("..") != 0)
                    {
                        // If it is a drive that was selected do not include slash
                        if (currentDirectory.Length == 3)
                            currentDirectory += selectedItem;
                        // include slash if not a drive letter
                        else 
                            currentDirectory += "\\" + selectedItem;

                        fileBrowser.Title = currentDirectory;
                    }

                    // If user chose to go to parent directory
                    if (selectedItem.CompareTo("..") == 0)
                    {
                        // If parent drive does not exist show logical drives
                        if (Path.GetFileName(currentDirectory).CompareTo("") == 0)
                        {
                            directories = Directory.GetLogicalDrives();
                            TrimDriveSlashes();
                            fileBrowser.Title = "Logical Drives";
                            fileEntries = new string[0];
                        }
                        else // go back a directory
                        {
                            currentDirectory = Path.GetDirectoryName(currentDirectory);
                            fileBrowser.Title = currentDirectory;
                        }
                    }

                    // If currentDirectory is not empty, repopulate the 2 arrays
                    if (currentDirectory.CompareTo("") != 0)
                    {
                        try
                        {
                            directories = Directory.GetDirectories(currentDirectory);
                            fileEntries = Directory.GetFiles(currentDirectory, "*.mp3");
                        }
                        catch (IOException exp)
                        {
                            Console.WriteLine(exp.ToString());
                        }
                    }
                    
                    // Repopulate the arrays with now correct information

                    if (fileBrowser.Title.CompareTo("Logical Drives") == 0)
                    {
                        directories = Directory.GetLogicalDrives();
                        TrimDriveSlashes();
                        currentDirectory = "";
                    }
                    else
                    {
                        for (int i = 0; i < directories.Length; i++ )
                        {
                            string directory = Path.GetFileName(directories[i]);
                            directories[i] = directory;                      
                        }
                    }

                    for (int i = 0; i < fileEntries.Length; i++)
                    {
                        string fileEntry = Path.GetFileName(fileEntries[i]);
                        fileEntries[i] = fileEntry;
                    }
                }

                PopulateMenu();
            }
            else if (input.IsNewKeyPress(Keys.Escape))
            {
                GameStateManager.CurrentGameState = GameStateManager.GameState.Menu;
            }
        }
        #endregion

        #region Properties

        public Song Song
        {
            get { return song; }
            set { song = value; }
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Populates menu with information from directories and fileEntries. Will first clear menu before adding menu entries.
        /// </summary>
        
        private void PopulateMenu()
        {
            fileBrowser.Clear();
            fileBrowser.Add(new MenuEntry(".."));

            for (int i = 0; i < directories.Length; i++)
            {
                fileBrowser.Add(new MenuEntry(directories[i]));
            }

            for (int i = 0; i < fileEntries.Length; i++)
            {
                fileBrowser.Add(new MenuEntry(fileEntries[i]));
            }
        }

        /// <summary>
        /// Removes the trailing '\' at the end of the drive names.
        /// This is done to not ruin the rest of the classes directory logic.
        /// </summary>
        
        private void TrimDriveSlashes()
        {
            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = directories[i].TrimEnd(new char[1] { '\\' });
            }
        }

        #endregion
    }
}
