using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPlot;
using System.IO;


namespace Speech_Example
{
    public partial class Form1 : Form
    {
        MP3 Mfile;
        public Form1()
        {
            InitializeComponent();

           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                plotSurface2D1.Title = "Wave";
                string fileName = openFileDialog1.FileName;
                Mfile = new MP3();
                Mfile.Read_PCM(fileName);
                double[] time = new double[Mfile.NormData.Length];
                for (int i = 0; i < time.Length; i++)
                    time[i] = (double)i / 44100;
                plotSurface2D1.Clear();

                plotSurface2D1.BackColor = Color.White;
                plotSurface2D1.TitleFont = new System.Drawing.Font("Arial", 12);

                //'Left Y axis grid:
                plotSurface2D1.Add(new NPlot.Grid(), NPlot.PlotSurface2D.XAxisPosition.Bottom, NPlot.PlotSurface2D.YAxisPosition.Left);

                // 'Timeseries 1 to Plot 1:
                LinePlot npPlot1 = new NPlot.LinePlot();
                npPlot1.AbscissaData = time;
                npPlot1.DataSource = Mfile.NormData;
                npPlot1.Color = Color.Blue;
                npPlot1.Label = "Timeseries 1";
                plotSurface2D1.Add(npPlot1, NPlot.PlotSurface2D.XAxisPosition.Bottom, NPlot.PlotSurface2D.YAxisPosition.Left);
               
            }
            
           }

        private void button2_Click(object sender, EventArgs e)
        {
            Mfile.Get_frames(10, 25, 44100);
            Mfile.Word = comboBox1.Text;
            Mfile.Get_matrix(13);
            for (int i = 0; i < Mfile.Frames.Length; i++)
            {
                double[] mas = Mfile.Frames[i].MFCC;

                dataGridView1.Columns.Add("MFCC" + i.ToString(), "MFCC" + i.ToString());
                if (dataGridView1.ColumnCount == 1)
                    dataGridView1.Rows.Add(mas.Length - 1);
                for (int j = 0; j < mas.Length; j++)
                {
                    dataGridView1[dataGridView1.ColumnCount - 1, j].Value = mas[j];

                }
            }

           // DataBase.Insert_values(Mfile, Convert.ToInt32(textBox1.Text));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
        }

        /// <summary>
        /// Добавление тестовой базы в бд, использовать только с подготовленными данными, смотри папку Record/Base/I
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string[] s=openFileDialog1.FileNames;
                string[] s1={ "Пять", "Один","Семь","Шесть","Два" };
                int j=0;
                for (int i = 0; i < s.Length; i++)
                {
                     MP3 M = new MP3();
                    M.Read_PCM(s[i]);
                    M.Get_frames(10, 25, 44100);
                  
                    M.Word = s1[j];
                    if ((i + 1) % 8 == 0)
                        j++;
                    DataBase.Insert_values(M, i+1);
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MP3 [] d= DataBase.Read_from_baze();
           // int w=0;
            //double [,] c=null;
            //double[,] c1 = null;
            //double[,] c2 = null;
            //double[,] c3= null;
            //double[,] c4 = null;
            //int [] xyc=null;
            //string descr =d[0].Word;
           // Dictionary<string, double[,]> dict = new Dictionary<string, double[,]>();
            //int q=0;
            //int s = 0;
            //while (q < 5)
            //{ 
                
            //    List<double> data = new List<double>();
            //    while (s < d.Length && descr == d[s].Word)
            //    {
            //        //foreach (double dd in d[s].NormData)

            //        for (int z = 0; z < d[s].NormData.Length;z++ )
            //            data.Add(d[s].NormData[z]);
            //        s++;
            //    }
        
            //    double[,] Matrixd = new double[data.Count / 13, 13];
            //    int k=0;
            //    for (int i = 0; i < Matrixd.GetLength(0); i++)
            //    {
            //    for (int j = 0; j < Matrixd.GetLength(1); j++)
            //    {
            //     Matrixd[i, j] = data[k];
            //    k++;
            //    }
            //    }
            //    dict.Add(descr, Matrixd);
            //    if(s<d.Length)
            //    descr = d[s].Word;
            //    q++;      
            //}

            //Dictionary<string, double[,]> super = new Dictionary<string, double[,]>();
            //alglib.kmeansgenerate(dict["Пять"], dict["Пять"].GetLength(0), dict["Пять"].GetLength(1), 10, 2, out w, out c, out xyc);
            //super.Add("Пять", c);
            //alglib.kmeansgenerate(dict["Один"], dict["Один"].GetLength(0), dict["Один"].GetLength(1), 10, 2, out w, out c1, out xyc);
            //super.Add("Один", c1);
            //alglib.kmeansgenerate(dict["Два"], dict["Два"].GetLength(0), dict["Два"].GetLength(1), 10, 2, out w, out c2, out xyc);
            //super.Add("Два", c2);
            //alglib.kmeansgenerate(dict["Шесть"], dict["Шесть"].GetLength(0), dict["Шесть"].GetLength(1), 10, 2, out w, out c3, out xyc);
            //super.Add("Шесть", c3);
            //alglib.kmeansgenerate(dict["Семь"], dict["Семь"].GetLength(0), dict["Семь"].GetLength(1), 10, 2, out w, out c4, out xyc);
            //super.Add("Семь", c4);
            for (int s = 0; s < d.Length; s++)
            {
                List<double> data = new List<double>();
             for (int z = 0; z < d[s].NormData.Length;z++ )
                       data.Add(d[s].NormData[z]);
             double[,] Matrixd = new double[data.Count / 13, 13];
             int k = 0;
             for (int i = 0; i < Matrixd.GetLength(0); i++)
             {
                 for (int j = 0; j < Matrixd.GetLength(1); j++)
                 {
                     Matrixd[i, j] = data[k];
                     k++;
                 }
             }
             d[s].Matrix = Matrixd;
            }
            int w = 0;
           
            int[] xyc = null;
            double[,] res = null;
            alglib.kmeansgenerate(Mfile.Matrix, Mfile.Matrix.GetLength(0), Mfile.Matrix.GetLength(1), 1, 2, out w, out res, out xyc);
            bool endflag = true;
            double[] min = new double[d.Length];
            for (int s = 0; s < d.Length&&endflag; s++)
            {
                double[,] c = null;
                alglib.kmeansgenerate(d[s].Matrix, d[s].Matrix.GetLength(0), d[s].Matrix.GetLength(1), 1, 2, out w, out c, out xyc);
               double answer= Clasterization.euclid(c, res);
               min[s] = answer;
               //if (answer < 1 && answer > 0)
              // {
                 //  MessageBox.Show("Распознанное слово - " + d[s].Word, "Распознание", MessageBoxButtons.OK, MessageBoxIcon.Information);
               //    endflag = false;
               //}
               
            }
            double min_m = min[0];
            int n_min = 0;
            for (int s = 1; s < min.Length; s++)
            {
                if (min[s] < min_m)
                {
                    min_m = min[s];
                    n_min = s;
                }
            }
            MessageBox.Show("Распознанное слово - " + d[n_min].Word, "Распознание", MessageBoxButtons.OK, MessageBoxIcon.Information);
          // string answer= Clasterization.recogn(super, res);
          // string answer = Clasterization.recogn_mat(super, res);
         //  MessageBox.Show("Распознанное слово - "+answer, "Распознание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // alglib.lda.fisherldan(MP3.Matrixd, MP3.Matrixd.GetLength(0), 13, 5, ref i, ref c);
            //    Clasterization.Kmeans(d, 2000, 10, 10);
            //Clasterization.MakeAdaptation(10, d);
        }
    }
}
