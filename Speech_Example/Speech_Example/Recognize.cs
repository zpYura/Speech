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
            //writer = new WaveFileWriter(outputFilename, waveIn.WaveFormat);

            waveIn.StartRecording();
          
        }

        WaveIn waveIn;
        //WaveFileWriter writer;
        //string outputFilename = "D:/demo.wav";
        List<double> data = new List<double>();
        MP3 Mfile;

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
           // writer.WriteData(e.Buffer, 0, e.BytesRecorded);
           for (int index = 0; index < e.BytesRecorded; index += 2)
        {
            short sample = (short)((e.Buffer[index + 1] << 8) | 
                                    e.Buffer[index + 0]);
           float sample32 = sample / 32768f;
           //ProcessSample(sample32);
           data.Add(sample32);
        }
        }

        void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
          //  writer.Close();
            //writer = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            waveIn.StopRecording();

            int g = data.Count;
            Mfile = new MP3();
            //Mfile.Read_PCM(fileName);
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
           // Mfile.Word = comboBox1.Text;
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
            
            for (int s = 0; s < d.Length; s++)
            {
                List<double> data = new List<double>();
                for (int z = 0; z < d[s].NormData.Length; z++)
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
            for (int s = 0; s < d.Length && endflag; s++)
            {
                double[,] c = null;
                alglib.kmeansgenerate(d[s].Matrix, d[s].Matrix.GetLength(0), d[s].Matrix.GetLength(1), 1, 2, out w, out c, out xyc);
                double answer = Clasterization.euclid(c, res);
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
           
        }

    }
}
