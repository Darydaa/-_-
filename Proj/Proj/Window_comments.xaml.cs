using EO.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Proj
{
    public partial class Window_comments : Window
    { string id1 = "";
        public ObservableCollection<Comments> comment = new ObservableCollection<Comments>();

        public ObservableCollection<Comments> Comment
        {
            get { return comment; }
            set
            {
                if (value == comment) return;
                comment = value;

            }
        }

        public Window_comments(string id)
        {
            InitializeComponent();
            id1 = id;
            this.DataContext = this;
            MainWindow.comments = this;
            Task.Run(() =>
            {
                string content = "";

                content = "FindComment:"+ToB64String(id) + "<EOF>";
                byte[] data = Encoding.UTF8.GetBytes(content);
                MainWindow.stream.Write(data, 0, data.Length);
                MainWindow.stream.Flush();


            });
           
        }
        private string ToB64String(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
   
        public void CommenttAdd(string user,string text,string id)
        {
            Task.Run(() =>
            {
                string content = "";

                content = "CreateComment:" + user + "." + ToB64String(text) +"." + ToB64String(id) + "<EOF>";
                byte[] data = Encoding.UTF8.GetBytes(content);
                MainWindow.stream.Write(data, 0, data.Length);
                MainWindow.stream.Flush();
                

            });
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommenttAdd(MainWindow.userName, Text.Text, id1);
   
         }

    }

    public class Comments
    {
        public string User { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public Comments(string use,string tex,string dat)
        {
            User = use;
            Text = tex;
            Date = dat;

        }
    }
}
