using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace Speech_Example
{
    class Clasterization
    {

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
