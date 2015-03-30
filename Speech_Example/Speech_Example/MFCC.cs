using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Speech_Example
{
    class MFCC
    {
        public static double[] FourierTransform(double[] source, Int32 lenght, bool useWindow)
        {
            Complex [] fourierCmplxRaw = new Complex[lenght];
            double[] fourierRaw = new double[lenght];
            for (Int32 k = 0; k < lenght; k++)
            {
                fourierCmplxRaw[k]=new Complex(0,0);
       
                for (Int32 n = 0; n < lenght; n++)
                {
                    double sample = source[n];

                    double x =-2*Math.PI*k*n/(double)lenght;
                    Complex f = sample * (new Complex(Math.Cos(x), Math.Sin(x)));

                    double w = 1;
                    if (useWindow)
                    {
                        w = 0.54 - 0.46 * Math.Cos(2 * Math.PI * n / (lenght - 1));
                    }

                    fourierCmplxRaw[k] += f * w;
                }

                fourierRaw[k] = Math.Sqrt(Math.Pow(fourierCmplxRaw[k].Magnitude, 2));
            }

            return fourierRaw;
        }

        public static double[] FastFourierTransform(double[] source, UInt32 lenght, bool useWindow)
        {
            UInt32 p2length = lenght;
            //bool powerOfTwo = (lenght > 0) && !((&(lenght - 1));
            
            Complex[] fourierRawTmp = new Complex[p2length];
            double[] fourierRaw = new double[lenght];
	        for (Int32 i = 0; i < p2length; i++) 
                {

		            // Even element is the real part of complex number
		            if (i < lenght) 
                    {
			            fourierRawTmp[i] =new Complex(source[i], 0);

			            if (useWindow) 
                        {
				         fourierRawTmp[i] *= (0.54 - 0.46 * Math.Cos(2 * Math.PI * i / (lenght - 1)));
			            }

		            } 
                    else {
			                    fourierRawTmp[i] = new Complex(0, 0);
		                   }
	              }

	// Perform recursive calculations
	fourierTransformFastRecursion( ref fourierRawTmp);

	// As for magnitude, let's use Euclid's distance for its calculation
	for (Int32 i = 0; i < lenght; i++) {
        fourierRaw[i] = Math.Sqrt(Math.Pow(fourierRawTmp[i].Magnitude, 2));
	}

	return fourierRaw;
        }

     private static void   fourierTransformFastRecursion(ref Complex[] data) 
         {
	       // Exit from recursion
	       Int32 n = data.Length;
	        if (n <= 1) {
		        return;
	        }

	                // Divide into Even/Odd
                    //valarray<complex<double>> even = data[std::slice(0, n/2, 2)];
                    //valarray<complex<double>> odd = data[std::slice(1, n/2, 2)];
                    Complex[] even=new Complex [n/2];
                    int j=0;
                    for(int i=0;i<even.Length;i=i+1)
                    {
                        even[i]=data[j];
                        j=j+2;
                    }
                    Complex[] odd=new Complex [n/2];
                     j=1;
                    for(int i=0;i<even.Length;i=i+1)
                    {
                        odd[i]=data[j];
                        j=j+2;
                    }
	                // Compute recursion
	                fourierTransformFastRecursion(ref  even);
	                fourierTransformFastRecursion( ref odd);

	                // Combine
	                for (Int32 i = 0; i < n / 2; i++) 
                    {
		            Complex t = Complex.FromPolarCoordinates(1.0, -2 * Math.PI * i / n) * odd[i];
		            data[i]       = even[i] + t;
		            data[i + n/2] = even[i] - t;
	                }
         }

     static double convertToMel(double f) { return 1125 * Math.Log(1 + f/700); }
     static double convertFromMel(double m) { return 700 * (Math.Exp(m/1125) - 1); }

       private static double [,] getMelFilters(int mfccSize, UInt32 filterLength, UInt32 frequency,UInt32 freqMin, UInt32 freqMax) 
       {

	    // Create points for filter banks
	    double[] fb = new double[mfccSize + 2];
	    fb[0] = convertToMel(freqMin);
	    fb[mfccSize + 1] = convertToMel(freqMax);

	    // Create mel bin
	    for ( short m = 1; m < mfccSize + 1; m++) 
        {
		fb[m] = fb[0] + m * (fb[mfccSize + 1] - fb[0]) / (mfccSize + 1);
	    }

	//frequency = 0.5 * frequency;
	    for ( short m = 0; m < mfccSize + 2; m++) 
        {

		// Convert them from mel to frequency
		fb[m] = convertFromMel(fb[m]);

		// Map those frequencies to the nearest FT bin
		fb[m] =Math.Floor((filterLength + 1) * fb[m] / (double) frequency);

        //assert("FT bin too small" &&
        //        !(m > 0 && (fb[m] - fb[m-1]) < numeric_limits<double>::epsilon()));
	    }

	// Calc filter banks
	double[,] filterBanks = new double [mfccSize,filterLength];
    //for (unsigned short m = 0; m < mfccSize; m++) {
    //    filterBanks[m] =  new double[filterLength];
    //}

	for ( short m = 1; m < mfccSize + 1; m++) 
    {
		for (UInt32 k = 0; k < filterLength; k++) 
        {

			if (fb[m - 1] <= k && k <= fb[m]) {
				filterBanks[m - 1,k] = (k - fb[m - 1]) / (fb[m] - fb[m - 1]);

			} else if (fb[m] < k && k <= fb[m + 1]) {
				filterBanks[m - 1,k] = (fb[m + 1] - k) / (fb[m + 1] - fb[m]);

			} else {
				filterBanks[m - 1,k] = 0;
			}
		}
	}

	return filterBanks;
    }

    private static  double[]  calcPower(double[] fourierRaw, UInt32 fourierLength,double[,] melFilters, int mfccCount) 
    {

	    double[] logPower = new double[mfccCount];

	    for ( short m = 0; m < mfccCount; m++) 
        {
		    logPower[m] = 0;

		    for (UInt32 k = 0; k < fourierLength; k++) 
            {
			    logPower[m] += melFilters[m,k] * Math.Pow(fourierRaw[k], 2);
		    }

        //assert("Spectrum power is less than zero" &&
        //        !(logPower[m] < numeric_limits<double>::epsilon()));

		// NOTE I'm not sure that we need to take logs since we normalized the input data
		    logPower[m] = Math.Log(logPower[m]);
	    }

	return logPower;
    }

   private static double[] dctTransform( double[] data, int length) 
    {

	    double[] dctTransform = new double[length];

	    for ( short n = 0; n < length; n++) 
        {
		    dctTransform[n] = 0;

		    for ( short m = 0; m < length; m++) 
            {
			    dctTransform[n] += data[m] * Math.Cos(Math.PI * n * (m + 1/2) / length);
		    }
	    }

	return dctTransform;
    }

   public static double[] transform( double[] source, UInt32 start,UInt32 finish, int mfccSize,UInt32 frequency, UInt32 freqMin, UInt32 freqMax) 
        {
        UInt32 sampleLength = finish - start + 1;
	    UInt32 p2length =(UInt32) Math.Pow(2, Math.Floor(Math.Log(sampleLength,2)));

	// Calc
	//time_t fourierStart = time(0);
	double[] fourierRaw =FastFourierTransform(source,p2length,true); 
	//cout << "Fourier: " << time(0) - fourierStart << " sec for " << p2length << " (" << sampleLength << ") elements" << endl;

	double[,] melFilters = getMelFilters(mfccSize, p2length, frequency, freqMin, freqMax);
	double[] logPower = calcPower(fourierRaw, p2length, melFilters, mfccSize);
	double[] dctRaw = dctTransform(logPower, mfccSize);

	return dctRaw;
}


       public static double DTWcalcDistance(double[] seq1, UInt32 seq1size, double[] seq2, UInt32 seq2size) 
       {

		// Create diff matrix
		double[,] diffM = new double[seq1size,seq2size];

		for (UInt32 i = 0; i < seq1size; i++) {
			for (UInt32 j = 0; j < seq2size; j++) {
				diffM[i,j] = Math.Abs(seq1[i] - seq2[j]);
			}
		}

		// Compute distance
		double distance = findDistance(seq1size, seq2size, diffM);

		// Clean up
		return distance;
	}

	public static double calcDistanceVector(double[] seq1, UInt32 seq1size, double[] seq2, UInt32 seq2size,int vectorSize) 
    {

		UInt32 seq1sizeV = (UInt32)(seq1size / vectorSize);
		UInt32 seq2sizeV = (UInt32)(seq2size / vectorSize);

		// Create diff matrix
		double[,] diffM = new double[seq1sizeV,seq2sizeV];

		for (UInt32 i = 0; i < seq1sizeV; i++) {
			for (UInt32 j = 0; j < seq2sizeV; j++) {

				// Calc distance between vectors
				double distance = 0;
				for (UInt32 k = 0; k < vectorSize; k++) {
					distance += Math.Pow(seq1[i * vectorSize + k] - seq2[j * vectorSize + k], 2);
				}

				diffM[i,j] = Math.Sqrt(distance);
			}
		}

		// Compute distance
		double distance1 = findDistance(seq1sizeV, seq2sizeV, diffM);
		return distance1;
	}

      public static  double findDistance(UInt32 seq1size, UInt32 seq2size, double[,] diffM) 
      {

	// Create distance matrix (forward direction)
	double[,] pathM = new double[seq1size,seq2size];

	pathM[0,0] = diffM[0,0];
	for (UInt32 i = 1; i < seq1size; i++) {
		pathM[i,0] = diffM[i,0] + pathM[i - 1,0];
	}
	for (UInt32 j = 1; j < seq2size; j++) {
		pathM[0,j] = diffM[0,j] + pathM[0,j - 1];
	}

	for (UInt32 i = 1; i < seq1size; i++) {
		for (UInt32 j = 1; j < seq2size; j++) {
			if (pathM[i - 1,j - 1] < pathM[i - 1,j]) {
				if (pathM[i - 1,j - 1] < pathM[i,j - 1]) {
					pathM[i,j] = diffM[i,j] + pathM[i - 1,j - 1];
				} else {
					pathM[i,j] = diffM[i,j] + pathM[i,j - 1];
				}
			} else {
				if (pathM[i - 1,j] < pathM[i,j - 1]) {
					pathM[i,j] = diffM[i,j] + pathM[i - 1,j];
				} else {
					pathM[i,j] = diffM[i,j] + pathM[i,j - 1];
				}
			}
		}
	}

	// Find the warping path (backward direction)
	UInt32 warpSize = seq1size * seq2size;
	double[] warpPath = new double[warpSize];

	UInt32 warpPathIndex = 0;
	UInt32 i1 = seq1size - 1; 
    UInt32 j1 = seq2size - 1;

	warpPath[warpPathIndex] = pathM[i1,j1];
    //DEBUG("Path (%d, %d): %f", i, j, pathM[i1,j1]);

	do {
		if (i1 > 0 && j1 > 0) {

			if (pathM[i1 - 1,j1 - 1] < pathM[i1 - 1,j1]) {
				if (pathM[i1 - 1,j1 - 1] < pathM[i1,j1 - 1]) {
					i1--;
					j1--;
				} else {
					j1--;
				}

			} else {
				if (pathM[i1 - 1,j1] < pathM[i1,j1 - 1]) {
					i1--;
				} else {
					j1--;
				}
			}

		} else {
			if (0 == i1) {
				j1--;
			} else {
				i1--;
			}
		}

		warpPath[++warpPathIndex] = pathM[i1,j1];
        //DEBUG("Path (%d, %d): %f", i, j, pathM[i][j]);

	} while (i1 > 0 || j1 > 0);

	// Calculate path measure
	double distance = 0;
	for (UInt32 k = 0; k < warpPathIndex + 1; k++) {
		distance += warpPath[k];
	}
	distance = distance / (warpPathIndex + 1);

	return distance;
}

      //public static double[] fft(double[] data, int len)
      //{ 
      
      //}

}
}
