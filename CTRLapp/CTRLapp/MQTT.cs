using CTRLapp.Views.Settings_pages;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CTRLapp
{
    class MQTT
    {
        private static ManagedMqttClient mqttClient;

        public static void DisconnectMQTT()
        {
            Debug.WriteLine("Disconnecting MQTT");
            if (mqttClient != null) mqttClient.StopAsync();
        }
        public static async Task<bool> ConnectMQTT()
        {
            DisconnectMQTT();
            Debug.Write("Connecting to MQTT...");
            var factory = new MqttFactory();
            mqttClient = (ManagedMqttClient)new MqttFactory().CreateManagedMqttClient();
            try
            {
                var options = new MqttClientOptionsBuilder()
                    .WithClientId(Preferences.Get("device_name", ""))                                                         //client name
                    .WithTcpServer(Preferences.Get("broker_ip", ""))                                                          //mqtt server
                    .WithCredentials(Preferences.Get("broker_username", ""), Preferences.Get("broker_password", ""))          //username, password
                    .Build();
                var managedOptions = new ManagedMqttClientOptionsBuilder()
                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                    .WithClientOptions(options)
                    .Build();
                await mqttClient.StartAsync(managedOptions);
                Debug.WriteLine("connected");
            }
            catch (Exception e)
            {
                Debug.WriteLine("error while connecting: " + e.Message);
                if (await App.Current.MainPage.DisplayAlert("MQTT connection failed", "do you want to enter settings?", "yes", "no"))
                    await App.Current.MainPage.Navigation.PushModalAsync(new Settings_page());
                return false;
            };
            return true;
        }

        public static async Task<bool> SendMQTT(string topic, string payload)
        {
            if (mqttClient == null || !mqttClient.IsConnected)
            {
                if (await App.Current.MainPage.DisplayAlert("MQTT connection failed", "do you want to enter settings?", "yes", "no"))
                    await App.Current.MainPage.Navigation.PushModalAsync(new Settings_page());
                return false;
            }
            try
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithRetainFlag()
                    .Build();
                Debug.WriteLine("topic: " + topic + "   payload: " + payload);
                await mqttClient.PublishAsync(message, CancellationToken.None);
                Debug.WriteLine("message sent successfully");
            }
            catch (Exception e)
            {
                Debug.WriteLine("error while sending: " + e.Message);
                if (await App.Current.MainPage.DisplayAlert("MQTT sending failed", "do you want to enter settings?", "yes", "no"))
                    await App.Current.MainPage.Navigation.PushModalAsync(new Gui_page(Views.MainPage.master_menu, Views.MainPage.bottom_menu));
                return false;
            };
            return true;


        }

        

    }
}
