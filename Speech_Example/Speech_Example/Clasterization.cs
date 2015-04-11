using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Example
{
    class Clasterization
    {

        //bool	learn = false;
        //int	meansNum = 10;
        //int	kmeansMaxIters = 2000;
        //int	minVectorsNumForKMeans = 10;
        //double adaptWeight = 20.0f;
        //int	LDASize = 20;
        //double sigmaFloor = 0.001f;
        //bool	useSigma = false;
        static int dim = 13;

        //int m_dim;
        //int meansNum;
        static double[] _means=null;

        //float m_adaptWeight;
        //MemoryVectorsSet m_adaptMeans;

        //int m_LDASize;
        //vector<float> m_LDAMatrix;
        //vector<float> m_mean;

        //MemoryVectorsSet m_projections;

        //vector<string> m_learnWords;
        //vector<float> m_wordsMean;
        //vector<float> m_wordsSigma;

        bool m_baseOK;
        bool m_meansOK;
        bool m_adaptMeansOK;
        bool m_LDAOK;
        bool m_projectionsOK;
        bool m_compareSystemOK;

	
        public static void Kmeans(MP3 [] mfcc, int _maxItersNum, int _meansNum, int _minVectorsNum)
        { 
            _means=new double[_meansNum * dim];
            int filesNum = mfcc.Length;
            int vectorsSize =dim;
            int vectorsNum = 0;
           // double[] pVector = null;
            double[] pMean = null;
            int allVectorsNum = 0;

            int n, i, k, m, iter;
            double dist;
            double delta;
            double minDist;
            int nearestM;
   
        double[] minValData=new double [vectorsSize];
        double[] maxValData=new double [vectorsSize];
   // float* minVal = &minValData[0];
    //float* maxVal = &maxValData[0];
         double[] minVal=new double [vectorsSize];
          double[] maxVal=new double [vectorsSize];
    for (k = 0; k < vectorsSize; k++)
    {
        minVal[k] = 3.4*Math.Pow(10,34);
        maxVal[k] = -3.4*Math.Pow(10,34);
    }

    allVectorsNum = 0;
    
    for (n = 0; n < filesNum; n++)
    {
        m = 0;
       vectorsNum=mfcc[n].NormData.Length/dim;
        for (i = 0; i < vectorsNum; i++)
        {
            for (k = 0; k < vectorsSize; k++)
            {
                if (mfcc[n].NormData[m] < minVal[k]) minVal[k] = mfcc[n].NormData[m];
                if (mfcc[n].NormData[m] > maxVal[k]) maxVal[k] = mfcc[n].NormData[m];
                m = m + 1;
            }
        }
        allVectorsNum += vectorsNum;
    }
    if (allVectorsNum < _minVectorsNum * _meansNum)
    {
        //throw exception("KMeans: not enough data (all vectors num < (min vectors num) * (means num))");
        int j=0;
    }

    int[] vectorsNumInClusterData=new int [_meansNum];
    double[] newMeansData=new double[_meansNum * vectorsSize];
    int[] vectorsNumInCluster = new int[_meansNum];
    double[] newMeans = new double[_meansNum * vectorsSize]; ;

    Random r = new Random(0);
    pMean = _means;
    i = 0;
    for (m = 0; m < _meansNum; m++)
    {
        for (k = 0; k < vectorsSize; k++)
        {
            pMean[i] = minVal[k] + (maxVal[k] - minVal[k]) * r.Next(32767) / 32767;
            i = i + 1;
        }
    }

    for (iter = 0; iter < _maxItersNum; iter++)
    {

        //memset(vectorsNumInCluster, 0, sizeof(int) * _meansNum);
       // memset(newMeans, 0, sizeof(float) * _meansNum * vectorsSize);
        vectorsNumInCluster.Initialize();
        newMeans.Initialize();
        int j = 0;
        for (n = 0; n < filesNum; n++)
        {
            vectorsNum = mfcc[n].NormData.Length / dim;
            j = 0;
            for (i = 0; i < vectorsNum; i++)
            {
                // Íàõîäèì áëèæàéøåå ñðåäíåå çíà÷åíèå äëÿ î÷åðåäíîãî âåêòîðà pVector
                minDist = 3.4 * Math.Pow(10, 34);
                nearestM = -1;
                pMean = _means;
                int h = 0;
                for (m = 0; m < _meansNum; m++)
                {
                    dist = 0;
                    for (k = 0; k < vectorsSize; k++)
                    {
                        delta = pMean[h] - mfcc[n].NormData[k + vectorsSize*i];
                       // j = j + 1;
                        h = h + 1;
                        dist += delta * delta;
                    }
                    dist = Math.Sqrt(dist);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearestM = m;
                    }
                }
                if (nearestM < 0)
                {
                    //throw exception("KMeans: internal error");
                    int q = 0;
                }

                //было равно newmeANS
                for (int z = 0; z < pMean.Length;z++ )
                    pMean[z] = newMeans[z] + nearestM * vectorsSize;
                for (k = 0; k < vectorsSize; k++)
                {
                    pMean[k] += mfcc[n].NormData[k+vectorsSize * i];
                }
                vectorsNumInCluster[nearestM]++;
                j = j + 1;
            }
        }

        pMean = newMeans;
         i = 0;
        for (m = 0; m < _meansNum; m++, i += vectorsSize)
        {
            if (vectorsNumInCluster[m] >= _minVectorsNum)
            {
                for (k = i; k < vectorsSize; k++)
                {
                    pMean[k] /= (float)vectorsNumInCluster[m];
                }
            }
            else
            {
                for (k = 0; k < vectorsSize; k++)
                {
                    pMean[k] = minVal[k] + (maxVal[k] - minVal[k]) * r.Next(32767) / 32767;
                }
            }
        }

        delta = 0;
        for (k = 0; k < _meansNum * vectorsSize; k++)
        {
            if (Math.Abs(_means[k] - newMeans[k]) > delta) delta = Math.Abs(_means[k] - newMeans[k]);
        }
        //memcpy(_means, newMeans, sizeof(float) * _meansNum * vectorsSize);
        _means=newMeans;
        double ffff=Math.Pow(1.175494351,-38);
        if (delta < 2.0f * ffff)
        {
        int y=0;
        }
    }
    int kkkk = 0;
        }

        
    }
}
