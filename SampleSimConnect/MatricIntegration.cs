using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.CSharp;
using SampleSimConnect;


namespace SampleSimConnect {
    public class MatricIntegration {

        UdpClient udpClient;
        IPEndPoint serverEP = new IPEndPoint(IPAddress.Loopback, API_PORT);
        public static int API_PORT = 50300;
        public static int UDP_LISTENER_PORT = 50301;
        public static string APP_NAME = "FlightSim2020";
        public static string APP_PIN = "";
        public static string APP_CLIENT = "dSgtBwC0PVzyL8JgV73Aid3ywG1ZOb5LMWLu4Exfcdo=";

        public MatricIntegration()
        {
            try { 
            udpClient = new UdpClient(UDP_LISTENER_PORT);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            }
            catch
            {
                Trace.TraceError("There was an error trying to open UDP client on port " + UDP_LISTENER_PORT + ". Please check there are no duplicate applications running. If restarting the application does not work then please reboot.");
                MessageBox.Show("There was an error trying to open UDP client on port "+ UDP_LISTENER_PORT + ". Please check there are no duplicate applications running. If restarting the application does not work then please reboot.");
                udpClient.Dispose();
                //Application.Exit();
            }
        }


        public void ReceiveCallback(IAsyncResult ar)
        {
            //Console.WriteLine("Got Call Back");
            try { 
                IPEndPoint ep = new IPEndPoint(serverEP.Address, serverEP.Port);
                byte[] receiveBytes = udpClient.EndReceive(ar, ref ep);
                string receiveString = Encoding.ASCII.GetString(receiveBytes);
                Program.UpdateClientsList(receiveString);
                //Console.WriteLine(receiveString);
            }
            catch
            {
                Trace.TraceError("No response from MATRIC");
                Form1.Global.statusLabelMatric = "No response from MATRIC..";
                Form1.setStatusMatric(false);

            }
        }


        public static Boolean CheckMatricAuthorized()
        {

            SampleSimConnect.Form1.Global.statusLabelMatric = "Attempting to Read Your MATRIC config..";



            try {
                //string ConfigFile = File.ReadAllText($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\..\Documents\.matric\config.json");
                string jsonLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\.matric\config.json";
                string ConfigFile = File.ReadAllText(jsonLocation);



                //JObject ConfigObject = JObject.Parse(ConfigFile);
                var ConfigObject = JObject.Parse(ConfigFile);

                var MatricAuthApps = ConfigObject.SelectTokens($".AuthorizedApps[?(@.appName == '" + APP_NAME + "')]");

                var tokenCount = 0;

                foreach (JToken Token in MatricAuthApps)
                {
                    tokenCount++;    
                    APP_PIN = (string)Token.SelectToken("appPIN");
                    break;
                }
            
                if (tokenCount == 0)
                {
                    //Console.WriteLine("FALSE");
                    return false;
                }
                else
                {
                    //Console.WriteLine("TRUE");
                    //Console.WriteLine(APP_PIN);
                    return true;
                }
            }
            catch
            {
                MessageBox.Show(@"A problem occured when we tried to load your MATRIC config file from { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\.matric\config.json");
                Trace.TraceError(@"A problem occured when we tried to load your MATRIC config file from { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\.matric\config.json");
                return false;
            }
        }

        public Boolean ConnectToMatric()
        {
            SampleSimConnect.Form1.Global.statusLabelMatric = "Attempting to connect to MATRIC..";
            RetryMatricAuth:
            if (!MatricIntegration.CheckMatricAuthorized())
            {
                string msg = $@"
                {{""command"":""CONNECT"", 
                    ""appName"":""{APP_NAME}""}}
                ";
                UDPSend(msg);
                SampleSimConnect.Form1.Global.statusLabelMatric = "Interface not authorised by MATRIC..";
                DialogResult dialogResult = MessageBox.Show("You need to authorise this app in MATRIC, please confirm in the MATRIC application and then click Retry to try again.", "Please authorise app", MessageBoxButtons.RetryCancel);
                if (dialogResult == DialogResult.Retry)
                {
                    goto RetryMatricAuth;
                }
                SampleSimConnect.Form1.Global.statusLabelMatric = "Could not confirm authority to connect..";
                Form1.StatusMatric = false;
                return false;
            }
            else
            {
                SampleSimConnect.Form1.Global.statusLabelMatric = "Connection established..";
                Form1.StatusMatric = true;
                return true;
            }
        }

        


        /// <summary>
        /// Initiates the connection
        /// </summary>
        public void Connect() {
            string msg = $@"
            {{""command"":""CONNECT"", 
                ""appName"":""{APP_NAME}""}}
            ";
            SampleSimConnect.Form1.Global.statusLabelMatric = "Sending connection request..";
            UDPSend(msg);
            SampleSimConnect.Form1.Global.statusLabelMatric = "Sent connection request..";
        }


        public void GetConnectedClients()
        {
            string msg = $@"
            {{""command"":""GETCONNECTEDCLIENTS"",
            ""appName"":""{APP_NAME}"",
            ""appPIN"":""{APP_PIN}""}}
            ";
            SampleSimConnect.Form1.Global.statusLabelMatric = "Requesting list of clients..";
            UDPSend(msg);
        }


        public void SetButtonProperties(string clientId, string buttonId = null, string text = null, string textcolorOn = null, string textcolorOff = null,  
            string backgroundcolorOff = null, string backgroundcolorOn = null, string imageOn = null, string imageOff = null, string buttonName = null)
        {
            //Remark: if we do not want to change a particular property, we will send it as null
            string msg = $@"
            {{""command"":""SETBUTTONPROPS"", 
                ""appName"":""{APP_NAME}"", 
                ""appPIN"":""{APP_PIN}"", 
                ""clientId"":""{clientId}"", 
                ""buttonId"": { (buttonId == null ? "null" : "\"" + buttonId + "\"") }, 
                ""buttonName"": ""{buttonName}"",
                    ""data"":{{
                        ""imageOff"": { (imageOff == null ? "null" : "\"" + imageOff + "\"") }, 
                        ""imageOn"":  { (imageOn == null ? "null" : "\"" + imageOff + "\"") }, 
                        ""textcolorOn"": { (textcolorOn == null ? "null" : "\"" + textcolorOn + "\"")}, 
                        ""textcolorOff"":{ (textcolorOff == null ? "null" : "\"" + textcolorOff + "\"")}, 
                        ""backgroundcolorOn"": { (backgroundcolorOn == null ? "null" : "\"" + backgroundcolorOn + "\"")}, 
                        ""backgroundcolorOff"":{ (backgroundcolorOff == null ? "null" : "\"" + backgroundcolorOff + "\"")}, 
                        ""text"":{ (text == null ? "null" : "\"" + text + "\"")}
                    }}
            }}";
            //MessageBox.Show(msg);
            UDPSend(msg);
        }


        public void SetButtonText(string clientId, string buttonId = null, string text = null, string buttonName = null)
        {
            //Remark: if we do not want to change a particular property, we will send it as null
            string msg = $@"
            {{""command"":""SETBUTTONPROPS"", 
                ""appName"":""{APP_NAME}"", 
                ""appPIN"":""{APP_PIN}"", 
                ""clientId"":""{APP_CLIENT}"", 
                { (buttonId == null ? "" : "buttonId:\"" + buttonId + "\", ") }
                ""buttonName"": ""{buttonName}"",
                    ""data"":{{
                        ""text"":{ (text == null ? "null" : "\"" + text + "\"")}
                    }}
            }}";
            //MessageBox.Show(msg);
            UDPSend(msg);
        }


        public void SetButtonList(string ClientId,List<SampleSimConnect.Form1.MatricButton> myButtonList)
        {

            var listTotal = myButtonList.Count;
            var listCount = 0;

            string msg = $@"
             {{""command"":""SETBUTTONPROPSEX"", 
                 ""appName"":""{APP_NAME}"", 
                 ""appPIN"":""{APP_PIN}"", 
                 ""clientId"":""{ClientId}"",
                     ""data"":[";
            
             foreach (var item in myButtonList)
             {
                listCount++;

                msg += $@"{{
                        ""buttonName"":""{item.ButtonName}"",
                        ""text"":""{item.ButtonText}""
                        }}{(listCount<listTotal ? "," : "" )} ";
            }



            msg += "]}";

            //Clipboard.SetText(msg);
            UDPSend(msg);
        }
        


        /// <summary>
        /// Sets the active page
        /// </summary>
        /// <param name="clientId">Target client id</param>
        /// <param name="pageId">Page id</param>
        public void SetActivePage(string clientId, string pageId)
        {
            string msg = $@"
            {{""command"":""SETACTIVEPAGE"", 
                ""appName"":""{APP_NAME}"", 
                ""appPIN"":""{APP_PIN}"", 
                ""clientId"":""{APP_CLIENT}"", 
                ""pageId"":""{pageId}""}}
            ";
            UDPSend(msg);
        }

        /// <summary>
        /// Sets deck and optionally page
        /// </summary>
        /// <param name="clientId">Target client id</param>
        /// <param name="deckId">deck id</param>
        /// <param name="pageId">page id</param>
        public void SetDeck(string clientId, string pageName) {

            string pageId = null;
            string deckId = "ee0fc072-649c-436c-8f55-006cbcab2c9d";
            switch (pageName)
            {
                case "MOBILE_AUTOPILOT":
                    pageId = "bd0fb447-f49e-4b3c-8cfb-43e1bd9fc1d6";
                    break;
                case "MOBILE_COMMS_PORTRAIT":
                    pageId = "289aa292-d7b4-42c9-8b41-fd2ffa97313e";
                    break;
                case "MOBILE_COMMS_LANDSCAPE":
                    pageId = "e91c8e01-54c4-4c61-9206-ae41a9d6b949";
                    break;
                case "COMMS":
                    pageId = "33ae5fb2-cfb8-495c-89d5-8ce1984c9777";
                    break;
                case "1_ENGINE_TURBOPROP":
                    pageId = "4e606141-77d3-4908-a595-11b7ecedbde8";
                    break;
                case "GA_DASHBOARD":
                    pageId = "2ac2df36-3932-44a8-8ae8-77080250da9e";
                    break;
                default:
                    pageId = "70b0da79-6dbd-4979-990f-f1e17d1bcb18";
                    break;
            }

            string msg = $@"
            {{""command"":""SETDECK"", 
                ""appName"":""{APP_NAME}"", 
                ""appPIN"":""{APP_PIN}"", 
                ""clientId"":""{clientId}"", 
                ""deckId"":""{deckId}"",
                ""pageId"":""{pageId}""}}
            ";
            UDPSend(msg);
        }


        public class VisualStateItem {
            public string buttonId;
            public string buttonName;
            public string state;

            public VisualStateItem(string buttonName, string state)
            {
                this.state = state;
                this.buttonName = buttonName;
            }
        }
        
        public void SetButtonsVisualState(string clientId, List<VisualStateItem> list) {
            string btnList = "";

            for (int i = 0; i < list.Count; i++) {
                VisualStateItem item = list[i];
                if (i != 0) {
                    btnList += ",";
                }
                btnList += $@"{{""buttonName"": ""{item.buttonName}"", ""state"":""{item.state}""}}";
            }

            string msg = $@"
            {{""command"":""SETBUTTONSVISUALSTATE"", 
                ""appName"":""{APP_NAME}"", 
                ""appPIN"":""{APP_PIN}"", 
                ""clientId"":""{clientId}"", 
                ""data"":[{btnList}]
            }}
            ";
            UDPSend(msg);
        }
        

        /// <summary>
        /// Sends UDP message to Matric server
        /// </summary>
        void UDPSend(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(bytes, bytes.Length, serverEP);
        }

        internal void setButtonList(List<Form1.MatricButton> myButtonList)
        {
            throw new NotImplementedException();
        }
    }
}
