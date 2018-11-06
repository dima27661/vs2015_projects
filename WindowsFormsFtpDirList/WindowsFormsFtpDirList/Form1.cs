using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WindowsFormsFtpDirList
{

    public partial class Form1 : Form
    {
        public const string UserName = "ddd";
        public const string UserPass = "ddd";
        public const string UserFtpUrl = "FTP://gg.pp.ua/";
        public Form1()
        {
            InitializeComponent();
        }




        private void button2_Click_1(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(@textBox3.Text);
            foreach (var item in dir.GetDirectories())
            {

                float folderSize = 0.0f;

                DirectoryInfo di = new DirectoryInfo(@item.FullName);

                foreach (var fi in di.GetFiles("*", SearchOption.AllDirectories))
                {
                    folderSize += fi.Length;
                }

                textBox2.AppendText(item.Name + Convert.ToChar(9) + Convert.ToString(folderSize) + Convert.ToChar(13) + Convert.ToChar(10));

                ListViewItem item1 = new ListViewItem(item.Name, 0);

                item1.Checked = true;

                //Console.WriteLine(value.ToString("0,0.00", CultureInfo.InvariantCulture));
                //Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0:0,0.00}", value));

                //item1.SubItems.Add(Convert.ToString(folderSize));

                item1.SubItems.Add( folderSize.ToString ("0 000 000 000" ));
                //item1.SubItems.Add("2");
                //item1.SubItems.Add("3");
                listView1.Items.Add(item1);

            }


            // string[] dirs = Directory.GetFiles(@textBox3.Text , "*");

            //Console.WriteLine("The number of files starting with c is {0}.", dirs.Length);
            //foreach (string dir in dirs)
            //{
            //  textBox2.AppendText(dir + Convert.ToChar(13) + Convert.ToChar(10));
            //}


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {

                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(UserFtpUrl + "out");

                ftpRequest.Credentials = new NetworkCredential(UserName, UserPass);

                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
 
               // ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                 ftpRequest.Method =  WebRequestMethods.Ftp.ListDirectoryDetails;
                /* Establish Return Communication with the FTP Server */
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */


                Stream ftpStream1 = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream1);
                /* Store the Raw Response */
                string directoryRaw = null;



                string pattern = @"^(\d+-\d+-\d+\s+\d+:\d+(?:AM|PM))\s+(<DIR>|\d+)\s+(.+)$";

                pattern = @"(\w+)\s+(poskeeper)";

                pattern =@"^([\w-]+)\s+(\d+)\s+(\w+)\s+(\w+)\s+(\d+)\s+" +
                         @"(\w+\s+\d+\s+\d+|\w+\s+\d+\s+\d+:\d+)\s+(.+)$";

                Regex regex = new Regex(pattern);
                IFormatProvider culture = CultureInfo.GetCultureInfo("en-us");


                string[] hourMinFormats =
                    new[] { "MMM dd HH:mm", "MMM dd H:mm", "MMM d HH:mm", "MMM d H:mm" };
                string[] yearFormats =
                    new[] { "MMM dd yyyy", "MMM d yyyy" };



                while (!ftpReader.EndOfStream)
                {

                    string line = ftpReader.ReadLine();

                   try
                    {
                        Match match = regex.Match(line);
                        string permissions = match.Groups[1].Value;
                        int inode = int.Parse(match.Groups[2].Value, culture);
                        string owner = match.Groups[3].Value;
                        string group = match.Groups[4].Value;
                        long size = long.Parse(match.Groups[5].Value, culture);
                        DateTime modified;
                        string s = Regex.Replace(match.Groups[6].Value, @"\s+", " ");
                        if (s.IndexOf(':') >= 0)
                        {
                            modified = DateTime.ParseExact(s, hourMinFormats, culture, DateTimeStyles.None);
                        }
                        else
                        {
                            modified = DateTime.ParseExact(s, yearFormats, culture, DateTimeStyles.None);
                        }
                        string name = match.Groups[7].Value;

                        line =   name    + ": size = " +  size.ToString("0 000 000 000")     ; 


                        //   textBox1.AppendText (  ftpReader.ReadLine() + CharCode(13)  );
                        textBox1.AppendText(line + Convert.ToChar(13) + Convert.ToChar(10));

                        Console.WriteLine(
                            "{0,-16} permissions = {1}  size = {2, 9}  modified = {3}",
                            name, permissions, size, modified.ToString("yyyy-MM-dd HH:mm"));

                    }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }



                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream1.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */

            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Return an Empty string Array if an Exception Occurs */
            // return new string[] { "" };
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string text = "-rw-r--r--    1 poskeeper  poskeeper        3776 Jan 21  2015 rsKontrAgent.RS";
            string pat = @"(\w+)\s+(car)";

           pat = @"^(\d+-\d+-\d+\s+\d+:\d+(?:AM|PM))\s+(<DIR>|\d+)\s+(.+)$";
            pat = @"(\w+)\s+(poskeeper)";
            // Instantiate the regular expression object.
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);

            // Match the regular expression pattern against a text string.
            Match m = r.Match(text);
            int matchCount = 0;
            while (m.Success)
            {
                Console.WriteLine("Match" + (++matchCount));
                for (int i = 1; i <= 2; i++)
                {
                    Group g = m.Groups[i];
                    Console.WriteLine("Group" + i + "='" + g + "'");
                    CaptureCollection cc = g.Captures;
                    for (int j = 0; j < cc.Count; j++)
                    {
                        Capture c = cc[j];
                        System.Console.WriteLine("Capture" + j + "='" + c + "', Position=" + c.Index);
                    }
                }
                m = m.NextMatch();
            }
        }



        // синхронизация каталогов 
        static void SyncDir(string FromDir, string ToDir)
        {
            Directory.CreateDirectory(ToDir);

            foreach (string s1 in Directory.GetFiles(ToDir))
            {
                string s2 = FromDir + "\\" + Path.GetFileName(s1);
                if (!File.Exists(s2))
                {
                    Console.WriteLine("Deleting file^ {0}", s1); // закомментить если не нужен вывод в консоль либо заменить на вывод куда нужно
                    File.Delete(s1);
                }
            }

            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                string s2 = ToDir + "\\" + Path.GetFileName(s1);
                //Console.WriteLine(s1); // закомментить если не нужен вывод в консоль либо заменить на вывод куда нужно
                if (!File.Exists(s2))
                {
                    File.Copy(s1, s2);
                }
                else
                {
                    FileInfo fi1 = new FileInfo(s1);
                    FileInfo fi2 = new FileInfo(s2);
                    if (fi1.LastWriteTime != fi2.LastWriteTime)
                    {
                        File.Delete(s2);
                        File.Copy(s1, s2);
                        Console.WriteLine("Update file {0} from file {1}", s1, s2); // закомментить если не нужен вывод в консоль либо заменить на вывод куда нужно
                    }
                }
            }

            foreach (string s in Directory.GetDirectories(FromDir))
            {
                SyncDir(s, ToDir + "\\" + Path.GetFileName(s));
                Console.WriteLine(s); // закомментить если не нужен вывод в консоль либо заменить на вывод куда нужно
            }
        }



        private void button5_Click(object sender, EventArgs e)
        {

                List<string> sFullFileName = new List<string>();
                List<string> sReportFileName = new List<string>();
                sFullFileName.AddRange(new string[] { "df", "1", "2" });
                sReportFileName.AddRange(new string[] { "1", "2" });
                var t = sFullFileName.Except(sReportFileName); //Есть в 1-м, нет во 2-м
                t = sFullFileName.Except(sFullFileName.Except(sReportFileName)); //Одинаковые записи

            List<string> TempList = new List<string>();
            TempList.AddRange ( sFullFileName.Except(sFullFileName.Except(sReportFileName))); 

            foreach (string elem in TempList)
                {
                    listView2.Items.Add(new ListViewItem(elem));
                }

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (FTP_Copy("D:\\projects\\php\\1001apteka_test\\index_small.html", UserFtpUrl + "index_small.html"))
            {
                MessageBox.Show("small.css Ok");
            }
      

            if ( FTP_Copy("D:\\projects\\php\\1001apteka_test\\assets\\css\\small.css", UserFtpUrl + "assets/css/small.css"))
                {
                  MessageBox.Show("Ok");
                 }

        }


        private Boolean FTP_Copy (string FilePath, string URL)
        {
            // Get the object used to communicate with the server.  
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(URL);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.  
            request.Credentials = new NetworkCredential("admin@fff.pp.ua", UserPass);

            // Copy the contents of the file to the request stream.  
            StreamReader sourceStream = new StreamReader(FilePath);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

            response.Close();
            return true;
        }


    } // end class
}
