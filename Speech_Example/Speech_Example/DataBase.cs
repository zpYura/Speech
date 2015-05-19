using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Speech_Example
{
    class DataBase
    {
        public static void Insert_values(MP3 mp3file, int id)
        {
           
            SqlConnection connect = new SqlConnection();
            connect.ConnectionString = "Data Source=.;Initial Catalog=Speech;Integrated Security=True";
            SqlCommand command = new SqlCommand("INSERT MP3 (Mp3_id, Mp3_file,Description) VALUES (@id,@m3,@d)", connect);
            SqlParameter par = new SqlParameter("@m3", SqlDbType.Binary);
            par.Value = mp3file.Mp3byte;
            command.Parameters.Add(par);
            SqlParameter par1 = new SqlParameter("@id", SqlDbType.Int);
            par1.Value = id;
            command.Parameters.Add(par1);
            SqlParameter par2 = new SqlParameter("@d", SqlDbType.NVarChar);
            par2.Value = mp3file.Word;
            command.Parameters.Add(par2);
            try
            {
                connect.Open();
                command.ExecuteNonQuery();
                int w = 0;

                int[] xyc = null;
                double[,] res = null;
                alglib.kmeansgenerate(mp3file.Matrix, mp3file.Matrix.GetLength(0), mp3file.Matrix.GetLength(1), 1, 2, out w, out res, out xyc);
                //for (int j = 0; j < mp3file.Frames.Length; j++)
                //{
                    SqlCommand command2 = new SqlCommand("INSERT MFCC (Coef1,Coef2,Coef3,Coef4,Coef5,Coef6,Coef7,Coef8,Coef9,Coef10,Coef11,Coef12,Coef13,Mp3_id) VALUES (@coef1,@coef2,@coef3,@coef4,@coef5,@coef6,@coef7,@coef8,@coef9,@coef10,@coef11,@coef12,@coef13,@id)", connect);
                    SqlParameter[] mpar = new SqlParameter[13];
                    for (int i = 0; i < mpar.Length; i++)
                    {
                        mpar[i] = new SqlParameter("coef" + (i + 1).ToString(), SqlDbType.Float);
                        mpar[i].Value = (float)res[i,0];
                    }
                    command2.Parameters.AddRange(mpar);
                    SqlParameter sqid = new SqlParameter("@id", SqlDbType.Int);
                    sqid.Value = id;
                    command2.Parameters.Add(sqid);
                    command2.ExecuteNonQuery();
                //}
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();

            }
               
        }

        public static MP3 [] Read_from_baze()
        {
            SqlConnection connect = new SqlConnection();
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("Один", 0);
            dic.Add("Два", 1);
            dic.Add("Пять", 2);
            dic.Add("Шесть", 3);
            dic.Add("Семь", 4);
            connect.ConnectionString = "Data Source=.;Initial Catalog=Speech;Integrated Security=True";
            string query = "SELECT Description,MP3.Mp3_id,Coef1,Coef2,Coef3,Coef4,Coef5,Coef6,Coef7,Coef8,Coef9,Coef10,Coef11,Coef12,Coef13 FROM MFCC inner join MP3 on MFCC.Mp3_id=MP3.Mp3_id Order by MP3.Mp3_id";
            try
            {
                connect.Open();
                SqlCommand command = new SqlCommand(query, connect);
                var Reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                List<MP3> listmp3 = new List<MP3>();
                List<double> data = new List<double>();
                List<double> all_data = new List<double>();
                int id = 0;
                string descr = "";
                while (Reader.Read() == true)
                {
                    if (id != Convert.ToInt32(Reader.GetValue(1)) && id == 0)
                    {
                        id = Convert.ToInt32(Reader.GetValue(1));
                        descr = Reader.GetValue(0).ToString();
                    }

                    if (id != Convert.ToInt32(Reader.GetValue(1)))
                    {
                        MP3 m = new MP3();
                        // в поле где раньше хранилсь нормализированные данные из мр3 файла записываем значения всех mfcc коэффициентов
                        m.Matrix = new double[data.Count, 1];
                        for (int h = 0; h < data.Count; h++)
                            m.Matrix[h, 0] = data[h];
                            m.Word = descr;
                        data.Clear();
                        listmp3.Add(m);
                        id = Convert.ToInt32(Reader.GetValue(1));
                        descr = Reader.GetValue(0).ToString();
                    }

                    if (id == Convert.ToInt32(Reader.GetValue(1)))
                    {
                        for (int i = 2; i < 15; i++)
                        { 
                            data.Add(Convert.ToDouble(Reader.GetValue(i)));
                          
                        }
                    }


                }


                MP3 m1 = new MP3();
                // в поле где раньше хранилсь нормализированные данные из мр3 файла записываем значения всех mfcc коэффициентов
                m1.Matrix = new double[data.Count, 1];
                for (int h = 0; h < data.Count; h++)
                    m1.Matrix[h, 0] = data[h];
                m1.Word = descr;
                data.Clear();
                listmp3.Add(m1);

                Reader.Close();

                return listmp3.ToArray();
               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                connect.Close();
            }

           
        }

        public static void Delete(string Tablename)
        {
            SqlConnection connect = new SqlConnection();
            connect.ConnectionString = "Data Source=.;Initial Catalog=Speech;Integrated Security=True";
            string query = "DELETE FROM "+Tablename;
            SqlCommand command = new SqlCommand(query, connect);
            try
            {
                connect.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        public static int GetId()
        {
            SqlConnection connect = new SqlConnection();
            connect.ConnectionString = "Data Source=.;Initial Catalog=Speech;Integrated Security=True";
            string query = "SELECT Mp3_id FROM MFCC  MP3 Order by MP3.Mp3_id DESC";
            connect.Open();
            SqlCommand command = new SqlCommand(query, connect);
            var Reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            
            int Id = 0;
            if(Reader.Read()==true)
                Id= Convert.ToInt32(Reader.GetValue(0));
            Reader.Close();
            connect.Close();
            return Id;
        }
    }
}
