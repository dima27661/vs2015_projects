using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace SocketClient
{
    public partial class Form1 : Form
    {
        AsynchronousClient Socket1; 
        public Form1()
        {
            InitializeComponent();
            Socket1 = new SocketClient.AsynchronousClient();

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
