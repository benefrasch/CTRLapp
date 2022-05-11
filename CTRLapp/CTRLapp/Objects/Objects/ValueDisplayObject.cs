using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CTRLapp.Objects.Objects
{
    public class ValueDisplayObject : BaseObject
    {
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public string BeforeText { get; set; }
        public string AfterText { get; set; }
        public string Topic { get; set; }
        public int FontSize { get; set; }


        public static View BuildValueDisplay(ValueDisplayObject obj)
        {
            Label label = new()
            {
                WidthRequest = obj.Width,
                TextColor = obj.TextColor,
                BackgroundColor = obj.BackgroundColor,
                FontSize = obj.FontSize,
                Text = obj.BeforeText + obj.AfterText,
                IsEnabled = false,
            };

            //mqtt syncing function
            //MQTT.MqttMessageReceived += (_, e) =>
            //{
            //    if (e.Topic != obj.Arguments[3]) return;
            //    label.Text = obj.Arguments[2] + e.Message + obj.Arguments[4];
            //};
            //if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[3])
            return label;
        }
    }

}
