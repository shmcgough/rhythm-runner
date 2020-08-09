using System;
using RhythmRunner.Game;

namespace RhythmRunner
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameplayScreen game = new GameplayScreen())
            {
                game.Run();
            }
        }
    }
#endif
}

