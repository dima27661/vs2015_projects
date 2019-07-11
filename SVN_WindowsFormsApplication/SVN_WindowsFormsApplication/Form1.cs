using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SvnClient;
namespace SVN_WindowsFormsApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String[] paramsSvn = new String[] {"completesync",
               "--message=Updating my project",
               "D:\\temp\\svn_to_data",
                "--verbose",
                "--username=svnuser",
                "--password=svnpass",
                "--trust-server-cert"  };
            
            SvnClient.Program.Main_consol(paramsSvn);
        }


    }
}
