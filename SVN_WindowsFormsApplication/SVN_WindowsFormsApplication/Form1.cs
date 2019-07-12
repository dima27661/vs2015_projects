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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            String[] paramsSvn = new String[] {"completesync",
               "--message=Updating my project",
               "D:\\temp\\svn_from",
                "--verbose",
                "--username=dimalat",
                "--password=3GDYHMB5",
                "--trust-server-cert"  };
            // SvnClient.Program.Main_consol(paramsSvn);
            MessageBox.Show(SvnClient.Program.GetLogText(paramsSvn, "940")); 

        }

        private void button3_Click(object sender, EventArgs e)
        {
            String[] paramsSvn = new String[] {"completesync",
               "--message=Updating my project",
               "D:\\temp\\svn_from",
                "--verbose",
                "--username=dimalat",
                "--password=3GDYHMB5",
                "--trust-server-cert"  };
            //     SvnClient.Program.Main_consol(paramsSvn);
            SvnClient.Program.UpdateToRevision(paramsSvn, RevisionTextBox.Text);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            String[] paramsSvn = new String[] {"completesync",
               "--message=Updating my project",
               "D:\\temp\\svn_from",
                "--verbose",
                "--username=dimalat",
                "--password=3GDYHMB5",
                "--trust-server-cert"  };
            //     SvnClient.Program.Main_consol(paramsSvn);
            MessageBox.Show ( SvnClient.Program.GetInfo (paramsSvn, "940"));

        }
    }
}
