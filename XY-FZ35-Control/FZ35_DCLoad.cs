﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace XY_FZ35_Control
{
    /// <summary>
    /// Class to control the XY-FZ35 DC Electronic Load
    /// </summary>
    class FZ35_DCLoad
    {
        // Device Comands
        private const string START_COMAND = "start";
        private const string STOP_COMAND = "stop";
        private const string TURN_ON_LOAD = "on";
        private const string TURN_OFF_LOAD = "off";
        private const string READ_SETTINGS = "read";

        // Device Response
        private const string SUCCSES_MESSAGE = "success\r";
        private const string FAIL_MESSAGE = "fail\r";

        // Error messages
        private const string VALUE_OUT_OF_RANGE = "Value to high or to low";

        // Device Limits
        private const double MAX_VOLTAGE = 25.2;
        private const double MIN_VOLTAGE = 1.5;
        private const double MAX_CURRENT = 5.1;
        private const double MIN_CURRENT = 0.0;
        private const double MAX_POWER = 35.5;

        // Device Settings
        private double ovpValue = 0.0;
        private double ocpValue = 0.0;
        private double oppValue = 0.0;
        private double capacityValue = 0.0;
        private double lvpValue = 0.0;
        private double oahValue = 0.0;
        private string ohpValue = "";


        // Private Variable
        private string portName = "";
        private SerialPort sPort = new SerialPort();
        private static Regex receivedSettingsPatern = new Regex(".*?[O][V][P][:](?<ovp>[0-9]+[.][0-9])"  + 
                                                               ".*?[O][C][P][:](?<ocp>[0-9][.][0-9]+)"  +
                                                               ".*?[O][P][P][:](?<opp>[0-9]+[.][0-9]+)" +
                                                               ".*?[L][V][P][:](?<lvp>[0-9]+[.][0-9])"  +
                                                               ".*?[O][A][H][:](?<oah>[0-9][.][0-9]+)"  +
                                                               ".*?[O][H][P][:](?<ohp>[0-9]+[:][0-9]+)", 
                                                               RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static Regex receivedLogDataPatern = new Regex(".*?(?<voltage>[0-9]+[.][0-9]+)[V]"  +
                                                               ".*?(?<current>[0-9][.][0-9]+)[A]"   +
                                                               ".*?(?<capacity>[0-9][.][0-9]+)[Ah]" +
                                                               ".*?(?<time>[0-9]+[:][0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        List<double[]> loggedData = new List<double[]>(3600); // Holds data for 1h after it has to allocate
        DateTime timeStartLogging;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName"> COM Port name ex. "COM1" </param>
        public FZ35_DCLoad(string portName)
        {
            SetPortName(portName);
            OpenCommunicationPort();
            sPort.DiscardInBuffer();
            sPort.DataReceived += DataRecivedHandler;
        }

        /// <summary>
        /// Calculates a time stamp for the logged data
        /// </summary>
        /// <returns> Returns the time since the logging started </returns>
        private double GetTimeStamp()
        {
            double timeStamp;
            TimeSpan newTimeSpan;

            newTimeSpan = DateTime.Now - timeStartLogging;
            timeStamp = newTimeSpan.TotalSeconds;

            return timeStamp;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenCommunicationPort()
        {
            sPort.BaudRate = 9600;
            sPort.DataBits = 8;
            sPort.StopBits = StopBits.One;
            sPort.Parity = Parity.None;
            sPort.PortName = portName;
            
            try
            {
                sPort.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sets the port name
        /// </summary>
        /// <param name="portName"> Serial port name ex. "COM1" </param>        
        private void SetPortName(string portName)
        {
            this.portName = portName;
        }

        /// <summary>
        /// Sets the over voltage protection of the device and stores the value.
        /// Checks if the new value is inside the limits 
        /// </summary>
        /// <param name="voltageLevel"></param>
        public void SetOverVoltageProtection(double voltageLevel)
        {
            if (voltageLevel <= MAX_VOLTAGE) //TODO: Check for minimum value
            {
                ovpValue = voltageLevel;
            }
            else
            {
                ovpValue = MAX_VOLTAGE;
                MessageBox.Show(VALUE_OUT_OF_RANGE);
            }
            sPort.WriteLine("OVP:" + ovpValue.ToString("00.0"));
        }

        /// <summary>
        /// Sets the over current protection of the device and stores the value.
        /// Checks the new value if it exceeded the Max value.  
        /// </summary>
        /// <param name="currentLevel"> Over current protection value </param>
        public void SetOverCurrentProtection(double currentLevel)
        {
            if (currentLevel <= MAX_CURRENT)
            {
                ocpValue = currentLevel;
            }
            else
            {
                ocpValue = MAX_CURRENT;
                MessageBox.Show(VALUE_OUT_OF_RANGE);
            }
            sPort.WriteLine("OCP:" + ocpValue.ToString("0.00"));
        }

        /// <summary>
        /// Sets the over power protection of the device and stores the value.
        /// Checks the new value if it exceeded the Max value.
        /// The device turns off the Input if the power exceeds this value
        /// </summary>
        /// <param name="powerLevel"> Over power protection value in W </param>
        public void SetOverPowerProtection(double powerLevel)
        {
            if (powerLevel <= MAX_POWER)
            {
                oppValue = powerLevel;
            }
            else
            {
                oppValue = MAX_POWER;
                MessageBox.Show(VALUE_OUT_OF_RANGE);
            }
            sPort.WriteLine("OPP:" + oppValue.ToString("00.00"));
        }

        /// <summary>
        /// Sets the maximum capacity of the device and stores the value.
        /// The device will turn off the input if the capacity level is reached. 
        /// </summary>
        /// <param name="capacityLevel"> Capacity value in Ah </param>
        public void SetMaximumCapacity(double capacityLevel)
        {
            /* TODO: The capacity value can be set in different ranges.
             *       Check if it's possible and how to implement it. 
            */
            capacityValue = capacityLevel;
            sPort.WriteLine("OHA:" + capacityLevel.ToString("0.000"));
        }

        /// <summary>
        /// Sets minimum voltage protection of the device and stores it.
        /// The new value is checked against the max and min boundaries.
        /// The device will turn of if this voltage is reached.
        /// This is useful to protect the battery during discharge.
        /// </summary>
        /// <param name="voltageLevel">  </param>
        public void SetLowVoltageProtection(double voltageLevel)
        {
            // TODO: Implement the max value check, and decide how to react 
            if (voltageLevel >= MIN_VOLTAGE)
            {
                lvpValue = voltageLevel;
            }
            else
            {
                lvpValue = MIN_VOLTAGE;
                MessageBox.Show(VALUE_OUT_OF_RANGE);
            }
            sPort.WriteLine("LVP:" + voltageLevel.ToString("00.0"));
        }


        public object GetRef()
        {
            return sPort;
        }


        private void StartUpload()
        {
            sPort.WriteLine(START_COMAND);
        }

        private void StopUpload()
        {
            sPort.WriteLine(STOP_COMAND);
        }

        public string ReadMessage()
        {
            return sPort.ReadLine();
        }

        public void TurnOnLoad()
        {
            sPort.WriteLine(TURN_ON_LOAD);
        }

        public void TurnOffLoad()
        {
            sPort.WriteLine(TURN_OFF_LOAD);
        }

        public void SetLoadCurrent(double setCurrent)
        {
            string test;
            test = setCurrent.ToString("N2") + "A";
            
            sPort.WriteLine(test);
        }

        public void ReadSettings()
        {
            sPort.WriteLine(READ_SETTINGS);           
        }

        public void StartLogging()
        {
            timeStartLogging = DateTime.Now;
            StartUpload();
        }

        public void StopLogging()
        {
            StopUpload();
        }


        public void DataRecivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            
            string receivedData;
            double[] logData = new double[5];

            receivedData = sPort.ReadLine();

            Match settingsMatch = receivedSettingsPatern.Match(receivedData);
            Match logDataMatch = receivedLogDataPatern.Match(receivedData);

            if ( settingsMatch.Success)
            {
                ovpValue = Double.Parse(settingsMatch.Groups["ovp"].Value);
                ocpValue = Double.Parse(settingsMatch.Groups["ocp"].Value);
                oppValue = Double.Parse(settingsMatch.Groups["opp"].Value);
                lvpValue = Double.Parse(settingsMatch.Groups["lvp"].Value);
                oahValue = Double.Parse(settingsMatch.Groups["oah"].Value);
                ohpValue = settingsMatch.Groups["ohp"].Value;
            }

            if( logDataMatch.Success)
            {
                logData[0] = GetTimeStamp();
                logData[1] = Double.Parse(logDataMatch.Groups["voltage"].Value);
                logData[2] = Double.Parse(logDataMatch.Groups["current"].Value);
                logData[3] = Double.Parse(logDataMatch.Groups["capacity"].Value);
                logData[4] = 0.0;//Double.Parse(logDataMatch.Groups["voltage"].Value);

                loggedData.Add(logData);
            }

            if(receivedData == SUCCSES_MESSAGE)
            {
                // All OK
            }

            if(receivedData == FAIL_MESSAGE)
            {
                MessageBox.Show("Command Send Fail");
            }

        }
    }

   

    

    

    
}
