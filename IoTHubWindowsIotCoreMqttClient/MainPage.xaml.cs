using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string hubAddress = "MakerDen.azure-devices.net";
        const string hubName = "MqttDevice";
        const string hubPass = "SharedAccessSignature sr=MakerDen.azure-devices.net%2fdevices%2fMqttDevice&sig=sh8ZVK3L4u9XYuTeI%2f3l%2bx7X6D3BRJADz1rppuK3hvw%3d&se=1477701103";

        string hubUser = $"{hubAddress}/{hubName}";
        string hubTopicPublish = $"devices/{hubName}/messages/events/";
        string hubTopicSubscribe = $"devices/{hubName}/messages/devicebound/#";

        private MqttClient client;

        public MainPage() {
            this.InitializeComponent();

            // https://m2mqtt.wordpress.com/m2mqtt_doc/
            this.client = new MqttClient(hubAddress, 8883, true, MqttSslProtocols.TLSv1_2);
            this.client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            this.client.Subscribe(new string[] { hubTopicSubscribe }, new byte[] { 0 });


            this.client.Connect(hubName, hubUser, hubPass);

            Task.Run(async() =>
            {


                while (true) {
                    float temperature = 25;
                    string json = "{ temp : " + temperature + " }";

                    this.client.Publish(hubTopicPublish, Encoding.UTF8.GetBytes(json));

                    await Task.Delay(120000);
                }
            });
        }

        private void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e) {
            Debug.WriteLine("message");
            string result = System.Text.Encoding.UTF8.GetString(e.Message);
        }
    }
}
