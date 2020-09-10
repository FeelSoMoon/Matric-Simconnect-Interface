using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.FlightSimulator.SimConnect;
using System.Diagnostics;
using System.IO;

namespace SampleSimConnect
{
    public partial class Form1 : Form
    {

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_USER_SIMCONNECT)
            {
                if (simconnect != null)
                {
                    try{
                        simconnect.ReceiveMessage();
                    }
                    catch
                    {
                        setStatusSimConnect(false);
                        setLabelSimConnect("Connection lost to SimConnect");
                        Trace.TraceError("Connection lost to SimConnect");
                    }
                    
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }


        // User-defined win32 event
        const int WM_USER_SIMCONNECT = 0x0402;

        // SimConnect object
        SimConnect simconnect = null;

        public static bool StatusSimConnect = false;
        public static bool StatusMatric = false;


        enum DEFINITIONS
        {
            Struct1,
        }

        enum DATA_REQUESTS
        {
            REQUEST_1,
        };

        private enum EVENT_ID
        {
            TEXT,
        };

        // this is how you declare a data structure so that
        // simconnect knows how to fill it/read it.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct Struct1
        {
            public double latitude;
            public double longitude;
            public double altitude;
            public double transponder;

            /*
                declare comms
            */
            public double comm1status;
            public double comm1active;
            public double comm1standby;

            public double comm2status;
            public double comm2active;
            public double comm2standby;

            public double comm3status;
            public double comm3active;
            public double comm3standby;

            /*
                declare nav
            */
            public double nav1status;
            public double nav1active;
            public double nav1standby;

            public double nav2status;
            public double nav2active;
            public double nav2standby;

            /*
               declare adf
           */
            public double adf1signal;
            public double adf1active;
            public double adf1standby;
            public double adf2signal;
            public double adf2active;
            public double adf2standby;


            // declare autopilot
            public double autopilot_master;

            public double autopilot_heading_lock;
            public double autopilot_heading_lock_dir;

            public double autopilot_altitude_lock;
            public double autopilot_altitude_lock_var;

            public double autopilot_approach_hold;
            public double autopilot_vertical_hold_var;
            public double autopilot_pitch_hold;

            public double autopilot_flight_director_active;

            public double autopilot_airspeed_hold;
            public double autopilot_airspeed_hold_var;

            public double autopilot_yaw_damper;

            public double autopilot_nav1_lock;
            public double autopilot_vertical_hold;

            public double autopilot_wing_leveler;

            public double flap_angle;
            public double flap_positions;
            public double trim_position;
            public double altimeter;

            public double light_strobe;
            public double light_panel;
            public double light_landing;
            public double light_taxi;
            public double light_beacon;
            public double light_nav;
            public double light_logo;
            public double light_wing;
            public double light_recognition;
            public double light_cabin;
        };

        public class Global
        {

            //declare status globals
            public static string statusLabelMatric;
            public static string statusLabelSimConnect;
            public static string statusMatric;
            public static string statusSimConnect;

            //declare global others
            public static string latitude;
            public static string longitude;
            public static string altitude;
            public static string transponder;

            // declare global comms
            public static string comm1status;
            public static string comm1active;
            public static string comm1standby;

            public static string comm2status;
            public static string comm2active;
            public static string comm2standby;

            public static string comm3status;
            public static string comm3active;
            public static string comm3standby;

            // declare global nav
            public static string nav1status;
            public static string nav1active;
            public static string nav1standby;

            public static string nav2status;
            public static string nav2active;
            public static string nav2standby;

            // declare global adf
            public static string adf1signal;
            public static string adf1active;
            public static string adf1standby;
            public static string adf2signal;
            public static string adf2active;
            public static string adf2standby;

            //devices
            public static string device1 = null;
            public static string device2 = null;
            public static string device3 = null;

            public static string device1name = null;
            public static string device2name = null;
            public static string device3name = null;

            public static bool device1updated = false;
            public static bool device2updated = false;
            public static bool device3updated = false;

            // declare autopilot
            public static string autopilot_master; //

            public static string autopilot_heading_lock; //
            public static string autopilot_heading_lock_dir;

            public static string autopilot_altitude_lock; //
            public static string autopilot_altitude_lock_var;

            public static string autopilot_approach_hold; //
            public static string autopilot_vertical_hold_var;
            public static string autopilot_pitch_hold; //

            public static string autopilot_flight_director_active; //

            public static string autopilot_airspeed_hold; //
            public static string autopilot_airspeed_hold_var;

            public static string autopilot_yaw_damper; //

            public static string autopilot_nav1_lock; //
            public static string autopilot_vertical_hold; //

            public static string autopilot_wing_leveler;

            public static string flap_angle;
            public static string flap_positions;
            public static string trim_position;

            public static string altimeter;

            public static string light_strobe;
            public static string light_panel;
            public static string light_landing;
            public static string light_taxi;
            public static string light_beacon;
            public static string light_nav;
            public static string light_logo;
            public static string light_wing;
            public static string light_recognition;
            public static string light_cabin;
        };




        public Form1()
        {
            InitializeComponent();
            labelMatricStatus.Text = Global.statusLabelMatric;
            try
            {
                //Open the connection to SimConnect
                simconnect = new SimConnect("Sample", this.Handle, WM_USER_SIMCONNECT, null, 0);
            }
            catch
            {
                MessageBox.Show("Flight Sim / SimConnect is not running or cannot be detected.");
                setStatusSimConnect(false);
                Global.statusLabelSimConnect="Flight Sim/SimConnect is not running.";
                Trace.TraceError("Unable to open SimConnect connection");
            }

            labelMatricStatus.Text = Global.statusLabelMatric;

            // Initialize data requests operations
            initDataRequest();
        }

        public string labelStatusSimConnect
        {
            get { return labelSimConnectStatus.Text; }
            set { labelSimConnectStatus.Text = Global.statusLabelSimConnect; }
        }

        public string labelStatusMatric
        {
            get { return labelMatricStatus.Text; }
            set { labelMatricStatus.Text = Global.statusLabelMatric; }
        }

        public bool Device1Check
        {
            get { return checkDevice1.Checked; }
            set { checkDevice1.Checked = value; }
        }

        public bool Device1Visible
        {
            get { return groupDevice1.Visible; }
            set { groupDevice1.Visible = value; }
        }

        public string Device1ID
        {
            get { return textDevice1ID.Text; }
            set { textDevice1ID.Text = value; }
        }

        public string Device1Page
        {
            get { return comboDevice1page.Text; }
            set { comboDevice1page.Text = value; }
        }

        public bool Device2Check
        {
            get { return checkDevice2.Checked; }
            set { checkDevice2.Checked = value; }
        }

        public bool Device2Visible
        {
            get { return groupDevice2.Visible; }
            set { groupDevice2.Visible = value; }
        }

        public string Device2ID
        {
            get { return textDevice2ID.Text; }
            set { textDevice2ID.Text = value; }
        }

        public string Device2Page
        {
            get { return comboDevice2page.Text; }
            set { comboDevice2page.Text = value; }
        }

        public bool Device3Check
        {
            get { return checkDevice3.Checked; }
            set { checkDevice3.Checked = value; }
        }

        public bool Device3Visible
        {
            get { return groupDevice3.Visible; }
            set { groupDevice3.Visible = value; }
        }

        public string Device3ID
        {
            get { return textDevice3ID.Text; }
            set { textDevice3ID.Text = value; }
        }

        public string Device3Page
        {
            get { return comboDevice3page.Text; }
            set { comboDevice3page.Text = value; }
        }

        // Simconnect client will send a win32 message when there is 
        // a packet to process. ReceiveMessage must be called to
        // trigger the events. This model keeps simconnect processing on the main thread.


        private void initDataRequest()
        {
            try
            {

                if (simconnect != null)
                {

                    // listen to connect and quit msgs
                    simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
                    simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);

                    // listen to exceptions
                    simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);

                    // define a data structure
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);


                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "TRANSPONDER CODE:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);


                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM STATUS:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM ACTIVE FREQUENCY:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM STANDBY FREQUENCY:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM STATUS:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM ACTIVE FREQUENCY:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM STANDBY FREQUENCY:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM STATUS:3", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM ACTIVE FREQUENCY:3", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "COM STANDBY FREQUENCY:3", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);


                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV AVAILABLE:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV ACTIVE FREQUENCY:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV STANDBY FREQUENCY:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV AVAILABLE:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV ACTIVE FREQUENCY:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "NAV STANDBY FREQUENCY:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);


                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ADF SIGNAL:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ADF ACTIVE FREQUENCY:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ADF STANDBY FREQUENCY:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ADF SIGNAL:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ADF ACTIVE FREQUENCY:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ADF STANDBY FREQUENCY:2", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);


                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT MASTER", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT HEADING LOCK", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT HEADING LOCK DIR", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT ALTITUDE LOCK", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT ALTITUDE LOCK VAR", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT APPROACH HOLD", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT VERTICAL HOLD VAR", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT PITCH HOLD", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT FLIGHT DIRECTOR ACTIVE", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT AIRSPEED HOLD", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT AIRSPEED HOLD VAR", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT YAW DAMPER", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT NAV1 LOCK", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT VERTICAL HOLD", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "AUTOPILOT WING LEVELER", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "TRAILING EDGE FLAPS LEFT ANGLE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "FLAPS NUM HANDLE POSITIONS", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "ELEVATOR TRIM PCT", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "KOHLSMAN SETTING HG", "inHg", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT STROBE", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT PANEL", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT LANDING", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT TAXI", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT BEACON", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT NAV", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT LOGO", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT WING", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT RECOGNITION", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "LIGHT CABIN", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);


        // IMPORTANT: register it with the simconnect managed wrapper marshaller
        // if you skip this step, you will only receive a uint in the .dwData field.
        simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

                    // catch a simobject data request
                    simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(simconnect_OnRecvSimobjectData);

                    // Request FSX data every second
                    simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1
                         , DEFINITIONS.Struct1
                         , SimConnect.SIMCONNECT_OBJECT_ID_USER
                         , SIMCONNECT_PERIOD.SECOND
                         , SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT
                         , 0
                         , 0
                         , 0);

                }

            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.Message);
                Trace.TraceError(ex.Message);
            }
        }

        public class MatricButton
        {
            public string ButtonName;
            public string ButtonText;
        }

        void simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            switch ((DATA_REQUESTS)data.dwRequestID)
            {
                case DATA_REQUESTS.REQUEST_1:
                    Struct1 s1 = (Struct1)data.dwData[0];

                    /* also superfluous, don't need this part of the sample
                     String textToDisplay = "Lat: " +  Convert.ToString(s1.latitude)
                        + "   Lon: " + Convert.ToString(s1.longitude)
                        + "   Alt: " + Convert.ToString(s1.altitude) ;
                    */

                    // take this away, we don't want to send it to the game
                    // simconnect.Text(SIMCONNECT_TEXT_TYPE.PRINT_WHITE, 0, EVENT_ID.TEXT, textToDisplay);

                    labelMatricStatus.Text = Global.statusLabelMatric;
                    /*if (!StatusMatric)
                    {
                        this.retryMatricConnection.Visible = true;
                    }*/


                    // set global transponder
                    /*
                    if (s1.transponder < 10)
                    {
                        Global.transponder = "000" + Convert.ToString(s1.transponder);
                    }else if(s1.transponder < 100)
                    {
                        Global.transponder = "00" +Convert.ToString(s1.transponder);
                    }else if(s1.transponder < 1000)
                    {
                        Global.transponder = "0" + Convert.ToString(s1.transponder);
                    }
                    else { 
                        Global.transponder = Convert.ToString(s1.transponder);
                    }*/

                    Global.longitude = Convert.ToString(s1.longitude);
                    Global.latitude = Convert.ToString(s1.latitude);

                    Global.transponder = (s1.transponder < 10? "0" : "")+ (s1.transponder < 100 ? "0" : "")+ (s1.transponder < 1000 ? "0" : "") + Convert.ToString(s1.transponder);

                    // set global comms
                    Global.comm1status = Convert.ToString(s1.comm1status);
                    Global.comm1active = FormatFreq(Convert.ToString(s1.comm1active));
                    Global.comm1standby = FormatFreq(Convert.ToString(s1.comm1standby));

                    Global.comm2status = Convert.ToString(s1.comm2status);
                    Global.comm2active = FormatFreq(Convert.ToString(s1.comm2active));
                    Global.comm2standby = FormatFreq(Convert.ToString(s1.comm2standby));


                    Global.comm3status = Convert.ToString(s1.comm3status);
                    Global.comm3active = FormatFreq(Convert.ToString(s1.comm3active));
                    Global.comm3standby = FormatFreq(Convert.ToString(s1.comm3standby));

                    // set global nav
                    Global.nav1status = Convert.ToString(s1.nav1status);
                    Global.nav1active = FormatFreq(Convert.ToString(s1.nav1active));
                    Global.nav1standby = FormatFreq(Convert.ToString(s1.nav1standby));


                    Global.nav2status = Convert.ToString(s1.nav2status);
                    Global.nav2active = FormatFreq(Convert.ToString(s1.nav2active));
                    Global.nav2standby = FormatFreq(Convert.ToString(s1.nav2standby));

                    
                    // set global adf
                    Global.adf1signal = Convert.ToString(s1.adf1signal);
                    Global.adf1active = FormatADF(s1.adf1active);
                    Global.adf1standby = FormatADF(s1.adf1standby);


                    Global.adf2signal = Convert.ToString(s1.adf2signal);
                    Global.adf2active = FormatADF(s1.adf2active);
                    Global.adf2standby = FormatADF(s1.adf2standby);
                    
                    /*
                    Console.WriteLine("ap master:" + s1.autopilot_master);
                    Console.WriteLine("heading lock:" + s1.autopilot_heading_lock);
                    Console.WriteLine("heading lock dir:" + s1.autopilot_heading_lock_dir);
                    Console.WriteLine("alt lock:" + s1.autopilot_altitude_lock);
                    Console.WriteLine("alt lock var:" + s1.autopilot_altitude_lock_var);
                    Console.WriteLine("apr:" + s1.autopilot_approach_hold);
                    Console.WriteLine("vspeed var:" + s1.autopilot_vertical_hold_var);
                    Console.WriteLine("pitch hold:" + s1.autopilot_pitch_hold);
                    Console.WriteLine("fd:" + s1.autopilot_flight_director_active);
                    Console.WriteLine("speed hold:" + s1.autopilot_airspeed_hold);
                    Console.WriteLine("speed holdvar:" + s1.autopilot_airspeed_hold_var);
                    Console.WriteLine("yd:" + s1.autopilot_yaw_damper);
                    Console.WriteLine("nav1:" + s1.autopilot_nav1_lock);
                    Console.WriteLine("vhold:" + s1.autopilot_vertical_hold);
                    */
                    //Console.WriteLine();
                 

                    Global.autopilot_master = Convert.ToString(s1.autopilot_master);

                    Global.autopilot_heading_lock = Convert.ToString(s1.autopilot_heading_lock);
                    Global.autopilot_heading_lock_dir = Convert.ToString(Math.Round(s1.autopilot_heading_lock_dir));

                    Global.autopilot_altitude_lock = Convert.ToString(s1.autopilot_altitude_lock);
                    Global.autopilot_altitude_lock_var = Convert.ToString(Math.Round(s1.autopilot_altitude_lock_var));

                    Global.autopilot_approach_hold = Convert.ToString(s1.autopilot_approach_hold);
                    Global.autopilot_vertical_hold_var = Convert.ToString(Math.Round(s1.autopilot_vertical_hold_var,0)*200); //Convert.ToString(s1.autopilot_vertical_hold_var);
                    Global.autopilot_pitch_hold = Convert.ToString(s1.autopilot_pitch_hold);

                    Global.autopilot_flight_director_active = Convert.ToString(s1.autopilot_flight_director_active);

                    Global.autopilot_airspeed_hold = Convert.ToString(s1.autopilot_airspeed_hold);
                    Global.autopilot_airspeed_hold_var = Convert.ToString(Math.Round(s1.autopilot_airspeed_hold_var));

                    Global.autopilot_yaw_damper = Convert.ToString(s1.autopilot_yaw_damper);

                    Global.autopilot_nav1_lock = Convert.ToString(s1.autopilot_nav1_lock);
                    Global.autopilot_vertical_hold = Convert.ToString(s1.autopilot_vertical_hold);
                    Global.autopilot_wing_leveler = Convert.ToString(s1.autopilot_wing_leveler);


                    Global.flap_angle = Convert.ToString(Math.Round(s1.flap_angle,0));

                    Global.flap_positions = Convert.ToString(s1.flap_positions);
                    //Console.WriteLine(Global.flap_positions);

                    Global.trim_position = Convert.ToString(Math.Round(s1.trim_position*100, 0))+"%";

                    string altimeter = Convert.ToString(Math.Round(s1.altimeter, 2));
                    Global.altimeter = (altimeter.Length == 4 ? altimeter + "0" : altimeter);

                    Global.light_strobe = Convert.ToString(s1.light_strobe); 
                    Global.light_panel = Convert.ToString(s1.light_panel);
                    Global.light_landing = Convert.ToString(s1.light_landing);
                    Global.light_taxi = Convert.ToString(s1.light_taxi);
                    Global.light_beacon = Convert.ToString(s1.light_beacon);
                    Global.light_nav = Convert.ToString(s1.light_nav);
                    Global.light_logo = Convert.ToString(s1.light_logo);
                    Global.light_wing = Convert.ToString(s1.light_wing);
                    Global.light_recognition = Convert.ToString(s1.light_recognition);
                    Global.light_cabin = Convert.ToString(s1.light_cabin);






                    var myButtonList = new List<MatricButton>();

                    myButtonList.Add(new MatricButton { ButtonName = "TRANSPONDER_DISPLAY", ButtonText = Global.transponder });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM1ACTIVE", ButtonText = Global.comm1active });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM1STANDBY", ButtonText = Global.comm1standby });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM2ACTIVE", ButtonText = Global.comm2active });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM2STANDBY", ButtonText = Global.comm2standby });
                    //myButtonList.Add(new MatricButton { ButtonName = "COMM3ACTIVE", ButtonText = Global.comm3active });
                    //myButtonList.Add(new MatricButton { ButtonName = "COMM3STANDBY", ButtonText = Global.comm3standby });
                    myButtonList.Add(new MatricButton { ButtonName = "NAV1ACTIVE", ButtonText = Global.nav1active });
                    myButtonList.Add(new MatricButton { ButtonName = "NAV1STANDBY", ButtonText = Global.nav1standby });
                    myButtonList.Add(new MatricButton { ButtonName = "NAV2ACTIVE", ButtonText = Global.nav2active });
                    myButtonList.Add(new MatricButton { ButtonName = "NAV2STANDBY", ButtonText = Global.nav2standby });

                    myButtonList.Add(new MatricButton { ButtonName = "ADF1ACTIVE", ButtonText = Global.adf1active });
                    myButtonList.Add(new MatricButton { ButtonName = "ADF1STANDBY", ButtonText = Global.adf1standby });

                    myButtonList.Add(new MatricButton { ButtonName = "FLAP_DISPLAY", ButtonText = Global.flap_angle });
                    myButtonList.Add(new MatricButton { ButtonName = "TRIM_DISPLAY", ButtonText = Global.trim_position });

                    myButtonList.Add(new MatricButton { ButtonName = "AP_HEADING_DISPLAY", ButtonText = Global.autopilot_heading_lock_dir });
                    myButtonList.Add(new MatricButton { ButtonName = "AP_SPEED_DISPLAY", ButtonText = Global.autopilot_airspeed_hold_var });
                    myButtonList.Add(new MatricButton { ButtonName = "AP_VS_DISPLAY", ButtonText = Global.autopilot_vertical_hold_var });
                    myButtonList.Add(new MatricButton { ButtonName = "AP_ALT_DISPLAY", ButtonText = Global.autopilot_altitude_lock_var });
                    myButtonList.Add(new MatricButton { ButtonName = "ALTIMETER_DISPLAY", ButtonText = Global.altimeter });

                    //ADDING GA DASHBOARD ELEMENTS
                    myButtonList.Add(new MatricButton { ButtonName = "TRANSPONDER_DISPLAY2", ButtonText = Global.transponder });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM1ACTIVE2", ButtonText = Global.comm1active });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM1STANDBY2", ButtonText = Global.comm1standby });
                    myButtonList.Add(new MatricButton { ButtonName = "AP_HEADING_DISPLAY2", ButtonText = Global.autopilot_heading_lock_dir });
                    myButtonList.Add(new MatricButton { ButtonName = "AP_SPEED_DISPLAY2", ButtonText = Global.autopilot_airspeed_hold_var });
                    myButtonList.Add(new MatricButton { ButtonName = "AP_VS_DISPLAY2", ButtonText = Global.autopilot_vertical_hold_var });
                    myButtonList.Add(new MatricButton { ButtonName = "AP_ALT_DISPLAY2", ButtonText = Global.autopilot_altitude_lock_var });
                    myButtonList.Add(new MatricButton { ButtonName = "ALTIMETER_DISPLAY2", ButtonText = Global.altimeter });

                    //ADDING MOBILE ELEMENTS
                    myButtonList.Add(new MatricButton { ButtonName = "TRANSPONDER_DISPLAY_MOBILE", ButtonText = Global.transponder });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM1ACTIVE_MOBILE", ButtonText = Global.comm1active });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM1STANDBY_MOBILE", ButtonText = Global.comm1standby });
                    myButtonList.Add(new MatricButton { ButtonName = "NAV1ACTIVE_MOBILE", ButtonText = Global.nav1active });
                    myButtonList.Add(new MatricButton { ButtonName = "NAV1STANDBY_MOBILE", ButtonText = Global.nav1standby });
                    myButtonList.Add(new MatricButton { ButtonName = "ALTIMETER_DISPLAY_MOBILE", ButtonText = Global.altimeter });

                    myButtonList.Add(new MatricButton { ButtonName = "TRANSPONDER_DISPLAY_MOBILE2", ButtonText = Global.transponder });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM1ACTIVE_MOBILE2", ButtonText = Global.comm1active });
                    myButtonList.Add(new MatricButton { ButtonName = "COMM1STANDBY_MOBILE2", ButtonText = Global.comm1standby });
                    myButtonList.Add(new MatricButton { ButtonName = "NAV1ACTIVE_MOBILE2", ButtonText = Global.nav1active });
                    myButtonList.Add(new MatricButton { ButtonName = "NAV1STANDBY_MOBILE2", ButtonText = Global.nav1standby });
                    myButtonList.Add(new MatricButton { ButtonName = "ALTIMETER_DISPLAY_MOBILE2", ButtonText = Global.altimeter });
                    myButtonList.Add(new MatricButton { ButtonName = "FLAP_DISPLAY_MOBILE", ButtonText = Global.flap_angle });
                    myButtonList.Add(new MatricButton { ButtonName = "TRIM_DISPLAY_MOBILE", ButtonText = Global.trim_position });
                    myButtonList.Add(new MatricButton { ButtonName = "FLAP_DISPLAY_AIRBUS", ButtonText = Global.flap_angle });
                    myButtonList.Add(new MatricButton { ButtonName = "TRIM_DISPLAY_AIRBUS", ButtonText = Global.trim_position });




                    var myButtonState = new List<MatricIntegration.VisualStateItem>();

                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_ap",(Global.autopilot_master == "1" ? "on" : "off" )));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_lvl", (Global.autopilot_wing_leveler == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_fd", (Global.autopilot_flight_director_active == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_yd", (Global.autopilot_yaw_damper == "1" ? "on" : "off")));

                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_ias", (Global.autopilot_airspeed_hold == "1" ? "on" : "off")));
                    //myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_vnv", (Global.autopilot_ == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_vs", (Global.autopilot_vertical_hold == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_alt", (Global.autopilot_altitude_lock == "1" ? "on" : "off")));

                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_apr", (Global.autopilot_approach_hold == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_nav", (Global.autopilot_nav1_lock == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_hdg", (Global.autopilot_heading_lock == "1" ? "on" : "off")));

                    //watch toggle states of ga dashboard
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_ap2", (Global.autopilot_master == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_lvl2", (Global.autopilot_wing_leveler == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_fd2", (Global.autopilot_flight_director_active == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_yd2", (Global.autopilot_yaw_damper == "1" ? "on" : "off")));

                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_ias2", (Global.autopilot_airspeed_hold == "1" ? "on" : "off")));
                    //myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_vnv", (Global.autopilot_ == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_vs2", (Global.autopilot_vertical_hold == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_alt2", (Global.autopilot_altitude_lock == "1" ? "on" : "off")));

                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_apr2", (Global.autopilot_approach_hold == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_nav2", (Global.autopilot_nav1_lock == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("autopilot_hdg2", (Global.autopilot_heading_lock == "1" ? "on" : "off")));

                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_strobe", (Global.light_strobe == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_panel", (Global.light_panel == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_landing", (Global.light_landing == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_taxi", (Global.light_taxi == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_beacon", (Global.light_beacon == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_nav", (Global.light_nav == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_logo", (Global.light_logo == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_wing", (Global.light_wing == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_recognition", (Global.light_recognition == "1" ? "on" : "off")));
                    myButtonState.Add(new MatricIntegration.VisualStateItem("light_cabin", (Global.light_cabin == "1" ? "on" : "off")));

                    this.textLong.Text = Global.longitude;
                    this.textLat.Text = Global.latitude;

                    double TargetLong;
                    double TargetLat;
                    double TargetDistance;
                    double TargetBearing;

                    if(this.textTargetLong.Text.Length > 0 && this.textTargetLat.Text.Length > 0)
                    {
                        try
                        {
                            TargetLong = Convert.ToDouble(this.textTargetLong.Text);
                            TargetLat = Convert.ToDouble(this.textTargetLat.Text);

                            TargetDistance = getDistanceFromLatLonInNM(Convert.ToDouble(Global.latitude), Convert.ToDouble(Global.longitude), TargetLat, TargetLong);

                            TargetBearing = DegreeBearing(Convert.ToDouble(Global.latitude), Convert.ToDouble(Global.longitude), TargetLat, TargetLong);

                            this.textTargetDistance.Text = Convert.ToString(TargetDistance);
                            this.textTargetBearing.Text = Convert.ToString(TargetBearing);
                        }
                        catch
                        {
                            Trace.TraceError("Error handling long/lat targeting");
                        }
                    }



                    this.textTransponder.Text = Global.transponder;

                    this.textComm1Status.Text = FormatCommStatus(Global.comm1status);
                    this.textComm1Active.Text = Global.comm1active;
                    this.textComm1Standby.Text = Global.comm1standby;

                    this.textComm2Status.Text = FormatCommStatus(Global.comm2status);
                    this.textComm2Active.Text = Global.comm2active;
                    this.textComm2Standby.Text = Global.comm2standby;

                    this.textComm3Status.Text = FormatCommStatus(Global.comm3status);
                    this.textComm3Active.Text = Global.comm3active;
                    this.textComm3Standby.Text = Global.comm3standby;

                    this.textNav1Status.Text = FormatCommStatus(Global.nav1status);
                    this.textNav1Active.Text = Global.nav1active;
                    this.textNav1Standby.Text = Global.nav1standby;

                    this.textNav2Status.Text = FormatCommStatus(Global.nav2status);
                    this.textNav2Active.Text = Global.nav2active;
                    this.textNav2Standby.Text = Global.nav2standby;

                    this.textADF1Status.Text = Global.adf1signal;
                    this.textADF1Active.Text = Global.adf1active;
                    this.textADF1Standby.Text = Global.adf1standby;

                    this.textADF2Status.Text = Global.adf2signal;
                    this.textADF2Active.Text = Global.adf2active;
                    this.textADF2Standby.Text = Global.adf2standby;

                    this.textAPaltitude.Text = Global.autopilot_altitude_lock_var;
                    this.textAPheading.Text = Global.autopilot_heading_lock_dir;
                    this.textAPspeed.Text = Global.autopilot_airspeed_hold_var;
                    this.textAPvspeed.Text = Global.autopilot_vertical_hold_var;

                    if(SampleSimConnect.Form1.StatusMatric==false)
                    {
                        this.checkDevice1.Checked = false;
                        this.checkDevice2.Checked = false;
                        this.checkDevice3.Checked = false;
                    }

                    if (Global.device1 != null)
                    {
                        groupDevice1.Text = Global.device1name;
                        groupDevice1.Visible = true;
                        textDevice1ID.Text = Global.device1;



                        if (checkDevice1.Checked == true)
                        {
                            if (Global.device1updated == false)
                            {
                                Global.device1updated = true;
                                //MessageBox.Show(this.comboDevice1page.Text);
                                Program.mtrx.SetDeck(Global.device1, this.comboDevice1page.Text);
                                //Console.WriteLine(this.comboDevice1page.Text);
                                //Thread.Sleep(1000);
                            }

                            //mtrx = new MatricIntegration();
                            Program.mtrx.SetButtonList(Global.device1, myButtonList);
                            Program.mtrx.SetButtonsVisualState(Global.device1,myButtonState);

                            
                        }

                    }

                    if (Global.device2 != null)
                    {
                        groupDevice2.Text = Global.device2name;
                        groupDevice2.Visible = true;
                        textDevice2ID.Text = Global.device2;



                        if (checkDevice2.Checked == true)
                        {
                            if (Global.device2updated == false)
                            {
                                Global.device2updated = true;
                                Program.mtrx.SetDeck(Global.device2, this.comboDevice2page.Text);
                                //Thread.Sleep(1000);
                            }

                            //mtrx = new MatricIntegration();
                            Program.mtrx.SetButtonList(Global.device2, myButtonList);
                        }

                    }



                    break;

                default:
                    MessageBox.Show("Unknown request ID: " + data.dwRequestID);
                    break;
            }
        }


        static string FormatFreq(string input)
        {

            string output;

            if(input != "0")
            { 
                output = input.Substring(0, 3) + "." + input.Substring(3, 3);
            }
            else
            {
                output = input;
            }
            return output;
        }

        static string FormatADF(double input)
        {

            string output;
            string inputString = Convert.ToString(input);
            string removeTrail;

                removeTrail = Convert.ToString(input).Substring(0, 5);
            try
            {
                //input.Length
                if (input > 1799.9)
                {
                    output = removeTrail.Substring(0, removeTrail.Length - 1) + Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator + removeTrail.Substring(removeTrail.Length - 1, 1);
                    if (Convert.ToDouble(output) >= 1110)
                    {
                        output = removeTrail.Substring(0, removeTrail.Length - 2) + Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator + removeTrail.Substring(removeTrail.Length - 2, 1);
                    }

                    output = output.Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".");

                    return output;
                }
                else
                {
                    return Convert.ToString(input);
                }
            }
            catch(Exception evilError)
            {
                //MessageBox.Show(evilError.Message + "Input: "+input+", Output:"+output);

                Trace.TraceError(evilError.Message);
                return Convert.ToString(input);

            }

        }

        static string FormatCommStatus(string input)
        {

            string output;

            switch (input)
            {
                case "-1":
                    output = "Invalid";
                    break;
                case "0":
                    output = "OK";
                    break;
                case "1":
                    output = "Does Not Exist";
                    break;
                case "2":
                    output = "No Power";
                    break;
                case "3":
                    output = "Failed";
                    break;
                default:
                    output = "N/A";
                    break;
            }
            return output;
        }


        static double getDistanceFromLatLonInNM(double lat1, double lon1, double lat2,double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            //var d = R * c; // Distance in km
            var d = Math.Round((R * c)*0.539957,2); // Distance in NM
            return d;
        }

        /*  alternative from stackoverflow that apparently runs twice as fast.
         * 
         * function distance(lat1, lon1, lat2, lon2) {
              var p = 0.017453292519943295;    // Math.PI / 180
              var c = Math.cos;
              var a = 0.5 - c((lat2 - lat1) * p)/2 + 
                      c(lat1 * p) * c(lat2 * p) * 
                      (1 - c((lon2 - lon1) * p))/2;

              return 12742 * Math.asin(Math.sqrt(a)); // 2 * R; R = 6371 km
            }
            */

        static double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        static double DegreeBearing(
            double lat1, double lon1,
            double lat2, double lon2)
        {
            var dLon = deg2rad(lon2 - lon1);
            var dPhi = Math.Log(
                Math.Tan(deg2rad(lat2) / 2 + Math.PI / 4) / Math.Tan(deg2rad(lat1) / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
            return Math.Round(ToBearing(Math.Atan2(dLon, dPhi)),2);
        }

        public static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double ToBearing(double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (ToDegrees(radians) + 360) % 360;
        }

        void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            setLabelSimConnect("Connected to SimConnect.");  // MessageBox.Show("Connected to SimConnect");
            setStatusSimConnect(true);
        }

        // The case where the user closes FSX
        void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            setLabelSimConnect("SimConnect / Flight Sim has exited.");  //MessageBox.Show("SimConnect / Flight Sim has exited.");
            setStatusSimConnect(false);
        }

        void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            setLabelSimConnect("Exception received: " + data.dwException);
            MessageBox.Show("Exception received: " + data.dwException);
            setStatusSimConnect(false);
        }

        private void closeConnection()
        {
            if (simconnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close()
                simconnect.Dispose();
                simconnect = null;
                setLabelSimConnect("Connection closed");
                setStatusSimConnect(false);
            }
        }

        private void ButtonCancel(object sender, EventArgs e)
        {
            closeConnection();
            this.Close();
        }

        public void setLabelSimConnect(string message = null)
        {
            if(message != null)
            {
                Global.statusLabelSimConnect = message;
            }
            this.labelSimConnectStatus.Text = Global.statusLabelSimConnect;
        }

        public void setLabelMatric(string message = null)
        {
            if (message != null)
            {
                Global.statusLabelMatric = message;
            }
            this.labelMatricStatus.Text = Global.statusLabelMatric;
        }

        public static void setStatusMatric(Boolean status)
        {
            StatusSimConnect = status;
        }

        public static void setStatusSimConnect(Boolean status)
        {
            StatusSimConnect = status;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage4);
        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void comboDevice1page_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void retryMatricConnection_Click(object sender, EventArgs e)
        {
            
        }
    }
}
