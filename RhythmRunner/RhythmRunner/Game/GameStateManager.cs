using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhythmRunner.Game
{
    class GameStateManager
    {
        private static GameState currentGameState;

        public enum GameState
        {
            Menu,
            FileBrowser,
            HowToPlay,
            Playing,
            ScoreScreen,
            Exit
        };
        
        public static GameState CurrentGameState
        {
            get { return currentGameState; }
            set { currentGameState = value; }
        }
    }
}
