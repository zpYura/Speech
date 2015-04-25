using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using System.IO;
using NAudio.Wave;

namespace Speech_Example
{
    class MP3
    {
        //int[] rawData;
        /// <summary>
        /// Нормализованные данные
        /// </summary>
        double[] normalizedData=null;
        /// <summary>
        /// Содержание мр3 файла
        /// </summary>
        byte[] bdata=null;
        double maxVal=0;
        double minVal=0;
        /// <summary>
        /// Слово
        /// </summary>
        string description=null;
        /// <summary>
        /// Набор фреймов
        /// </summary>
        Frame[] frames=null;

        public static double[,] Matrixd;
        double[] Matrix_mfcc=null;

        /// <summary>
        /// Св-во для доступа к нормализованным данным
        /// </summary>
        public double[] NormData
        {
            get { return normalizedData; }
            set { normalizedData = value; }
        }

        /// <summary>
        /// Св-во для доступа к содержанию мр3 файла
        /// </summary>
        public byte[] Mp3byte
        {
            get { return bdata; }
            set { bdata = value; }
        }

        /// <summary>
        /// Св-во для доступа к слову
        /// </summary>
        public string Word
        {
            get { return description; }
            set { description = value; }
        }

        public Frame [] Frames
        {
            get { return frames; }
           
        }

        public double[] Matrix
        {
            get { return Matrix_mfcc; }
        }

        public void Write()
        {
            StreamWriter mat = new StreamWriter("matlabtwo2.txt");
            if (normalizedData.Length!=0)
            {
                for (int i = 0; i < normalizedData.Length; i++)
                {
                    mat.WriteLine(normalizedData[i].ToString());
                }
            }
            mat.Close();
        }

        public void Read(string filename)
        {
            StreamReader mat = new StreamReader(filename);
            var mas = new List<double>();
            while (!mat.EndOfStream)
            {
                double m = Convert.ToDouble(mat.ReadLine());
                mas.Add(m);
            }
            normalizedData = mas.ToArray();
            mat.Close();
        }

        public void Get_frames(double Ts, double Tw, int fs)
        { 
            double Nw = Math.Round( 1E-3*Tw*fs );    
            double Ns = Math.Round( 1E-3*Ts*fs );
           int L = normalizedData.Length;                
           double  M = Math.Floor((L-Nw)/Ns+1);
            int num=0;
            double m = M;
            bool flag = true;
           while(flag)
           {
               if (L % m == 0)
               {
                   num = Convert.ToInt32(L / m) * 2;
                   flag = false;
               }
               else
                   m = m - 1;
           }
           //frames = new Frame[(int)M];
           //int j = 0;
           //for (int i = 0; i < frames.Length; i++)
           //{ 
           //    double [] mas=new double[num];
           //    j = i * num / 2;
           //    for (int k = 0; k < num; k++)
           //    {
           //        mas[k] = normalizedData[j];
           //        j++;
           //    }
           //    frames[i] = new Frame(mas);
           //}
           List<Frame> f = new List<Frame>();
           int j = 0;
           int i = 0;
           while (j < normalizedData.Length)
           {
               double[] mas = new double[num];
               j = i * num / 2;
               for (int k = 0; k < num; k++)
               {
                   mas[k] = normalizedData[j];
                   j++;
               }
               i++;
               bool a = mas.All(element => element == 0);
               if (!a)
               {
                   double[] masmf = MFCC.transform(mas, 0, (UInt32)mas.Length, 13, 44100, 300, 8000);
                   if (masmf[0].ToString() !="-бесконечность")
                   {
                       f.Add(new Frame(mas, masmf));
                   }
                   
               }

           }
           frames = f.ToArray();
        }


        public void Get_matrix( int mfccSize, UInt32 frequency, UInt32 freqMin, UInt32 freqMax)
        {

            Matrix_mfcc = new double[mfccSize*frames.Length];
            int k=0;
            for (int i = 0; i < frames.Length; i++)
            {
                double[] mas = MFCC.transform(frames[i].Data, 0, (UInt32)frames[i].Data.Length, mfccSize, frequency, freqMin, freqMax);
                for (int j = 0; j < mas.Length; j++)
                {
                    Matrix_mfcc[k] = mas[j];
                    k++;
                }
            }

        }
        public void Read_MP3(string filename)
        { 
            NAudio.Wave.Mp3FileReader read = new NAudio.Wave.Mp3FileReader(filename);

            List<int> rdata = new List<int>();
            BinaryReader br = new BinaryReader(File.OpenRead(filename));
            if (read.Id3v2Tag!=null)
            br.BaseStream.Seek(read.Id3v2Tag.RawData.Length, SeekOrigin.Begin);
            Mp3Frame frame;
           Int32 value16, valueLeft16, valueRight16;
            Int32 bytesPerSample = read.WaveFormat.BitsPerSample/ 8;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                br.BaseStream.Seek(4,SeekOrigin.Current);
                while ((frame = read.ReadNextFrame()) != null && br.BaseStream.Position < br.BaseStream.Length)
                 {
                     long numberOfSamplesXChannels = (frame.FrameLength-4) / (read.WaveFormat.Channels * bytesPerSample);
                    Int32 sampleNumber=0;
                    for (; sampleNumber < numberOfSamplesXChannels && br.BaseStream.Position<br.BaseStream.Length; sampleNumber++)
                     {
                         if (read.WaveFormat.Channels == 1)
                         {
                             value16 = br.ReadInt16();
                         }
                         else
                         {
                             valueLeft16 = br.ReadInt16();
                             valueRight16 = br.ReadInt16();
                             value16 = (Math.Abs(valueLeft16) + Math.Abs(valueRight16)) / 2;
                         }

                         if (maxVal < value16)
                             maxVal = value16;

                         if (minVal > value16)
                             minVal = value16;

                         rdata.Add(value16);
                     }
                  }
            }
            //rawData = rdata.ToArray();
            normalizedData = new double[rdata.Count];
            double maxAbs = Math.Max(Math.Abs(maxVal), Math.Abs(minVal));
            for (Int32 i = 0; i < rdata.Count; i++)
            {
                normalizedData[i] = (double)(rdata[i] / maxAbs);
            }

            br.Close();
        }

        public static double[] Read_data(string filename)
        {
            NAudio.Wave.Mp3FileReader read = new NAudio.Wave.Mp3FileReader(filename);
            //BinaryReader br = new BinaryReader(File.OpenRead(filename));
            //br.BaseStream.Seek(read.Id3v2Tag.RawData.Length, SeekOrigin.Begin);
           
            Mp3Frame frame;
            List<double> data = new List<double>();
            int i = 0;
            while ((frame = read.ReadNextFrame()) != null)
            {
                for (i = 4; i < frame.RawData.Length-1; i=i+2)
                {
                    short sample = (short)((frame.RawData[i + 1] << 8) | frame.RawData[i + 0]);
                    /* short sample2 = BitConverter.ToInt16(buffer, index);
                    Debug.Assert(sample == sample2, "Oops"); */
                    float sample32 = sample / 32768f;
                    data.Add(sample32);
                }
            }
           
            read.Dispose();
            return data.ToArray();
        }

        /// <summary>
        /// Считывание, декодирование и нормализация данных, полученных из мр3 файла
        /// </summary>
        /// <param name="filename">Имя мр3 файла</param>
        /// <param name="desc">Слово</param>
        public void Read_PCM(string filename)
        {
          //  Alvas.Audio.DsReader dr = new Alvas.Audio.DsReader(filename);
            //IntPtr formatPcm = dr.ReadFormat();
          // byte[] dataPcm = dr.ReadData();
           // FileStream f=new FileStream(filename,FileMode.Open);
          //  Alvas.Audio.Mp3Reader mr = new Alvas.Audio.Mp3Reader(f);
          //  int dur=mr.GetDurationInMS();
         //   int le = mr.GetLengthInBytes();
          //  byte[] dataPcm = mr.ReadDataInBytes(0, (int)f.Length);
         //   mr.Close();
          //  f.Close();
            
           // dr.Close();
            Alvas.Audio.Mp3Reader mr = new Alvas.Audio.Mp3Reader(File.OpenRead(filename));
            IntPtr formatMp3 = mr.ReadFormat();
            byte[] dataMp3 = mr.ReadData();
            bdata = dataMp3;
            mr.Close();
            IntPtr formatPcm = Alvas.Audio.AudioCompressionManager.GetCompatibleFormat(formatMp3, Alvas.Audio.AudioCompressionManager.PcmFormatTag);
            //mp3 -> pcm
            byte[] dataPcm1 = Alvas.Audio.AudioCompressionManager.Convert(formatMp3, formatPcm, dataMp3, false);
            List<double> data = new List<double>();
            int i = 0;
            for (i = 0; i < dataPcm1.Length - 1; i = i + 2)
            {
                short sample = (short)((dataPcm1[i + 1] << 8) | dataPcm1[i + 0]);
                /* short sample2 = BitConverter.ToInt16(buffer, index);
                Debug.Assert(sample == sample2, "Oops"); */
                float sample32 = sample / 32768f;
                data.Add(sample32);
            }

            //return null;

            normalizedData= data.ToArray();

        }

         
}
}
