using System;
using System.Collections.Generic;

namespace RhythmRunner.Audio
{
	class SpectralFlux
	{
        private List<float> spectralFluxes;
        public double[] currentSpectrum;
        public double[] lastSpectrum;
		
		public SpectralFlux(int sampleWindow)
		{
			spectralFluxes = new List<float>();
			currentSpectrum = new double[sampleWindow];
            lastSpectrum = new double[sampleWindow];
		}
		
		/// <summary>
        /// Calculates the fluctuation between the current spectrum and the last spectrum and then adds it to spectralFluxList. Ignores negative values.
		/// This function is designed to be used in a loop whereby the spectral fluxes get added to the spectralFluxes List object, which can be accessed
		/// using the SpectralFluxes property.
        /// </summary>
        /// <param name="spectrum">A spectrum that is the same size as the sampleWindow size used in the constructor.</param>
		
        public void CalculateSpectralFlux(double[] spectrum)
        {
	        lastSpectrum = currentSpectrum;			// Swap the currentSpectrum value to the lastSpectrum object
	        currentSpectrum = (double[])spectrum.Clone();				// Give currentSpectrum the new spectrum value.
            double flux = 0;
			
	        for (int i = 0; i < spectrum.Length; i++)
	        {
                double value = (currentSpectrum[i] - lastSpectrum[i]);	// Compare the spectrums index by index.
		        flux += (value < 0) ? 0 : value;						// Top up flux with the value if it is greater than zero
	        }
	        spectralFluxes.Add((float)flux);        // Add the flux to the list
        }

        public List<float> SpectralFluxes
        {
            get { return spectralFluxes; }
            set { spectralFluxes = value; }
        }
	}
}