using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;

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
            connect.Open();
            command.ExecuteNonQuery();
           
            for(int j=0;j<mp3file.Frames.Length;j++)
            {
                SqlCommand command2 = new SqlCommand("INSERT MFCC (Coef1,Coef2,Coef3,Coef4,Coef5,Coef6,Coef7,Coef8,Coef9,Coef10,Coef11,Coef12,Coef13,Mp3_id) VALUES (@coef1,@coef2,@coef3,@coef4,@coef5,@coef6,@coef7,@coef8,@coef9,@coef10,@coef11,@coef12,@coef13,@id)", connect);
                SqlParameter[] mpar = new SqlParameter[13];
                for (int i = 0; i < mpar.Length; i++)
                {
                    mpar[i] = new SqlParameter("coef" + (i + 1).ToString(), SqlDbType.Float);
                    mpar[i].Value = (float)mp3file.Frames[j].MFCC[i];
                }
                command2.Parameters.AddRange(mpar);
                SqlParameter sqid = new SqlParameter("@id", SqlDbType.Int);
                sqid.Value = id;
                command2.Parameters.Add(sqid);
               // connect.Open();
                command2.ExecuteNonQuery();
               // connect.Close();
            }
            connect.Close();
                //SqlParameter par = new SqlParameter("@m3", SqlDbType.Binary);
                //par.Value = mp3file.Mp3byte;
                //command.Parameters.Add(par);
                //SqlParameter par1 = new SqlParameter("@id", SqlDbType.Int);
                //par1.Value = id;
                //command.Parameters.Add(par1);
                //SqlParameter par2 = new SqlParameter("@d", SqlDbType.NVarChar);
                //par2.Value = mp3file.Word;
                //command.Parameters.Add(par2);

               
        }

        public static int Read_from_baze(double a1, double a2, double a3, double a4)
        {
            SqlCeConnection connect = new SqlCeConnection();
            connect.ConnectionString = "Data Source=D:\\ProjectCup\\FritsBeta1\\Firts\\Database1.sdf";
            string query = "SELECT * FROM Descriptors";
            connect.Open();
            SqlCeCommand command = new SqlCeCommand(query, connect);
            var Reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            //List<string> llll = new List<string>();
            int numCheck = 0;
            while (Reader.Read() == true)
            {
                //if (Function.ResultChecking(a1, a2, a3, a4, Convert.ToDouble(Reader.GetValue(1)), Convert.ToDouble(Reader.GetValue(2)), Convert.ToDouble(Reader.GetValue(3)), Convert.ToDouble(Reader.GetValue(4))))
                  //  numCheck++;
                //llll.Add(Reader.GetValue(0).ToString() + Reader.GetValue(1).ToString() + Reader.GetValue(2).ToString());
            }
            Reader.Close();
            connect.Close();

            return numCheck;
        }

        public static void Delete()
        {
            SqlCeConnection connect = new SqlCeConnection();
            connect.ConnectionString = "Data Source=D:\\ProjectCup\\FritsBeta1\\Firts\\Database1.sdf";
            string query = "DELETE FROM Descriptors";
            SqlCeCommand command = new SqlCeCommand(query, connect);
            connect.Open();
            command.ExecuteNonQuery();
            connect.Close();
        }
    }
}
