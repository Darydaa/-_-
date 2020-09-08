using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AwesomeLibrary;
using Ninject;


namespace Proj
{
    
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
    public class Program
    {

       public static NetworkStream stream;
       public static void Send(string [] param)
        {
            

                if (param[1]=="movie")
                {
                    string content = "Movie:" + param[0] + "<EOF>";
                    byte[] data = Encoding.UTF8.GetBytes(content);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();

                }
                else if (param[1] == "tv")
                {
                    string content = "TV:" + param[0] + "<EOF>";
                    byte[] data = Encoding.UTF8.GetBytes(content);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }



         

        }
        public static void ConsoleSearch()
        {
            try {
                Task.Run(() =>
                {
                    int port = 8005;
                    string address = "127.0.0.1";

                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    socket.Connect(ipPoint);
                    NetworkStream stream = new NetworkStream(socket);
                    Program.stream = stream;
                    string message = "Console:<EOF>";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();

                    var messages = Proj.MainWindow.ReadMessage(stream).Split(':');
                    switch (messages[0])
                    {

                        case "Display":
                            {
                                Console.WriteLine(Proj.MainWindow.FromB64String(messages[1]));
                            }
                            break;

                    }


                });

                }
                catch (Exception ex)
                {
                IKernel kernel = new StandardKernel();
                ModuleLoader.Load(kernel, LoggerType.File);
                DIExperiment dI = kernel.Get<DIExperiment>();
                dI.UseLogger(ex.Message);
            }
           

        }

        public static string[] ConsoleCase()
        {
            string type = "";
            Console.WriteLine("Введите название:");
            string ans = Console.ReadLine();
            Console.WriteLine("Введите тип :\n 1.Фильм\n 2.Сериал");
            try
            {
                int typ = int.Parse(Console.ReadLine());
                switch (typ)
                {
                    case 1:
                        type = "movie";
                        break;
                    case 2:
                        type = "tv";
                        break;
                    default:
                        Console.WriteLine("Введено неверное значение!");
                        break;
                }
            }
            catch (Exception e)
            {
                IKernel kernel = new StandardKernel();
                ModuleLoader.Load(kernel, LoggerType.Console);
                DIExperiment dI = kernel.Get<DIExperiment>();
                dI.UseLogger("Неверное значение!");
            }

            return new string[] { ans, type };

        }

    }
}
