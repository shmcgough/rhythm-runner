using System;
using System.Collections.Generic;

namespace RhythmRunner.Audio
{
    class Peak
    {
        private List<float> thresholds;
        private List<float> spectralFluxes;
        private List<float> thresholdBreakers;
        private List<float> peaks;

        public Peak(List<float> thresholds, List<float> spectralFluxes)
        {
            this.thresholds = thresholds;
            this.spectralFluxes = spectralFluxes;
            thresholdBreakers = new List<float>();
            peaks = new List<float>();
        }

        public void CheckForThresholdBreaks()
        {
            // Fill new array with fluxes that break the threshold
            for (int i = 0; i < thresholds.Count; i++) /*****/
            {
                if (thresholds[i] <= spectralFluxes[i])
                    thresholdBreakers.Add(spectralFluxes[i] - thresholds[i]);
                else
                    thresholdBreakers.Add(0.0f);
            }
        }

        public void FindPeaks()
        {
            // timeOfOccurence = index * (SAMLE_WINDOW / SAMPLE_RATE)
            for (int i = 0; i < thresholdBreakers.Count - 1; i++)
            {
                if (thresholdBreakers[i] > thresholdBreakers[i + 1])
                {
                    peaks.Add(thresholdBreakers[i]);
                }
                else
                {
                    peaks.Add(0.0f);
                }
            }
        }

        #region Properties

        public List<float> Thresholds
        {
            get { return thresholds; }
            set { thresholds = value; }
        }

        public List<float> SpectralFluxes
        {
            get { return spectralFluxes; }
            set { spectralFluxes = value; }
        }

        public List<float> ThresholdBreakers
        {
            get { return thresholdBreakers; }
            set { thresholdBreakers = value; }
        }

        public List<float> Peaks
        {
            get { return peaks; }
            set { peaks = value; }
        }

        #endregion
    }
}
