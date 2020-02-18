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
using SharpSvn;
using System.Collections.ObjectModel;
using System.IO;

namespace SVN_WindowsFormsApplication
{
    public partial class Form1 : Form
    {
      private Collection<SvnLogEventArgs> logItems;
      private long CurRevision;
        private string CurComment;
        private long NextRevision;
        private string NextComment;
        public Form1()
        {
            InitializeComponent();
           // textBox4.Text = Properties.Settings.Default.



        }

        private void button1_Click(object sender, EventArgs e)
        {
          //  CurComment = 
            String[] paramsSvn = new String[] {"completesync",
               "--message=" + NextComment,
              // "D:\\temp\\svn_to_data",
               textBox4.Text,
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
            //   "D:\\temp\\svn_from",
                  textBox3.Text ,
                "--verbose",
                "--username=dimalat",
                "--password=jgRPqj3fB@XI",
                "--trust-server-cert"  };
            // SvnClient.Program.Main_consol(paramsSvn);

            textBox2.Text= ( SvnClient.Program.GetLogText(paramsSvn, "20000", out logItems));
            //    MessageBox.Show(SvnClient.Program.GetLogText(paramsSvn, "940")); 

            //// Create a FileStream that will write data to file.
            //FileStream writerFileStream =
            //    new FileStream("d:\\LogFileStreame.txt", FileMode.Create, FileAccess.Write);
            //// Save our dictionary of friends to file
            //this.formatter.Serialize(writerFileStream, logItems);

            //// Close the writerFileStream when we are done.
            //writerFileStream.Close();



        }

        private void button3_Click(object sender, EventArgs e)
        {
            String[] paramsSvn = new String[] {"completesync",
               "--message=Updating my project",
               // "D:\\temp\\svn_from",
                textBox3.Text ,
                "--verbose",
                "--username=dimalat",
                "--password=jgRPqj3fB@XI",
                "--trust-server-cert"  };
            // SvnClient.Program.Main_consol(paramsSvn);
            SvnClient.Program.UpdateToRevision(paramsSvn, TextNextRevision.Text);
          //  MessageBox.Show("Ок");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String[] paramsSvn = new String[] {"completesync",
               "--message=Updating my project",
               textBox3.Text,
              // "D:\\temp\\svn_from",
                "--verbose",
                "--username=dimalat",
                "--password=jgRPqj3fB@XI",
                "--trust-server-cert"  };
            //     SvnClient.Program.Main_consol(paramsSvn);
            CurRevision = Convert.ToInt32 (SvnClient.Program.GetInfo(paramsSvn, "20000"));
            TextCurrentRevision.Text = CurRevision.ToString();
            if (logItems != null)
                {
                //string AlllogText = "";
                for (int i = logItems.Count - 1; i > 0; i--)
                {
                    if (logItems[i].Revision > CurRevision)
                    {
                      //  MessageBox.Show( " next revision = " + logItems[i].Revision.ToString());
                        TextNextRevision.Text = logItems[i].Revision.ToString();
                        NextRevision = logItems[i].Revision;
                        NextComment = logItems[i].Time.ToString()  + " " + "Rev=" + logItems[i].Revision.ToString() + ";Author -- " + logItems[i].Author + "; " + logItems[i].LogMessage;
                        break;
                    }
                    if (logItems[i].Revision == CurRevision)
                    {
                     //   MessageBox.Show(" next revision = " + logItems[i].Revision.ToString());
                      //  TextNextRevision.Text = logItems[i].Revision.ToString();
                        CurRevision = logItems[i].Revision;
                        CurComment = logItems[i].Time.ToString() + " " + "Rev=" + logItems[i].Revision.ToString() + ";Author -- " + logItems[i].Author + "; " + logItems[i].LogMessage;
                      //  break;
                    }

                    //AlllogText = AlllogText + (Char)13 + (Char)10 +
                    //  logItems[i].Revision.ToString() + " " + logItems[i].LogMessage;
                }
            }

        //    MessageBox.Show (" Current revision = " + CurRevision.ToString());

        }

        private void button5_Click(object sender, EventArgs e)
        {
            String[] paramsSvn = new String[] {"completesync",
               "--message=Updating my project",
               "D:\\temp\\svn_from",
                "--verbose",
                "--username=dimalat",
                "--password=jgRPqj3fB@XI",
                "--trust-server-cert"  };
            //     SvnClient.Program.Main_consol(paramsSvn);
            long temp1;
            temp1 = Convert.ToInt32(TextCurrentRevision.Text );
           SvnClient.Program.Diff_ReposDiff(paramsSvn, Convert.ToInt32 (TextCurrentRevision.Text ),Convert.ToInt32(TextNextRevision.Text ));
            
        }
        static string SyncDir(string FromDir, string ToDir, Boolean  Execute=false)
        {
            Directory.CreateDirectory(ToDir);
            string Res="";

            foreach (string s1 in Directory.GetFiles(ToDir))
            {
                string s2 = FromDir + "\\" + Path.GetFileName(s1);
                if (!File.Exists(s2))
                {
                    Res = Res + (char)10 + (char)13 + "Deleting file" + (Char)9 + s1;
                    //  Console.WriteLine("Deleting file^ {0}", s1); // закомментить если не нужен вывод в консоль либо заменить на вывод куда нужно
                    if (Execute) { File.Delete(s1);}
                }
            }

            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                string s2 = ToDir + "\\" + Path.GetFileName(s1);
                //Console.WriteLine(s1); // закомментить если не нужен вывод в консоль либо заменить на вывод куда нужно
                if (!File.Exists(s2))
                {
                    Res = Res + (char)10 + (char)13 + "Copy file" + (Char)9 + s1;
                    if (Execute) { File.Copy(s1, s2); }
                }
                else
                {
                    FileInfo fi1 = new FileInfo(s1);
                    FileInfo fi2 = new FileInfo(s2);
                    if (fi1.LastWriteTime != fi2.LastWriteTime)
                    {
                        if (Execute)
                        {
                            File.SetAttributes(s2, FileAttributes.Normal);
                            File.Delete(s2);
                            File.Copy(s1, s2);
                        }
                        Res = Res + (char)10 + (char)13 + "Update file" + (Char)9 + s1;
                     //   Console.WriteLine("Update file {0} from file {1}", s1, s2); // закомментить если не нужен вывод в консоль либо заменить на вывод куда нужно
                    }
                }
            }

            foreach (string s in Directory.GetDirectories(FromDir))
            {
                string ss;
                ss = s.ToLower();

                if (ss.IndexOf(".svn") == -1)
                {
                    Res = Res + SyncDir(s, ToDir + "\\" + Path.GetFileName(s), Execute);
                }
                else
                { Res = Res;  }
               // Console.WriteLine(s); // закомментить если не нужен вывод в консоль либо заменить на вывод куда нужно
            }
            return Res;
        }
        private void button6_Click(object sender, EventArgs e)
        {
           MessageBox.Show( SyncDir(textBox3.Text, textBox4.Text, true  ));
        }

        private void button7_Click(object sender, EventArgs e)
        {
           if (logItems == null) {
                MessageBox.Show("Нужно загрузить лог");
                return;
            }
            textBox2.Text = "";

            for (int i = 0; i < logItems.Count - 20; i++)
            {
                button4_Click(sender, e);
                button3_Click(sender, e);
                Application.DoEvents();
               // if ((SyncDir("D:\\temp\\svn_from", "D:\\temp\\svn_to_data", true))!="")
                if ((SyncDir(textBox3.Text, textBox4.Text, true))!="")

                {
                    button1_Click(sender, e);
                    textBox2.AppendText(NextComment + (Char)13 + (Char)10 );               
                }

            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
