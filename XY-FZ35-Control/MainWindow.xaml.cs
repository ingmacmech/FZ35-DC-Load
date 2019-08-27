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
        private FZ35_DCLoad device; //= new FZ35_DCLoad("COM13");
       
        private String[] value = new String[1000];
       
        

        public MainWindow()
        {
            InitializeComponent();
        }


        private void ComButton_Click(object sender, RoutedEventArgs e)
        {
            SearchForPorts();
        }

        private void SearchForPorts()
        {
            // ISSUE: After reload com ports. none is selected 
            string[] ports = SerialPort.GetPortNames();

            ComList.Items.Clear();
            foreach (string port in ports)
            {
                ComList.Items.Add(port);
            }
        }

        
        public void SetText (string str)
        {
           
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                device = new FZ35_DCLoad(ComList.SelectedItem.ToString());
                messageTextBox.AppendText("Connected to device on Port: " 
                                               + ComList.SelectedItem.ToString()
                                               + "\n");

            }
            catch (Exception)
            {
                messageTextBox.AppendText("Connect to device failed!\n");             
            }
            
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if(device != null)
            {
                device.Disconect();
                messageTextBox.AppendText("Disconnected!\n");
            }
            else
            {
                messageTextBox.AppendText("No device to disconnect\n");
            }
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            if (device.IsLogging())
            {
                device.StopLogging();
                logButton.Content = "Start Log";
            }
            else
            {
                logButton.Content = "Stop Log";
                device.StartLogging();
            }
        }

        private void OnButton_Click(object sender, RoutedEventArgs e)
        {
            if (device.IsOutputOn())
            {
                device.TurnOffLoad();
                onButton.Content = "On";
            }
            else
            {
                onButton.Content = "Off";
                device.TurnOnLoad();
            }
        }

        private void TextBoxValue_KeyEnter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                var textBox = (TextBox)sender;

                switch (textBox.Name)
                {
                    case "loadCurrentTextBox":
                        device.SetLoadCurrent(double.Parse(textBox.Text));
                        break;

                    case "ovpTextBox":
                        device.SetOverVoltageProtection(double.Parse(textBox.Text));
                        break;

                    case "lvpTextBox":
                        device.SetLowVoltageProtection(double.Parse(textBox.Text));
                        break;

                    case "ocpTextBox":
                        device.SetOverCurrentProtection(double.Parse(textBox.Text));
                        break;

                    case "oppTextBox":
                        device.SetOverPowerProtection(double.Parse(textBox.Text));
                        break;

                    case "oahTextBox":
                        device.SetMaximumCapacity(double.Parse(textBox.Text));
                        break;

                    case "ohpTextBox":
                        // TODO: Call function to set max discharge time when implemented
                        break;

                    default:
                        break;
                }
                
            }

        }
    }
}
