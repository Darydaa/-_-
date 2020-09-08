using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Documents;
using mshtml;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Find;

namespace Proj
{
    public partial class MainWindow : Window
    {
        public static int port = 8005; 
        public  static string address = "127.0.0.1"; 
        public static string userName = "";
        public static NetworkStream stream;
        public static Window_comments comments; 
        public MainWindow()
        {
          
            InitializeComponent();
            Button_Movie.IsChecked = true;
            Browser.ObjectForScripting = new Button_Work();
            login.GotFocus+= ChangeText;
            Input.GotFocus += ChangeText;
            loginUs.GotFocus += ChangeText;
            passwordUs.GotFocus += ChangeText;
     
        }

        public Window_comments Window_comments
        {
            get => default;
            set
            {
            }
        }

        public static string ReadMessage(NetworkStream stream)
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
        void ClientSend(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                string content = "",input="";
                bool m=false, t=false;
                Dispatcher.Invoke(() =>
                {
                    m = (bool)Button_Movie.IsChecked;
                    t = (bool)Button_TV.IsChecked;
                    input = Input.Text;
                });

                if (m)
                {
                    content="GetMovie:"+input+"<EOF>";
                    byte[] data = Encoding.UTF8.GetBytes(content);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();

                }
                else if (t)
                {
                    content = "GetTV:" + input + "<EOF>";
                    byte[] data = Encoding.UTF8.GetBytes(content);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
               
                   

            });

        }
   
      
        void ClientListen(string name,string password)
        {
            Task.Run(() =>
            {
              
                        try
                        {
                            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    
                        socket.Connect(ipPoint);
                    using (NetworkStream stream = new NetworkStream(socket))
                    {
                        string message ="";
                        message += name+"."+password+"<EOF>";
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                        var ansver = ReadMessage(stream).Split(':');

                        if (ansver[0] == "ok"||ansver[0]=="Admin")
                        {
                            MainWindow.stream = stream;
                            if (ansver[0] == "ok")
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    loginGrid.Visibility = Visibility.Hidden;
                                    searchGrid.Visibility = Visibility.Visible;
                                    userName = login.Text;
                                });

                            }
                            else
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    loginGrid.Visibility = Visibility.Hidden;
                                    admin.Visibility = Visibility.Visible;
                                    oldKey.Text = ansver[1];
                                });
                                

                            }

                            while (true)
                            {
                                var messages = ReadMessage(stream).Split(':');
                                switch (messages[0])
                                {
                           
                                    case "Display":
                                        {

                                      Dispatcher.Invoke(()=> { Browser.NavigateToString(Encoding.UTF8.GetString(Convert.FromBase64String(messages[1]))); });
                                           
                                        }
                                        break;
                                    case "FindComment":
                                        {
                                            
                                            var arr = JsonConvert.DeserializeObject<Comments[]>(FromB64String(messages[1]));
                                            comments.Dispatcher.Invoke(() =>
                                            {
                                                for (int i = 0; i < arr.Length; i++)
                                                {
                                                    comments.Comment.Add(arr[i]);
                                                }
                                            });
                                        }
                                        break;
                                    case "CreateComment":
                                        {
                                            var arr = JsonConvert.DeserializeObject<Comments[]>(FromB64String(messages[1]));
                                            
                                            comments.Dispatcher.Invoke(() =>
                                            {
                                                comments.comment.Clear();
                                                for (int i = 0; i < arr.Length; i++)
                                                {
                                                    comments.Comment.Add(arr[i]);
                                                }
                                            });
                                        }
                                        break;
                                        


                                }
                            }
                    

                        }
                        
                    }
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    
                        }
                        catch (Exception ex)
                        {
                           
                        }


                    

            });
        }
        public static string FromB64String(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
        public void Register(object sender, RoutedEventArgs e)
        {
            
                loginGrid.Visibility = Visibility.Hidden;
                searchGrid.Visibility = Visibility.Hidden;
                registrationGrid.Visibility = Visibility.Visible;
         
        }
            public void ClientAdd(object sender, RoutedEventArgs e)
        {
           

            Task.Run(() =>
            {   
                string content = "", inputLogin = "",inputPassword = "";
              
                Dispatcher.Invoke(() =>
                {
                   inputLogin=loginUs.Text;
                   inputPassword=passwordUs.Text;
                   
                });

               
                    content = "Create:" + inputLogin+"."+inputPassword + "<EOF>";
                    byte[] data = Encoding.UTF8.GetBytes(content);
            
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);
                using (NetworkStream stream = new NetworkStream(socket))
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }

            });
            Dispatcher.Invoke(() =>
            {
                loginGrid.Visibility = Visibility.Visible;
                searchGrid.Visibility = Visibility.Hidden;
                registrationGrid.Visibility = Visibility.Hidden;
            });

        }

      

        private void ChangeText(object sender, EventArgs e)
        {
            if (sender==Input)
            {
                Input.Text = "";
                Input.Foreground = Brushes.Black;
            }
            if (sender == login)
            {
                login.Text = "";
                login.Foreground = Brushes.Black;

            }
            if (sender == loginUs)
            {
                loginUs.Text = "";
                loginUs.Foreground = Brushes.Black;

            }
            if (sender == passwordUs)
            {
                passwordUs.Text = "";
                passwordUs.Foreground = Brushes.Black;

            }


        }
       
        private void serchUser_Click(object sender, RoutedEventArgs e)
        {
            ClientListen(login.Text,password.Password);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                string APIKey = "";

                Dispatcher.Invoke(() =>
                {
                    APIKey = newKey.Text;

                });
                string content = $"AdminAPI:{APIKey}<EOF>";
                byte[] data = Encoding.UTF8.GetBytes(content);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            });
            }
    }


}

