using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace windowsIoTtextcode
{

    public sealed partial class MainPage : Page
    {
        //int counter = 0; // dummy temp counter value;
        UsbSerial usb;
        RemoteDevice arduino;
        private DispatcherTimer loopTimer;
        ConnectTheDotsHelper ctdHelper1;

        ushort Temperature = 0;

        public MainPage()
        {
            this.InitializeComponent();
            usb = new UsbSerial("VID_2341", "PID_8036");
            List<ConnectTheDotsSensor> sensors_one = new List<ConnectTheDotsSensor> {
                new ConnectTheDotsSensor("2198a348-e2f9-4438-ab23-82a3930662ac", "Temperature", "F"),
            };

            arduino = new RemoteDevice(usb);
            arduino.DeviceReady += onDeviceReady;
            usb.begin(57600, SerialConfig.SERIAL_8N1);

            ctdHelper1 = new ConnectTheDotsHelper(serviceBusNamespace: "exampleiot-ns",//please change this service bus name for you used
                eventHubName: "ehdevices",
                keyName: "D1",//change the date channel
                key: "××××××××××××××××××××××××××××",//please input your key number!!
                displayName: "Temperature",
                organization: "DFRobot",
                location: "Shanghai",
                sensorList: sensors_one);

            Button_Click(null, null);
        }

        private void onDeviceReady()
        {
            Debug.WriteLine("Device Ready");

            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                setup();
            }));
        }

        private void setup()
        {
            Debug.WriteLine("Setup");
            arduino.pinMode("A1",PinMode.ANALOG);
            loopTimer = new DispatcherTimer();
            loopTimer.Interval = TimeSpan.FromMilliseconds(500);
            loopTimer.Tick += textReadTem;
            loopTimer.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { }

        private void textReadTem(object sender, object e)
        {
            Temperature = arduino.analogRead("A1");

            ConnectTheDotsSensor sensor1 = ctdHelper1.sensors.Find(item => item.guid == "2198a348-e2f9-4438-ab23-82a3930662ac");
            sensor1.value = arduino.analogRead("A1") * (5 / 10.24);
            ctdHelper1.SendSensorData(sensor1);

        }
    }
}

