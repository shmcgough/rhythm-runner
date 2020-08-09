using System;
using NAudio.Wave;

namespace RhythmRunner.Audio
{
    class AudioDecoder
    {
        /// <summary>
        /// Converts an Mp3 file into a wave file.
        /// </summary>
        /// <param name="mp3File">Mp3 path name</param>
        /// <param name="outputFile">The output file name</param>
        
        public static void ConvertMp3(string mp3File, string outputFile)
        {
            Mp3FileReader reader = new Mp3FileReader(mp3File);
            WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
            WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
        }
    }
}
