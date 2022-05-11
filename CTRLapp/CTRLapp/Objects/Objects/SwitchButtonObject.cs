using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CTRLapp.Objects.Objects
{
    public class SwitchButtonObject : BaseObject
    {
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Color OnColor { get; set; }
        public string Text { get; set; }
        public string Topic { get; set; }
        public string HighMessage { get; set; }
        public string LowMessage { get; set; }


        public static View BuildSwitchButton(Objects.SwitchButtonObject obj)
        {
            bool toggled = false;
            var switchButton = new Button
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                BackgroundColor = obj.BackgroundColor,
                TextColor = obj.TextColor,
                Text = obj.Text,
                TextTransform = TextTransform.None,
            };

            switchButton.Clicked += async (_, e) =>
            {
                toggled = !toggled;
                if (toggled)
                {
                    switchButton.BackgroundColor = obj.OnColor;
                    await MQTT.SendMQTT(obj.Topic, obj.HighMessage);
                }
                else
                {
                    switchButton.BackgroundColor = obj.BackgroundColor;
                    await MQTT.SendMQTT(obj.Topic, obj.LowMessage);
                }
            };

            //syncing function
            //MQTT.MqttMessageReceived += (_, e) =>
            //{
            //    if (e.Topic != obj.Arguments[4]) return;
            //    if (e.Message == obj.Arguments[6]) //set background color according to received message (high if message == high value, low if else)
            //        switchButton.BackgroundColor = (Color)colorConverter.Convert(obj.Arguments[2]);
            //    else
            //        switchButton.BackgroundColor = (Color)colorConverter.Convert(obj.Arguments[1]);
            //};
            //if (MainPage.topicList.IndexOf(obj.Arguments[4]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[4]);
            return switchButton;
        }

    }
}
