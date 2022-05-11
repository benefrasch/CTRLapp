using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CTRLapp.Objects.Objects
{
    public class SliderObject : BaseObject
    {
        public Color MinimumTrackColor { get; set; }
        public Color MaximumTrackColor { get; set; }
        public Color ThumbColor { get; set; }
        public string Topic { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }


        public static View BuildSlider(SliderObject obj)
        {
            bool block = false; //avoid loopback from mqtt delay
            var slider = new Slider
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                ThumbColor = obj.ThumbColor,
                MinimumTrackColor = obj.MinimumTrackColor,
                MaximumTrackColor = obj.MaximumTrackColor,
                Maximum = obj.Maximum,
                Minimum = obj.Minimum,
            };
            slider.ValueChanged += async (_, e) =>
            {
                if (!block)
                    await MQTT.SendMQTT(obj.Topic, ((int)e.NewValue).ToString());
            };

            //syncing function
            //MQTT.MqttMessageReceived += (_, e) =>
            //{
            //    if (e.Topic != obj.Arguments[3]) return;
            //    float.TryParse(e.Message, out float messageFloat);
            //    block = true;
            //    slider.Value = messageFloat;
            //    block = false;
            //};
            //if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[3]);
            return slider;
        }

    }
}
