using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Proj
{
    [ComVisible(true)]
    public class Button_Work
    {
        public void Create_Comment(string id)
        {
            Window_comments window = new Window_comments(id);
            window.Show();
        }
    }
}
