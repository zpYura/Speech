﻿using System;
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

            DataBase.Insert_values(Mfile, Convert.ToInt32(textBox1.Text));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
        }

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
                    if ((i + 1) % 5 == 0)
                        j++;
                    DataBase.Insert_values(M, i+1);
                }

            }
        }
    }
}
