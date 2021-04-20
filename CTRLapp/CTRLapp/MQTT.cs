using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CTRLapp
{
    class MQTT
    {
        private static MqttClient mqttClient;

        public static void DisconnectMQTT()
        {
            if (mqttClient != null) mqttClient.DisconnectAsync();
            mqttClient = null;
        }
        public static async Task<int> ConnectMQTT()
        {
            if (Variables.Variables.MQTT_connecting) return 1;
            else Variables.Variables.MQTT_connecting = true;
            Debug.WriteLine("connecting to MQTT Broker...");
            var factory = new MqttFactory();
            mqttClient = (MqttClient)factory.CreateMqttClient();
            try
            {
                var options = new MqttClientOptionsBuilder()
                    .WithClientId(Preferences.Get("device_name", ""))                                                         //client name
                    .WithTcpServer(Preferences.Get("broker_ip", ""))                                                          //mqtt server
                    .WithCredentials(Preferences.Get("broker_username", ""), Preferences.Get("broker_password", ""))          //username, password
                    .Build();
                await mqttClient.ConnectAsync(options, CancellationToken.None);
                // Debug.WriteLine("connected");
            }
            catch (Exception)
            {
                mqttClient = null;
                return 2;
            };
            return 0;
        }

        public static async Task<string> SendMQTT(string topic, string payload)
        {
            topic = "test"; //just for testing


            if (mqttClient == null || !mqttClient.IsConnected)
            {
                Debug.WriteLine("connecting");
                int result = await ConnectMQTT();
                if (result == 2) return "connection_failed";
                else if (result == 1) return "already_connecting";
                Debug.WriteLine("result = " + result);
            }
            try
            {
                var message = new MqttApplicationMessageBuilder()
                    //.WithTopic(topic)
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithRetainFlag()
                    .Build();
                Debug.WriteLine("topic: " + topic + "   payload: " + payload);
                await mqttClient.PublishAsync(message, CancellationToken.None);
                Debug.WriteLine("message sent successfully");
            }
            catch (Exception)
            {
                return "message_sending_failed";
            };
            return "successful";
        }
    }
}
