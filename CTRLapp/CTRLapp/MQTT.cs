using CTRLapp.Views.SettingsPages;
using CTRLapp.Views.SettingsPages.GUI;
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
        public static ManagedMqttClient mqttClient;

        public static async Task<bool> ConnectMQTT()
        {
            if (mqttClient != null) await mqttClient.StopAsync();
            Debug.Write("Connecting to MQTT...");
            mqttClient = (ManagedMqttClient)new MqttFactory().CreateManagedMqttClient();
            try
            {
                var options = new MqttClientOptionsBuilder()
                    .WithClientId(Preferences.Get("deviceName", ""))                                                         //client name
                    .WithTcpServer(Preferences.Get("brokerIp", ""))                                                          //mqtt server
                    .WithCredentials(Preferences.Get("brokerUsername", ""), Preferences.Get("brokerPassword", ""))          //username, password
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
                    await App.Current.MainPage.Navigation.PushModalAsync(new SettingsPage());
                return false;
            };
            return true;
        }

        public static async Task<bool> SendMQTT(string topic, string payload)
        {
            if (topic == "") topic = "topic_not_set";
            Debug.WriteLine("sending mqtt message");
            if (mqttClient == null || !mqttClient.IsConnected)
            {
                if (await App.Current.MainPage.DisplayAlert("MQTT connection failed", "do you want to enter settings?", "yes", "no"))
                    await App.Current.MainPage.Navigation.PushModalAsync(new SettingsPage());
                return false;
            }
            try
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithRetainFlag()
                    .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)1)
                    .Build();
                Debug.WriteLine("topic: " + topic + "   payload: " + payload);
                await mqttClient.PublishAsync(message, CancellationToken.None);
                Debug.WriteLine("message sent successfully");
            }
            catch (Exception e)
            {
                Debug.WriteLine("error while sending: " + e.Message);
                if (await App.Current.MainPage.DisplayAlert("MQTT sending failed", "do you want to enter settings?", "yes", "no"))
                    await App.Current.MainPage.Navigation.PushModalAsync(new GuiPage(Views.MainPage.masterMenu, Views.MainPage.bottomMenu));
                return false;
            };
            return true;
        }


        public static async Task<bool> SubscribeMQTT(string topic)
        {
            var topicFilter = new MqttTopicFilterBuilder()
                .WithAtLeastOnceQoS()
                .WithTopic(topic)
                .Build();
            await mqttClient.SubscribeAsync(topicFilter);

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                MqttMessageEventArgs messageReceivedEventArgs = new MqttMessageEventArgs
                {
                    Topic = e.ApplicationMessage.Topic,
                    Message = System.Text.Encoding.Default.GetString(e.ApplicationMessage.Payload),
                };
                MqttMessageReceived?.Invoke(null, messageReceivedEventArgs);
                Debug.WriteLine("Received Message int topic: " + e.ApplicationMessage.Topic + " : " + System.Text.Encoding.Default.GetString(e.ApplicationMessage.Payload));
            });

            return true;
        }

        public static void RemoveMQTTHandelers()
        {
            MqttMessageReceived = null;
        }

        public static event EventHandler<MqttMessageEventArgs> MqttMessageReceived;

    }
    public class MqttMessageEventArgs : EventArgs
    {
        public string Topic;
        public string Message;
    }
}
