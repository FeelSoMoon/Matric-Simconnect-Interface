using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.FlightSimulator.SimConnect;
using System.Diagnostics;
using System.IO;


namespace SampleSimConnect
{
    static class Program
    {
        public static MatricIntegration mtrx;
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Environment.GetCommandLineArgs(); //allow command line switches

                foreach (string arg in args)
                {
                    switch (arg.Substring(0, 2).ToUpper())
                    {
                        case "-DEBUG":
                            StartDebug();
                            break;
                        default:
                            //StartDebug(); //for dev
                            break;
                    }
                }

                Stream myFile = File.Create("debug.log");
                Trace.AutoFlush = true;
                Trace.Listeners.Add(new TextWriterTraceListener("debug.log"));
                
                try
                {
                    //open the connection to Matric
                    mtrx = new MatricIntegration();

                    mtrx.ConnectToMatric();

                    if (Form1.StatusMatric)
                    {
                        //Console.WriteLine("GetConnectedClients");
                        mtrx.GetConnectedClients();
                    }
                }
                catch
                {
                    MessageBox.Show("Unable to connect to MATRIC, Please start MATRIC and try again.");
                    Form1.setStatusMatric(false);
                    Trace.TraceError("Unable to connect to MATRIC");
                }

                Application.Run(new Form1());
            } catch(Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        public static void StartDebug()
        {
            Stream MyFile = File.Create("debug.log");
            Trace.AutoFlush = true;
            Trace.Listeners.Add(new TextWriterTraceListener("debug.log"));
            //Trace.TraceError("Error Text");
            //Trace.TraceInfo("Info Text");
            //Trace.TraceWarning("Warning Text");
        }

        public static void UpdateClientsList(string json)
        {
            JArray connectedClients = (JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            if (connectedClients.Count == 0)
            {
                //Console.WriteLine("No clients");
                //Form1.setLabelMatric("No connected devices found, make sure your smartphone/tablet is connected");
                SampleSimConnect.Form1.Global.statusLabelMatric = "No devices found connected to MATRIC..";
            }
            //Console.WriteLine("clients found");
            var deviceCount = 0;
            Form1 obj = new Form1();

            foreach (JObject client in connectedClients)
            {
                deviceCount++;

                switch (deviceCount)
                {
                    case 1:
                        Form1.Global.device1 = $@"{client.GetValue("Id")}";
                        Form1.Global.device1name = $@"{client.GetValue("Name")}";
                        obj.Device1Visible = true;
                        break;
                    case 2:
                        Form1.Global.device2 = $@"{client.GetValue("Id")}";
                        Form1.Global.device2name = $@"{client.GetValue("Name")}";
                        obj.Device2Visible = true;
                        break;
                    case 3:
                        Form1.Global.device3 = $@"{client.GetValue("Id")}";
                        Form1.Global.device3name = $@"{client.GetValue("Name")}";
                        obj.Device3Visible = true;
                        break;
                    default:
                        break;
                }
            }

            SampleSimConnect.Form1.Global.statusLabelMatric = "Matric devices found ("+deviceCount+")..";
            //return true;
        }
    }
}
