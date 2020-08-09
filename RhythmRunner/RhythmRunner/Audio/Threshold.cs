using System;
using System.Collections.Generic;

namespace RhythmRunner.Audio
{
    class Threshold
    {
        private static int thresholdWindowSize;
        private static float multiplier;
        private List<float> thresholds;        

        public Threshold(int thresholdWindowSize, float multiplier)
        {
            Threshold.thresholdWindowSize = thresholdWindowSize;
            Threshold.multiplier = multiplier;
            thresholds = new List<float>();
        }

        public void CalculateThreshold(List<float> spectralFluxes)
        {
            for (int i = 0; i < spectralFluxes.Count; i++)
            {
                int start = Math.Max(0, i - thresholdWindowSize);
                int end = Math.Min(spectralFluxes.Count - 1, i + thresholdWindowSize);
                float mean = 0;

                for (int j = start; j <= end; j++)
                    mean += spectralFluxes[j];

                mean /= (end - start);
                thresholds.Add(mean * multiplier);
            }
        }

        public static int ThresholdWindowSize
        {
            get { return Threshold.thresholdWindowSize; }
            set { Threshold.thresholdWindowSize = value; }
        }

        public List<float> Thresholds
        {
            get { return thresholds; }
            set { thresholds = value; }
        }

        public static float Multiplier
        {
            get { return Threshold.multiplier; }
            set { Threshold.multiplier = value; }
        }
    }
}
