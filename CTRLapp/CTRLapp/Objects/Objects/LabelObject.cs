using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CTRLapp.Objects.Objects
{
    public class LabelObject : BaseObject
    {
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public string Text { get; set; }
        public int FontSize { get; set; }


        public static View BuildLabel(Objects.LabelObject obj)
        {
            Label label = new()
            {
                WidthRequest = obj.Width,
                TextColor = obj.TextColor,
                BackgroundColor = obj.BackgroundColor,
                Text = obj.Text,
                FontSize = obj.FontSize,
                IsEnabled = false,
            };
            return label;
        }
    }

}
