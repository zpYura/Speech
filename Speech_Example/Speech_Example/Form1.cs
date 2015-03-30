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


namespace Speech_Example
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            plotSurface2D1.Title = "Wave";
            string fileName = "D:\\Мои документы\\Универ\\РПБДИС\\Курсач\\Speech_Recognition\\samples\\f1one3.mp3";
           MP3 Mfile = new MP3();
           Mfile.NormData = MP3.Read_PCM(fileName);
           double[] time = new double[Mfile.NormData.Length];
           for (int i = 0; i < time.Length; i++)
               time[i] = (double)i / 44100;
             plotSurface2D1.Clear();
     
      plotSurface2D1.BackColor = Color.White;
      plotSurface2D1.TitleFont =  new System.Drawing.Font("Arial", 12);

      //'Left Y axis grid:
      plotSurface2D1.Add(new NPlot.Grid(), NPlot.PlotSurface2D.XAxisPosition.Bottom, NPlot.PlotSurface2D.YAxisPosition.Left);

     // 'Timeseries 1 to Plot 1:
             LinePlot npPlot1 =new NPlot.LinePlot();
      npPlot1.AbscissaData = time;
      npPlot1.DataSource = Mfile.NormData;
      npPlot1.Color = Color.Blue;
      npPlot1.Label = "Timeseries 1";
      plotSurface2D1.Add(npPlot1, NPlot.PlotSurface2D.XAxisPosition.Bottom, NPlot.PlotSurface2D.YAxisPosition.Left);
        }
    }
}
