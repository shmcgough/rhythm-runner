using System;
using System.Collections.Generic;

namespace RhythmRunner.Audio
{
    // this class is being removed

    class OnsetDetector
    {
        public static readonly int SAMPLE_WINDOW = 1024;
        public static readonly int THRESHOLD_WINDOW = 10;   // 5 frames for before and after 0 - 10 inclusive
        public static readonly float MULTIPLIER = 1.5f;     // Sensitivity of detector

        private List<float> spectralFluxList = new List<float>();
        private List<float> thresholdList = new List<float>();
        private List<float> brokenThresholdFluxList = new List<float>();
        private List<float> peaks = new List<float>();
        private float[] currentSpectrum = new float[SAMPLE_WINDOW];
        private float[] lastSpectrum = new float[SAMPLE_WINDOW];


        /// <summary>
        /// Calculates the fluxuation between the current spectrum and the last spectrum and then adds it to spectralFluxList. Ignores negative values.
        /// </summary>
        /// <param name="spectrum">A spectrum</param>

        public void CalculateSpectralFlux(double[] spectrum)
        {
            lastSpectrum = currentSpectrum;
            currentSpectrum = Array.ConvertAll(spectrum, element => (float)element);
            float flux = 0;

            for (int i = 0; i < spectrum.Length; i++)
            {
                float value = (currentSpectrum[i] - lastSpectrum[i]);
                flux += (value < 0) ? 0 : value;      // ignore negative values
            }

            spectralFluxList.Add(flux);
        }

        public void DoDetection()
        {
            CalculateThreshold();

            CheckForThresholdBreaks();

            FindPeaks();
        }


        private void CalculateThreshold()
        {
            for (int i = 0; i < spectralFluxList.Count; i++)
            {
                int start = Math.Max(0, i - THRESHOLD_WINDOW);
                int end = Math.Min(spectralFluxList.Count - 1, i + THRESHOLD_WINDOW);
                float mean = 0;

                for (int j = start; j <= end; j++)
                    mean += spectralFluxList[j];

                mean /= (end - start);
                thresholdList.Add(mean * MULTIPLIER);
            }
        }

        private void CheckForThresholdBreaks()
        {
            // Fill new array with fluxes that break the threshold
            for (int i = 0; i < thresholdList.Count; i++) /*****/
            {
                if (thresholdList[i] <= spectralFluxList[i])
                    brokenThresholdFluxList.Add(spectralFluxList[i] - thresholdList[i]);
                else
                    brokenThresholdFluxList.Add(0.0f);
            }
        }

        private void FindPeaks()
        {
            // Search for peaks
            // timeOfOccurence = index * (SAMLE_WINDOW / SAMPLE_RATE)
            for (int i = 0; i < brokenThresholdFluxList.Count - 1; i++)
            {
                if (brokenThresholdFluxList[i] > brokenThresholdFluxList[i + 1])
                {
                    peaks.Add(brokenThresholdFluxList[i]);
                }
                else
                {
                    peaks.Add(0.0f);
                }
            }
        }        

        public List<float> Peaks
        {
            get { return peaks; }
            set { peaks = value; }
        }
    }
}
