using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Proj
{
    public partial class App : Application
    {
    

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            if (e.Args.Length>0)
            {
                if (e.Args[0] == "console") {
                    console.Kernel32.AllocConsole();
                    try
                    {
                        Program.ConsoleSearch();
                        var mas = Program.ConsoleCase();
                        Program.Send(mas);
                    }catch(Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }

                    Console.ReadKey();
                    console.Kernel32.FreeConsole();
                   
                }
                Shutdown(666);
            }
            else
            {
                MainWindow wnd = new MainWindow();
                wnd.Show();
                wnd.Closed+=(x,y)=> Shutdown();
            }

        }
    }
}
