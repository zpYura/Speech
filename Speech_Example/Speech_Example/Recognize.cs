using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.Wave;
using NPlot;

namespace Speech_Example
{
    public partial class Recognize : Form
    {
        public Recognize()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            waveIn = new WaveIn();

            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += new EventHandler<StoppedEventArgs>(waveIn_RecordingStopped);
            waveIn.WaveFormat = new WaveFormat(44100, 1);
            waveIn.StartRecording();
          
        }

        WaveIn waveIn;
        List<double> data = new List<double>();
        MP3 Mfile;

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
           for (int index = 0; index < e.BytesRecorded; index += 2)
        {
            short sample=0;
            try
            {
                sample = (short)((e.Buffer[index + 1] << 8) |
                                       e.Buffer[index + 0]);


                if (Math.Abs(sample) > 1000)
                {
                    float sample32 = sample / 32768f;
                    data.Add(sample32);
                }
            }
            catch { }
        }
        }

        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            waveIn.StopRecording();

            int g = data.Count;
            Mfile = new MP3();
           Mfile.NormData = data.ToArray();
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

            Mfile.Get_frames(10, 25, 44100);
            Mfile.Get_matrix(13);
        }

        private void Recognize_Load(object sender, EventArgs e)
        {
            button1.Text = "&Record";
            button2.Text = "&Stop";
            button3.Text = "&Determine";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MP3[] d = DataBase.Read_from_baze();
            
  
            int w = 0;

            int[] xyc = null;
            double[,] res = null;
            alglib.kmeansgenerate(Mfile.Matrix, Mfile.Matrix.GetLength(0), Mfile.Matrix.GetLength(1), 1, 2, out w, out res, out xyc);
            double[] min = new double[d.Length];
            for (int s = 0; s < d.Length; s++)
            {
                double[,] c = null;
               // alglib.kmeansgenerate(d[s].Matrix, d[s].Matrix.GetLength(0), d[s].Matrix.GetLength(1), 1, 2, out w, out c, out xyc);
                double answer = Clasterization.euclid(d[s].Matrix, res);
                min[s] = answer;

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
            //Метод ближайших соседей
            //double radius = 13;
            //int[] etalons = new int[5];
            //for (int s = 1; s < min.Length; s++)
            //{
            //    if (min[s] < radius)
            //    {
            //        switch (d[s].Word)
            //        {
            //            case "Пять": { etalons[0]++; } break;
            //            case "Один": { etalons[1]++; } break;
            //            case "Семь": { etalons[2]++; } break;
            //            case "Шесть": { etalons[3]++; } break;
            //            case "Два": { etalons[4]++; } break;
            //        }
            //    }
            //}

            //double min_m = etalons[0];
            //int n_min = 0;
            //for (int s = 1; s < etalons.Length; s++)
            //{
            //    if (etalons[s] > min_m)
            //    {
            //        min_m = etalons[s];
            //        n_min = s;
            //    }
            //}
            //string Word = "";
            //switch (n_min)
            //{
            //    case 0: { Word = "Пять"; } break;
            //    case 1: { Word = "Один"; } break;
            //    case 2: { Word = "Семь"; } break;
            //    case 3: { Word = "Шесть"; } break;
            //    case 4: { Word = "Два"; } break;
            //}
            //MessageBox.Show("Распознанное слово - " + Word, "Распознание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
