using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;

namespace Proj
{
   public  class DataBase
    {
        SqlConnection sqlConnection;
        public DataBase()
        {
            sqlConnection = new SqlConnection("server=DESKTOP-1Q2PF5M;Trusted_Connection=Yes;DataBase=InfoMovie;");
            sqlConnection.Open();
               }
        ~DataBase()
        {
            try
            {
                sqlConnection.Close();
            }
            catch (Exception ) { }
        }
        public string SearchUsers(string[] s )
        {
            SHA256CryptoServiceProvider sHA = new SHA256CryptoServiceProvider();
            DataTable dataTable = new DataTable("dataBase");
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = $"select Login,Password from Users ";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(dataTable);
            string answer = "";
            for(int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i].ItemArray[0].ToString() == s[0] && dataTable.Rows[i].ItemArray[1].ToString() == Convert.ToBase64String(sHA.ComputeHash(Encoding.UTF8.GetBytes(s[1]))))
                {
                    answer = "ok";
                    break;
                }
                else answer = "neok";
            }
            return answer;

        }
        public void CreateUsers(string[]q)
        {
            SHA256CryptoServiceProvider sHA = new SHA256CryptoServiceProvider();
            DataTable dataTable = new DataTable("dataBase");
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText =$"insert into Users(Login,Password) values('{q[0]}','{Convert.ToBase64String(sHA.ComputeHash(Encoding.UTF8.GetBytes(q[1])))}')";
            sqlCommand.ExecuteNonQuery();

        }
        public bool IsAdmin(string d)
        {
         
            DataTable dataTable = new DataTable("dataBase");
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = $"select Administrator from Users where Login='{d}' ";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(dataTable);
            return (bool)(dataTable.Rows[0].ItemArray[0].GetType() == typeof(DBNull)?false: dataTable.Rows[0].ItemArray[0]);

        }

        public DataTable Select(string selectSQL) 
        {
            DataTable dataTable = new DataTable("dataBase");
           
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = selectSQL;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); 
            sqlDataAdapter.Fill(dataTable);
        
            return dataTable;
        }
        public void Insert(string selectSQL)
        {
            DataTable dataTable = new DataTable("dataBase");
            
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = selectSQL;
            sqlCommand.ExecuteNonQuery();
           

        }

        public string Search(string question, string type, Find.Sh sh)
        {
            string rezalts = null;
            DataTable dataTable = new DataTable("dataBase");
            DataTable dataTable1 = new DataTable("dataBase");

            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = $"select Id,Genre_ids,Popularity,Vote_count,Backdrop_path,Original_language,Vote_average,Overview," +
                            $"Poster_path,Adult,Release_date,Original_title,Title,Video,Original_name,Name,Origin_country,First_air_date from Content where Id=any(select Content_Id from Content_Answer where Search_Id=(select top(1) Id from Search_History where Type='{type}' and Text_Of_Search='{question}'))";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(dataTable);

            SqlCommand sqlCommand1 = sqlConnection.CreateCommand();
            sqlCommand1.CommandText = $"select * from Genres";
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter(sqlCommand1);
            sqlDataAdapter1.Fill(dataTable1);



            if (type == "movie")
            {
                Find.AnswerMovie rez = new Find.AnswerMovie();
                rez.results = new List<Find.Movie>();
                Find.Genres genress = new Find.Genres();
                genress.genres = new List<Find.Genres.genre>();
                for (int u = 0; u < dataTable1.Rows.Count; u++)
                {
                    Find.Genres.genre genre = new Find.Genres.genre();
                    genre.id = (int)dataTable1.Rows[u].ItemArray[0];
                    genre.name = (string)dataTable1.Rows[u].ItemArray[1];
                    genress.genres.Add(genre);
                }
                for (int r = 0; r < dataTable.Rows.Count; r++)
                {
                    rez.results.Add(new Find.Movie());
                    rez.results[r].adult = (bool)dataTable.Rows[r].ItemArray[9];
                    rez.results[r].video = (bool)dataTable.Rows[r].ItemArray[13];
                    rez.results[r].vote_average = (float)(double)dataTable.Rows[r].ItemArray[6];
                    rez.results[r].vote_count = (int)dataTable.Rows[r].ItemArray[3];
                    rez.results[r].title = (string)dataTable.Rows[r].ItemArray[12];
                    rez.results[r].release_date = (string)dataTable.Rows[r].ItemArray[10];
                    rez.results[r].poster_path = (string)dataTable.Rows[r].ItemArray[8];
                    rez.results[r].popularity = (float)(double)dataTable.Rows[r].ItemArray[2];
                    rez.results[r].overview = (string)dataTable.Rows[r].ItemArray[7];
                    rez.results[r].original_title = (string)dataTable.Rows[r].ItemArray[11];
                    rez.results[r].original_language = (string)dataTable.Rows[r].ItemArray[5];
                    rez.results[r].id = (int)dataTable.Rows[r].ItemArray[0];
                    rez.results[r].backdrop_path = (string)dataTable.Rows[r].ItemArray[4];

                    string str = "";
                    int e = 0;
                    List<int> vs = new List<int>();
                    for (int i = 0; i < dataTable.Rows[r].ItemArray[1].ToString().Length; i++)
                    {
                        while (i != dataTable.Rows[r].ItemArray[1].ToString().Length)
                        {
                            if (dataTable.Rows[r].ItemArray[1].ToString()[i] == ',')
                                break;
                            str += dataTable.Rows[r].ItemArray[1].ToString()[i];
                            i++;
                        }
                        vs.Add(Convert.ToInt32(str));
                        str = "";


                    }
                    rez.results[r].genre_ids = vs.ToArray();
                }
                rez.total_results = rez.results.Count;
                if (dataTable.Rows.Count != 0)
                    rezalts = sh(rez, null, genress);
            }
            else
            {
                Find.AnswerTV rez = new Find.AnswerTV();
                rez.results = new List<Find.TV>();
                Find.Genres genress = new Find.Genres();
                genress.genres = new List<Find.Genres.genre>();
                for (int u = 0; u < dataTable1.Rows.Count; u++)
                {
                    Find.Genres.genre genre = new Find.Genres.genre();
                    genre.id = (int)dataTable1.Rows[u].ItemArray[0];
                    genre.name = (string)dataTable1.Rows[u].ItemArray[1];
                    genress.genres.Add(genre);
                }
                for (int r = 0; r < dataTable.Rows.Count; r++)
                {
                    rez.results.Add(new Find.TV());
                    rez.results[r].original_name = (string)dataTable.Rows[r].ItemArray[14];
                    rez.results[r].name = (string)dataTable.Rows[r].ItemArray[15];
                    rez.results[r].vote_average = (float)(double)dataTable.Rows[r].ItemArray[6];
                    rez.results[r].vote_count = (int)dataTable.Rows[r].ItemArray[3];
                    rez.results[r].first_air_date = (string)dataTable.Rows[r].ItemArray[17];
                    rez.results[r].poster_path = (string)dataTable.Rows[r].ItemArray[8];
                    rez.results[r].popularity = (float)(double)dataTable.Rows[r].ItemArray[2];
                    rez.results[r].overview = (string)dataTable.Rows[r].ItemArray[7];
                    rez.results[r].original_language = (string)dataTable.Rows[r].ItemArray[5];
                    rez.results[r].id = (int)dataTable.Rows[r].ItemArray[0];
                    rez.results[r].backdrop_path = (string)dataTable.Rows[r].ItemArray[4];

                    string str = "", str1 = "";
                    int e = 0;
                    List<int> vs = new List<int>();
                    List<string> vs1 = new List<string>();
                    for (int i = 0; i < dataTable.Rows[r].ItemArray[1].ToString().Length; i++)
                    {
                        while (i != dataTable.Rows[r].ItemArray[1].ToString().Length)
                        {
                            if (dataTable.Rows[r].ItemArray[1].ToString()[i] == ',')
                                break;
                            str += dataTable.Rows[r].ItemArray[1].ToString()[i];
                            i++;
                        }
                        vs.Add(Convert.ToInt32(str));
                        str = "";


                    }
                    rez.results[r].genre_ids = vs.ToArray();

                    for (int i = 0; i < dataTable.Rows[r].ItemArray[16].ToString().Length; i++)
                    {
                        while (i != dataTable.Rows[r].ItemArray[16].ToString().Length)
                        {
                            if (dataTable.Rows[r].ItemArray[16].ToString()[i] == ',')
                                break;
                            str1 += dataTable.Rows[r].ItemArray[16].ToString()[i];
                            i++;
                        }
                        vs1.Add(str1);

                        str1 = "";


                    }

                    rez.results[r].origin_country = vs1.ToArray();
                }
                rez.total_results = rez.results.Count;
                if (dataTable.Rows.Count != 0)
                    rezalts = sh(null, rez, genress);
            }
            return rezalts;
        }


    }
}
