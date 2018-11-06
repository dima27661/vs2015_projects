using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {

        static SerialPort _serialPort;

        public Form1()
        {
            InitializeComponent();
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM3";
            _serialPort.BaudRate = 9600;
            //_serialPort.Parity = SetPortParity(_serialPort.Parity);
            //_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            //_serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            //_serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;



        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "sadf";
            _serialPort.Open();
            _serialPort.WriteLine("1234");
            _serialPort.Close();


        }
    }
}