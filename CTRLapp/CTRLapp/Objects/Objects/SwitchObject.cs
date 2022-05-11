using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CTRLapp.Objects.Objects
{
    public class SwitchObject : BaseObject
    {
        public Color ThumbColor { get; set; }
        public Color OnColor { get; set; }
        public string Text { get; set; }
        public string Topic { get; set; }
        public string HighMessage { get; set; }
        public string LowMessage { get; set; }


        public static View BuildSwitch(SwitchObject obj)
        {
            bool block = false; //avoid loopback from mqtt delay
            var temp = new Xamarin.Forms.Switch
            {
                ThumbColor = obj.ThumbColor,
                OnColor = obj.OnColor,
            };
            temp.Toggled += async (_, e) =>
                {
                    if (!block)
                    {
                        string message = obj.LowMessage;
                        if (e.Value) message = obj.HighMessage;
                        await MQTT.SendMQTT(obj.Topic, message);
                    }
                };

            //syncing function   -- buggy for switch
            //MQTT.MqttMessageReceived += (_, e) =>
            //{
            //    if (e.Topic != obj.Arguments[2]) return;
            //    bool receivedValue = false;
            //    if (e.Message == obj.Arguments[4]) receivedValue = true;
            //    block = true;
            //    temp2.IsToggled = receivedValue;
            //    block = false;
            //};
            //if (MainPage.topicList.IndexOf(obj.Arguments[2]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[2]);
            return temp;
        }

    }
}
