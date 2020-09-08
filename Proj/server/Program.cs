using AwesomeLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;
using Proj;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace server
{
    public class Comments
    {
        public string User { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public Comments(string use, string tex, string dat)
        {
            User = use;
            Text = tex;
            Date = dat;

        }
    }
    class Program
    {
        static int port = 8005;
        static  DataBase dataBase = new DataBase();
        private static string ReadMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {

                bytes = stream.Read(buffer, 0, buffer.Length);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    messageData.Remove(messageData.ToString().IndexOf("<EOF>"), 5);
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();

        }
        public static string GoToDatabase( string str)
        {
          
            string[] st = str.Split('.');
            return dataBase.SearchUsers(st)+"<EOF>";

        }
        static void Main(string[] args)
        {
            //SHA256CryptoServiceProvider sHA = new SHA256CryptoServiceProvider();
            //var r=  Convert.ToBase64String(sHA.ComputeHash(Encoding.UTF8.GetBytes("admin")));
            using (StreamReader stream = new StreamReader("API.txt")){
                Find.Search.API_key = stream.ReadToEnd().Remove('\r',1).Remove('\n',1);
            }

            Start();
        }
      
      static  public async void Start()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
                    Task.Run(async() =>
                    {
                        using (NetworkStream stream = new NetworkStream(handler))
                        {

                            var str = ReadMessage(stream);
                            var str1= str.Split(':');
                            if (str1[0] == "Create")
                            {
                                var m = str1[1].Split('.');
                                dataBase.CreateUsers(m);
                            }
                            if (str1[0] == "Console")
                            {
                                while (true)
                                {
                                    var message = ReadMessage(stream).Split(':');
                                    switch (message[0])
                                    {
                                       

                                        case "Movie":
                                            {
                                                
                                              string r="Display:"+ Convert.ToBase64String(Encoding.UTF8.GetBytes(await ShowRezult("movie",message[1], Find.Search.Show)))+"<EOF>";
                                                var data1 = Encoding.UTF8.GetBytes(r);
                                                stream.Write(data1, 0, data1.Length);
                                                stream.Flush();
                                            }
                                            break;
                                        case "TV":
                                            {
                                              string r= "Display:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(await  ShowRezult("tv",message[1], Find.Search.Show)))+"<EOF>";
                                                var data1 = Encoding.UTF8.GetBytes(r);
                                                stream.Write(data1, 0, data1.Length);
                                                stream.Flush();
                                            }
                                            break;
                                    }
                                }
                            }
                            var answer = GoToDatabase(str);
                            if(answer == "ok<EOF>")
                            {
                                if (dataBase.IsAdmin(str.Split('.')[0]))
                                {
                                    var mess = $"Admin:{Find.Search.API_key}<EOF>";
                                    var data1 = Encoding.UTF8.GetBytes(mess);
                                    stream.Write(data1, 0, data1.Length);
                                    stream.Flush();
                                }
                                else {
                                    var data = Encoding.UTF8.GetBytes(answer);
                                    stream.Write(data, 0, data.Length);
                                    stream.Flush();
                                }
                                while (true)
                                {

                                    try
                                    {
                                        var message = ReadMessage(stream).Split(':');
                                        switch (message[0])
                                        {
                                            case "GetTV":
                                                {
                                                    var mess = "Display:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(await Program.ShowRezult("tv", message[1], Display))) + "<EOF>";
                                                    var data1 = Encoding.UTF8.GetBytes(mess);
                                                    stream.Write(data1, 0, data1.Length);
                                                    stream.Flush();
                                                }
                                                break;
                                            case "GetMovie":
                                                {

                                                    var mess = "Display:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(await Program.ShowRezult("movie", message[1], Display))) + "<EOF>";
                                                    var data1 = Encoding.UTF8.GetBytes(mess);
                                                    stream.Write(data1, 0, data1.Length);
                                                    stream.Flush();
                                                }
                                                break;
                                            case "FindComment":
                                                {
                                                    var comm = dataBase.Select($"select User_comm,Text,Date from Comments where Id_content={FromB64String(message[1])}");
                                                    Comments[] comments = new Comments[comm.Rows.Count];
                                                    for (int i = 0; i < comm.Rows.Count; i++)
                                                    {
                                                        comments[i] = new Comments((string)comm.Rows[i].ItemArray[0], (string)comm.Rows[i].ItemArray[1], (string)comm.Rows[i].ItemArray[2]);
                                                    }
                                                    var mess = "FindComment:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(comments))) + "<EOF>";
                                                    var data1 = Encoding.UTF8.GetBytes(mess);
                                                    stream.Write(data1, 0, data1.Length);
                                                    stream.Flush();
                                                }
                                                break;

                                            case "CreateComment":
                                                {
                                                    var sss = message[1].Split('.');
                                                    dataBase.Insert($"Insert into Comments(User_comm,Text,Id_content,Date) values('{sss[0]}','{FromB64String(sss[1])}',{FromB64String(sss[2])},'{DateTime.Now}')");
                                                    var comm = dataBase.Select($"select User_comm,Text,Date from Comments where Id_content={FromB64String(sss[2])}");
                                                    Comments[] comments = new Comments[comm.Rows.Count];
                                                    for (int i = 0; i < comm.Rows.Count; i++)
                                                    {
                                                        comments[i] = new Comments((string)comm.Rows[i].ItemArray[0], (string)comm.Rows[i].ItemArray[1], (string)comm.Rows[i].ItemArray[2]);
                                                    }
                                                    var mess = "CreateComment:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(comments))) + "<EOF>";
                                                    var data1 = Encoding.UTF8.GetBytes(mess);
                                                    stream.Write(data1, 0, data1.Length);
                                                    stream.Flush();
                                                }
                                                break;
                                            case "AdminAPI":
                                                {
                                                    var sss = message[1];
                                                    using (StreamWriter writer = new StreamWriter("API.txt", false))
                                                    {
                                                        writer.WriteLine(sss);
                                                        Find.Search.API_key = sss;
                                                    }
                                                }
                                                break;

                                        }
                                    }
                                    catch (Exception e)
                                    {

                                        IKernel kernel = new StandardKernel();
                                        ModuleLoader.Load(kernel, LoggerType.File);
                                        DIExperiment dI = kernel.Get<DIExperiment>();
                                        dI.UseLogger(e.Message);
                                    }
        }
                            }

                        }
                    });
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен

                
            }
           
        }
           private static string FromB64String(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
        public class DIExperiment
        {
            ILogger log;
            public DIExperiment(ILogger log)
            {
                this.log = log;
            }
            public void UseLogger(Object obj)
            {
                log.Log(obj.ToString(), Messages.WARNING);
            }
        }
       
            
                
           

            public static async Task<string> ShowRezult(string type, string question, Find.Sh sh)
            {
                DataBase dataBase = new DataBase();

                string content = "";
                try
                {
                    question.Replace(' ', '+');

                    string url = $"https://api.themoviedb.org/3/search/{type}?api_key={Find.Search.API_key}&query={question}";
                    if ((content = dataBase.Search(question, type, sh)) == null)
                    {

                        if (type == "movie")
                        {
                            Find.AnswerMovie rez = await Find.Search.GetRezaltAsync<Find.AnswerMovie>(url);
                            Find.Genres genres = await Find.Search.GetRezaltAsync<Find.Genres>($"https://api.themoviedb.org/3/genre/movie/list?api_key={Find.Search.API_key}&language=en-US");
                            dataBase.Insert($"insert into Search_History(Text_Of_Search,Type,User_search) values('{question}','{type}','vlad')");

                            for (int o = 0; o < rez.results.Count; o++)
                            {
                                string genrez = "";
                                for (int q = 0; q < rez.results[o].genre_ids.Length; q++)
                                {
                                    genrez += rez.results[o].genre_ids[q].ToString();
                                    genrez += ",";
                                }
                                dataBase.Insert($"Insert into Content (Genre_ids,Popularity,Vote_count,Backdrop_path,Original_language,Vote_average,Overview," +
                                    $"Poster_path,Adult,Release_date,Original_title,Title,Video) values" +
                                    $"('{genrez}',{rez.results[o].popularity.ToString(CultureInfo.CreateSpecificCulture("en-US"))},{rez.results[o].vote_count},'{rez.results[o].backdrop_path}','{rez.results[o].original_language}'" +
                                    $",{rez.results[o].vote_average.ToString(CultureInfo.CreateSpecificCulture("en-US"))}, '{rez.results[o].overview.Replace("’", "").Replace("'", "")}','{rez.results[o].poster_path}'," +
                                    $"{(rez.results[o].adult ? 1 : 0)},'{rez.results[o].release_date}','{rez.results[o].original_title.Replace("’", "").Replace("'", "")}','{rez.results[o].title.Replace("’", "").Replace("'", "")}',{(rez.results[o].video ? 1 : 0)})");
                                dataBase.Insert($"insert into Content_Answer(Search_Id,Content_Id) values((select top(1) Id from Search_History where Text_Of_Search='{question}' and Type='{type}'),(select top(1) Id from Content where Original_title='{rez.results[o].original_title}'))");

                            }

                            for (int g = 0; g < genres.genres.Count; g++)
                            {
                                try
                                {
                                    dataBase.Insert($"Insert into Genres(Id,Name) values('{genres.genres[g].id}','{genres.genres[g].name}')");
                                }
                                catch (Exception) { }
                            }
                            content = sh(rez, null, genres);


                        }
                        else
                        {
                            Find.AnswerTV reza = await Find.Search.GetRezaltAsync<Find.AnswerTV>(url);
                            Find.Genres genress = await Find.Search.GetRezaltAsync<Find.Genres>($"https://api.themoviedb.org/3/genre/tv/list?api_key={Find.Search.API_key}&language=en-US");
                            dataBase.Insert($"insert into Search_History(Text_Of_Search,Type,User_search) values('{question}','{type}','vlad')");

                            for (int o = 0; o < reza.results.Count; o++)
                            {
                                string genrez = "", countries = "";
                                for (int q = 0; q < reza.results[o].genre_ids.Length; q++)
                                {
                                    genrez += reza.results[o].genre_ids[q].ToString();
                                    genrez += ",";
                                }
                                for (int q = 0; q < reza.results[o].origin_country.Length; q++)
                                {
                                    countries += reza.results[o].origin_country[q].ToString();
                                    countries += ",";
                                }
                                dataBase.Insert($"Insert into Content(Genre_ids,Popularity,Vote_count,Backdrop_path,Original_language,Vote_average," +
                                    $"Overview,Poster_path,Original_name,Name,Origin_country,First_air_date)" +
                                    $" values('{genrez}',{reza.results[o].popularity.ToString(CultureInfo.CreateSpecificCulture("en-US"))}," +
                                    $"{reza.results[o].vote_count},'{reza.results[o].backdrop_path}','{reza.results[o].original_language}'," +
                                    $"{reza.results[o].vote_average.ToString(CultureInfo.CreateSpecificCulture("en-US"))}," +
                                    $"'{reza.results[o].overview.Replace("’", "").Replace("'", "")}','{reza.results[o].poster_path}','{reza.results[o].original_name.Replace("’", "").Replace("'", "")}','{reza.results[o].name.Replace("’", "").Replace("'", "")}','{countries}','{reza.results[o].first_air_date}')");

                                dataBase.Insert($"insert into Content_Answer(Search_Id,Content_Id) values((select top(1) Id from Search_History where Text_Of_Search='{question}' and Type='{type}'),(select top(1) Id from Content where Original_name='{reza.results[o].original_name}'))");

                            }
                            for (int g = 0; g < genress.genres.Count; g++)
                            {
                                try
                                {
                                    dataBase.Insert($"Insert into Genres(Id,Name) values('{genress.genres[g].id}','{genress.genres[g].name}')");
                                }
                                catch (Exception) { }
                            }
                            content = sh(null, reza, genress);

                        }


                    }

                }
                catch (Exception e)
                {
                   
                    IKernel kernel = new StandardKernel();
                    ModuleLoader.Load(kernel, LoggerType.File);
                    DIExperiment dI = kernel.Get<DIExperiment>();
                    dI.UseLogger(e.Message);
                }
                return content;

            }
      

      static  string Display(Find.AnswerMovie x, Find.AnswerTV tV, Find.Genres z)
        {
            string content = "<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/></head><body>";
            if (x != null)
            {
                if (x.total_results != 0)

                    for (int i = 0; i < x.total_results && i < 20; i++)
                    {

                        content += $"<div><img width=200 height=300 style=\"display:inline-block;\" src=\"https://image.tmdb.org/t/p/w300_and_h450_bestv2{x.results[i].poster_path}\">" +
                           $"<p>Название:{x.results[i].original_title}</p>" +
                            $"<p>Популярность:{x.results[i].popularity}</p>" +
                            $"<p>Рейтинг:{x.results[i].vote_average}</p>" +
                            $"<p>Описание:{x.results[i].overview}</p>" +
                            $"<p>Жанp:";

                        for (int t = 0; t < x.results[i].genre_ids.Length; t++)
                        {
                            content += z.genres.Find(o => o.id == x.results[i].genre_ids[t]).name;
                            if (x.results[i].genre_ids.Length - 1 != t)
                            {
                                content += ",";
                            }
                        }

                        content += $"</p><p>Дата реализации:{x.results[i].release_date}</p><p><button onclick=\"window.external.Create_Comment('(select top(1) Id from Content where Original_title=\\'{x.results[i].original_title}\\')')\">Комментарии</button> </p></div><br><hr></ body ></ html > ";


                    }
                else
                    content = ("<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/></head><body><h1>Ничего не найдено!</h1></body><html>");

            }
            if (tV != null)
            {
                if (tV.total_results != 0)
                    for (int i = 0; i < tV.total_results && i < 20; i++)
                    {
                        content += $"<div><img width=200 height=300 style=\"display:inline-block;\" src=\"https://image.tmdb.org/t/p/w300_and_h450_bestv2{tV.results[i].poster_path}\">" +
                                $"<p>Название:{tV.results[i].original_name}</p>" +
                                $"<p>Популярность:{tV.results[i].popularity}</p>" +
                                $"<p>Рейтинг:{tV.results[i].vote_average}</p>" +
                                $"<p>Описание:{tV.results[i].overview}</p>" +
                                $"<p>Жанр: ";

                        for (int t = 0; t < tV.results[i].genre_ids.Length; t++)
                        {
                            content += z.genres.Find(o => o.id == tV.results[i].genre_ids[t]).name;
                            if (tV.results[i].genre_ids.Length - 1 != t)
                            {
                                content += ",";
                            }
                        }

                        content += $"</p><p>Дата реализации:{tV.results[i].first_air_date}</p><p><button onclick=\"window.external.Create_Comment('(select top(1) Id from Content where Original_title=\\'{tV.results[i].original_name}\\')')\">Комментарии</button> </p></div><br><hr></ body ></ html > ";
                    }
                else content = ("<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/></head><body><h1>Ничего не найдено!</h1></body><html>");
            }

            return content;


        }


    

    }
}
