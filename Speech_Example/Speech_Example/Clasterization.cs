﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

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
        static double m_adaptWeight = 20.0f;
        static List<double[]> m_adaptMeans=new List<double[]>();
        static int m_adaptMeansSetDim;
        static int m_adaptMeansSetSize;
        static double[] _result = null;
        //float m_adaptWeight;
        //MemoryVectorsSet m_adaptMeans;

        //int m_LDASize;
        //vector<float> m_LDAMatrix;
        //vector<float> m_mean;

        //MemoryVectorsSet m_projections;

        //vector<string> m_learnWords;
        //vector<float> m_wordsMean;
        //vector<float> m_wordsSigma;

        //bool m_baseOK;
        //bool m_meansOK;
        //bool m_adaptMeansOK;
        //bool m_LDAOK;
        //bool m_projectionsOK;
        //bool m_compareSystemOK;

	
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
                MessageBox.Show("Мало векторов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            bool flag = true;
            for (iter = 0; iter < _maxItersNum&&flag; iter++)
            {

                //memset(vectorsNumInCluster, 0, sizeof(int) * _meansNum);
                // memset(newMeans, 0, sizeof(float) * _meansNum * vectorsSize);
                vectorsNumInCluster.Initialize();
                newMeans.Initialize();
               // int j = 0;
                for (n = 0; n < filesNum; n++)
                {
                    vectorsNum = mfcc[n].NormData.Length / dim;
                    //j = 0;
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
                            MessageBox.Show("KMeans: internal error", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        //было равно newmeANS
                        for (int z = 0; z < pMean.Length;z++ )
                        pMean[z] = newMeans[z] + nearestM * vectorsSize;
                        for (k = 0; k < vectorsSize; k++)
                        {
                    pMean[k] += mfcc[n].NormData[k+vectorsSize * i];
                        }
                        vectorsNumInCluster[nearestM]++;
                        //j = j + 1;
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
            flag = false;
        }
    }

        }

        public static void MakeAdaptation(int meansNum, MP3 [] m_mfcc)
	{
		
		int superVectorDim = meansNum *dim;
		//if ((int)m_means.size() != superVectorDim) throw exception("wrong means size");

		// Ãðóïïèðóåì àäàïòèðîâàííûå ñðåäíèå çíà÷åíèÿ ïî ïðèíàäëåæíîñòè îäíîìó ñëîâó
		m_adaptMeansSetDim=superVectorDim;
        m_adaptMeansSetSize=5;
		//m_adaptMeans.SetSize((int)m_files.size());
		//int count = 0;
		for (int i = 0; i < m_adaptMeansSetSize; i++)
		{
            double [] mm=new double[5*superVectorDim];
			m_adaptMeans.Add(mm);
			//count += (int)m_files[i].size();
		}
		//if (count != m_mfcc.GetSize()) throw exception("m_mfcc.GetSize() != sum of m_files[i].size()");

		// Ïðîâîäèì àäàïòàöèþ äëÿ âñåõ ôàéëîâ
		int count = 0;
		int adaptVectorsNum = 0;
		double[] adaptResult = null;
		int mfccVectorsNum = 0;
		double[] mfccData = null;
		for (int i = 0; i < m_adaptMeansSetSize; i++)
		{
			//m_adaptMeans.GetVectors(i, adaptVectorsNum, adaptResult);
            //adaptResult += m_adaptMeans.GetDim()
            adaptVectorsNum=m_adaptMeans[i].Length/superVectorDim;
			for (int j = 0; j < adaptVectorsNum; j++, count++)
			{
				//m_mfcc.GetVectors(count, mfccVectorsNum, mfccData);
               mfccVectorsNum=m_mfcc[count].NormData.Length/dim;
               Adaptation(dim, mfccVectorsNum, meansNum, _means, m_adaptWeight, m_mfcc[count].NormData, ref adaptResult);
			}
		}
		//m_adaptMeansOK = true;
	}

        static void Adaptation(int _vectorsSize, int _vectorsNum, int _meansNum, double[] _means, double _weight, double[] _vectors, ref double[] _result)
{
    ////if (_weight < 0.f) throw exception("Adaptation: bad param (_weight < 0)");
    //if (_vectorsSize <= 0) throw exception("Adaptation: bad param (_vectorsSize < 0)");
    //if (_vectorsNum < 0) throw exception("Adaptation: bad param (_vectorsNum < 0)");
    //if (_meansNum <= 0) throw exception("Adaptation: bad param (_meansNum < 0)");

	 double[] pVector = null;
	 double[] pMean = null;
	double[] pResult = null;
	double minD = 3.4 * Math.Pow(10, 34);
	double d = 0;
	double x = 0;
	int minJ = 0;
	int i, j, k;
	
	int [] n=new int [_meansNum];
	//memset(&n[0], 0, sizeof(int) * _meansNum);
	_result=new double [_meansNum * _vectorsSize];
	
	pVector = _vectors;
	for (i = 0; i < _vectorsNum; i++)
	{
        minD = 3.4 * Math.Pow(10, 34);
		minJ = 0;
		d = 0;
		pMean = _means;
        int v = 0;
		for (j = 0; j < _meansNum; j++)
		{
			d = 0;
			for (k = 0; k < _vectorsSize; k++)
			{
                x = (pVector[k+_vectorsSize * i] - pMean[v]);
				d += x * x;
                v = v + 1;
			}
			d = Math.Sqrt(d);
			if (d < minD)
			{
				minD = d;
				minJ = j;
			}
		}
		n[minJ]++;
        pResult = new double[_result.Length];
        for (int f = 0; f < _result.Length;f++ )
            pResult[f] = _result[f] + minJ * _vectorsSize;
		for (k = 0; k < _vectorsSize; k++)
            pResult[k] += pVector[k+_vectorsSize * i];
	}
	pMean = _means;
	pResult = _result;
	for (j = 0; j < _meansNum; j++)
	{
		if (n[j] > 0)
		{
			d = 1.0f / (_weight + (float)n[j]);
			for (k = 0; k < _vectorsSize; k++)
                pResult[k] = (_weight * pMean[k + _vectorsSize * j] + pResult[k+_vectorsSize * j]) * d;
		}
		else
		{
			//memcpy(pResult, pMean, sizeof(float) * _vectorsSize);
            pResult = pMean;
		}
	}
}
        public static double SKO(double[,] data,out double sr)
        {
            double sko = 0;
            double[] sqr = new double[data.GetLength(0) * data.GetLength(1)];
            int k = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    sqr[k] = data[i, j];
                    k++;
                }
               
            }
            double av = sqr.Average();
            
            sr = av;
            double sum = 0;
            for (int i = 0; i < sqr.Length; i++)
            { 
               sum=sum+Math.Pow((sqr[i]-av),2);
            }
            sko = Math.Sqrt(sum / sqr.Length);
            return sko;
        }

        public static double[,] SKO_mat(double[,] data)
        {
           
            double[,] res = new double[data.GetLength(0), 2];
            for (int i = 0; i < data.GetLength(0); i++)
            {

                double average = 0;
                double sko = 0;
                double sum = 0;
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    average = average + data[i, j];
                }
                average = average / data.GetLength(1);
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    sum = sum + Math.Pow((data[i, j] - average), 2);
                }
                res[i, 1] = average;
                res[i, 0] = Math.Sqrt(sum / data.GetLength(1));

            }
            
            return res;
        }

        public static double euclid(double [,] data,double [,] res)
        {
            List<double> d = new List<double>();
            double e = 0;
            for (int j = 0; j < data.GetLength(1); j++)
            {
                double s = 0;
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    s = s + Math.Pow(data[i, j] - res[i, j], 2);
                    //s = s + Math.Log(Math.Abs(data[i, j] - res[i, j]),2);
                }
                d.Add(s);
            }
            //e = Math.Sqrt(Math.Pow(data[0, 0] - res[0, 0], 2) + Math.Pow(data[1, 0] - res[1, 0], 2) + Math.Pow(data[2, 0] - res[2, 0], 2) + Math.Pow(data[3, 0] - res[3, 0], 2) + Math.Pow(data[4, 0] - res[4, 0], 2) + Math.Pow(data[5, 0] - res[5, 0], 2) + Math.Pow(data[6, 0] - res[6, 0], 2) + Math.Pow(data[7, 0] - res[7, 0], 2) + Math.Pow(data[8, 0] - res[8, 0], 2) + Math.Pow(data[9, 0] - res[9, 0], 2) + Math.Pow(data[10, 0] - res[10, 0], 2) + Math.Pow(data[11, 0] - res[11, 0], 2) + Math.Pow(data[12, 0] - res[12, 0], 2));
            e = Math.Sqrt(d[0]);
            return e;
        }

        public static int Compare_mat(List<double[,]> sko_mat, double[,] res)
        {
            
            List<double> d = new List<double>();
            int[] comp = new int[sko_mat.Count];
            double e = 0;
            for (int j = 0; j < res.GetLength(1); j++)
            {
                
                
                for (int i = 0; i < res.GetLength(0); i++)
                {
                    double s = 0;
                    d.Clear();
                    for (int k = 0; k < sko_mat.Count; k++)
                    {
                        s = Math.Sqrt(Math.Pow(sko_mat[k][i, j] - res[i, j], 2));
                        d.Add(s);
                    }
                    double min = d[0];
                    int num_min = 0;
                    for (int k = 1; k < sko_mat.Count; k++)
                    {
                        if (d[k] < min)
                        {
                            min = d[k];
                            num_min = k;
                        }
                    }

                    comp[num_min]++;
                }
                //d.Add(s);
            }
            double max = comp[0];
            int num_max = 0;
            for (int k = 1; k < comp.Length; k++)
            {
                if (comp[k] > max)
                {
                    max = comp[k];
                    num_max = k;
                }
            }
           // e = Math.Sqrt(d[1]) + Math.Sqrt(d[0]);
            return num_max;
        }
        public static string recogn_mat(Dictionary<string, double[,]> dict, double[,] res)
        {
            List<double[,]> sko_mat = new List<double[,]>();
            
            //List<double> average = new List<double>();
            string[] d = dict.Keys.ToArray();
            for (int i = 0; i < dict.Count; i++)
            {
                double av = 0;
                sko_mat.Add(Clasterization.SKO_mat(dict[d[i]]));
                //average.Add(av);
            }
            //double av_res = 0;
            double[,] sko_res = Clasterization.SKO_mat(res);
            double[] sqr=new double [dict.Count];

            //List<double> sqr = new List<double>();
            for (int i = 0; i < sqr.Length; i++)
            {
                sqr[i] = Clasterization.euclid(sko_mat[i], sko_res);
            }
            double min = sqr[0];
            int num_min = 0;
            for (int i = 1; i < sqr.Length; i++)
            {
                if (sqr[i] < min)
                {
                    min = sqr[i];
                    num_min = i;
                }
            }
          //  int h=Compare_mat(sko_mat, sko_res);
            return d[num_min];
        }


        public static string recogn(Dictionary<string,double[,]> dict, double[,] res)
        {
            List<double> sko = new List<double>();
            List<double> average = new List<double>();
            string[] d=dict.Keys.ToArray();
            for (int i = 0; i < dict.Count; i++)
            {
                double av = 0;
                sko.Add(Clasterization.SKO(dict[d[i]],out av));
                average.Add(av);
            }
            double av_res = 0;
            double sko_res = Clasterization.SKO(res,out av_res);
            List<double> sqr = new List<double>();
            for (int i = 0; i < sko.Count; i++)
            { 
                 double v=Math.Sqrt(Math.Pow(sko[i]-sko_res,2));
                 sqr.Add(v);
            }
            double min = sqr[0];
            int num_min = 0;
            for (int i = 1; i < sqr.Count; i++)
            {
                if (sqr[i] < min)
                {
                    min = sqr[i];
                    num_min = i;
                }
            }

            return d[num_min];
        }
        
    }
}
