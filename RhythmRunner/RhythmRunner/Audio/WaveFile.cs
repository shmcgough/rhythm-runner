using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;

namespace RhythmRunner.Audio
{
    class WaveFile
    {
        #region Fields
        // Wave data structure
        // The "RIFF" chunk descriptor
        private char[] chunkID;                     // Contains the letters "RIFF" in ASCII form
        private long chunkSize;                     // 36 + SubChunk2Size. This is the size of the rest of the chunk following this number.
        private char[] format;                      // Contains the letters "WAVE"

        // The "fmt " sub-chunk
        private char[] subChunk1ID;                 // Contains the letters "fmt "
        private int subChunk1Size;                  // 16 for PCM.  This is the size of the rest of the Subchunk which follows this number.
        private short audioFormat;                  // PCM = 1 (i.e. Linear quantization) Values other than 1 indicate some form of compression.                               
        private short numChannels;                  // Mono = 1, Stereo = 2, etc.
        private int sampleRate;                     // 8000, 44100, etc.
        private int byteRate;                       // == SampleRate * NumChannels * BitsPerSample / 8
        private short blockAlign;                   // == NumChannels * BitsPerSample / 8.The number of bytes for one sample including all channels. 
        private short bitsPerSample;                // 8 bits = 8, 16 bits = 16, etc.

        //The "data" sub-chunk
        private char[] subChunk2ID;                 // Contains the letters "data"
        private int subChunk2Size;                  // == NumSamples * NumChannels * BitsPerSample / 8. This is the number of bytes in the data.
        private float[] samples;                       // Will be used to hold the audio samples.

        // Not part of the wave data structure
        private float[] leftSamples;                // Will contain the left channel audio samples
        private float[] rightSamples;               // Will contain the right channel audio samples

        // Testing variables
        private float leftSample;
        private float rightSample;

        #endregion
        
        public WaveFile()
        {
            subChunk1ID = new char[4];
            format = new char[4];
            subChunk2ID = new char[4];
        }

        /// <summary>
        /// Reads the first 44 bytes of the passed in wave file. This is the header of the wave file and does not contain any audio data.
        /// </summary>
        /// <param name="path">The path to the wave file</param>

        public void ReadWaveHeader(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            chunkID = new char[4] { 'R', 'I', 'F', 'F' };
            chunkSize = fs.Length;
            format = new char[4] { 'W', 'A', 'V', 'E' };
            subChunk1ID = new char[4] { 'f', 'm', 't', ' ' };
            fs.Position = 16;
            subChunk1Size = br.ReadInt32();
            audioFormat = br.ReadInt16();
            numChannels = br.ReadInt16();
            sampleRate = br.ReadInt32();
            byteRate = br.ReadInt32();
            blockAlign = br.ReadInt16();
            fs.Position = 34;
            bitsPerSample = br.ReadInt16();
            subChunk2ID = new char[4] { 'd', 'a', 't', 'a' };
            subChunk2Size = (int)fs.Length - 44;
        }

        /// <summary>
        /// Reads the wave audio data, beginning at position 44 in the filestream as this skips the wave header.
        /// </summary>
        /// <param name="path">The path to the wave file</param>
        
        public void ReadWaveData(string path)
        {
            WaveFileReader audioFileReader = new WaveFileReader(path);
            
            samples = new float[subChunk2Size / 4];
            leftSamples = new float[subChunk2Size / 4];
            rightSamples = new float[subChunk2Size / 4];
            
            for (int i = 0; i < samples.Length; i++)
            {
                ReadFloat(audioFileReader, out leftSample, out rightSample);
                if (i > 9) // skips reading the header portion of the audio data
                {
                    leftSamples[i] = leftSample;
                    rightSamples[i] = rightSample;
                }
            }
        }

        public bool ReadFloat(WaveFileReader waveFileReader, out float sampleValueLeft, out float sampleValueRight)
        {
            if (waveFileReader.WaveFormat.BitsPerSample == 16)
            {
                byte[] value = new byte[4];
                int read = waveFileReader.Read(value, 0, value.Length);
                
                if (read < value.Length)
                {
                    sampleValueLeft = 0.0f;
                    sampleValueRight = 0.0f;
                    return false;
                }

                sampleValueLeft = (float)BitConverter.ToInt16(value, 0) / 32768f;
                sampleValueRight = (float)BitConverter.ToInt16(value, 2) / 32768f;

                return true;
            }
            else
            {
                sampleValueLeft = 0.0f;
                sampleValueRight = 0.0f;
                return false;
            }
        }


        /// <summary>
        /// Splits a wave files samples that are contained in samples[]. Outputs them to two different arrays, one for the left samples and one for the right samples.
        /// </summary>
        /// <param name="leftSamples">This parameter will contain the left samples that were in data[].</param>
        /// <param name="rightSamples">This parameter will contain the right samples that were in data[].</param>
        
        private void SplitWaveFileSamples(out float[] leftSamples, out float[] rightSamples)
        {
            int halfLengthOfData = samples.Length / 2;
            leftSamples = new float[halfLengthOfData];
            rightSamples = new float[halfLengthOfData];

            for (int i = 0; i < halfLengthOfData; i++)
            {
                leftSamples[i] = samples[i];
                rightSamples[i] = samples[i + halfLengthOfData];
            }                
        }

        /// <summary>
        /// Takes two float arrays that contain samples from a wave file and averages them together and then returns it as a float array. 
        /// </summary>
        /// <returns>The average of leftSamples and rightSamples. Contains the amplitude values (I think). Range of values are [-1,1].</returns>
        
        public float[] AverageLeftAndRightSamples()
        {
            //SplitWaveFileSamples(out leftSamples, out rightSamples);
            float[] averagedSamples = new float[leftSamples.Length];

            for (int i = 0; i < leftSamples.Length; i++)
                averagedSamples[i] = (leftSamples[i] + rightSamples[i]) / 2;

            return averagedSamples;
        }

        #region Properties

        public char[] ChunkID
        {
            get { return chunkID; }
            set { chunkID = value; }
        }
        public long ChunkSize
        {
            get { return chunkSize; }
            set { chunkSize = value; }
        }
        public char[] Format
        {
            get { return format; }
            set { format = value; }
        }
        public char[] SubChunk1ID
        {
            get { return subChunk1ID; }
            set { subChunk1ID = value; }
        }
        public int SubChunk1Size
        {
            get { return subChunk1Size; }
            set { subChunk1Size = value; }
        }
        public short NumChannels
        {
            get { return numChannels; }
            set { numChannels = value; }
        }
        public short AudioFormat
        {
            get { return audioFormat; }
            set { audioFormat = value; }
        }
        public int SampleRate
        {
            get { return sampleRate; }
            set { sampleRate = value; }
        }
        public int ByteRate
        {
            get { return byteRate; }
            set { byteRate = value; }
        }
        public short BlockAlign
        {
            get { return blockAlign; }
            set { blockAlign = value; }
        }
        public short BitsPerSample
        {
            get { return bitsPerSample; }
            set { bitsPerSample = value; }
        }
        public char[] SubChunk2ID
        {
            get { return subChunk2ID; }
            set { subChunk2ID = value; }
        }
        public int SubChunk2Size
        {
            get { return subChunk2Size; }
            set { subChunk2Size = value; }
        }
        public float[] Samples
        {
            get { return samples; }
            set { samples = value; }
        }

        #endregion
    }
}
