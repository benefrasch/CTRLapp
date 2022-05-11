using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CTRLapp.Objects.Objects
{
    public class ButtonObject : BaseObject
    {
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public string Text { get; set; }
        public string Topic { get; set; }
        public string Message { get; set; }


        public static View BuildButton(ButtonObject obj)
        {
            var button = new Button
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                BackgroundColor = obj.BackgroundColor,
                TextColor = obj.TextColor,
                Text = obj.Text,
                TextTransform = TextTransform.None,
            };

            button.Clicked += async (_, e) =>
            {
                await MQTT.SendMQTT(obj.Topic, obj.Message);
            };
            return button;
        }
    }

}
