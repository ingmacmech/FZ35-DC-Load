using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

namespace XY_FZ35_Control
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FZ35_DCLoad leftDevice = new FZ35_DCLoad("COM13");
        private SerialPort sPort;
        private String[] value = new String[1000];
       
        

        public MainWindow()
        {
            InitializeComponent();
            sPort = (SerialPort)leftDevice.GetRef();
            //sPort.DataReceived += DataRecivedHandler;

        }


        private void ComButton_Click(object sender, RoutedEventArgs e)
        {
            SearchForPorts();
        }

        private void SearchForPorts()
        {
            string[] ports = SerialPort.GetPortNames();

            foreach(string port in ports)
            {
                ComList.Items.Add(port);
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {

            //leftDevice.ReadSettings();
            leftDevice.SetLoadCurrent(1.00);
            /*
            leftDevice.StartLogging();
            System.Threading.Thread.Sleep(200);
            leftDevice.TurnOnLoad();
            System.Threading.Thread.Sleep(10 * 1000);
            leftDevice.TurnOffLoad();
            System.Threading.Thread.Sleep(200);
            leftDevice.StopLogging();
            */




            /* 
             leftDevice.StartUpload();

             leftDevice.StopUpload();
             index = 0;
             */

        }

        public void SetText (string str)
        {
           // TestTextBox.AppendText(str);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
