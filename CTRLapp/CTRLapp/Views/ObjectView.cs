using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace CTRLapp.Views
{
    public class ObjectView : ContentView
    {
        private static readonly HexToColorConverter colorConverter = new();
        public ObjectView(int masterMenu, int bottomMenu, int objIndex)
        {

            Objects.Object obj = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex];



            switch (obj.Type)
            {

                case "Label":
                    Content = BuildLabel(obj);
                    break;

                case "ValueDisplay":
                    Content = BuildValueDisplay(obj);
                    break;

                case "Button":
                    Content = BuildButton(obj);
                    break;

                case "SwitchButton":
                    Content = BuildSwitchButton(obj);
                    break;

                case "Switch":
                    Content = BuildSwitch(obj);
                    break;

                case "Slider":
                    Content = BuildSlider(obj);
                    break;

                case "Joystick":
                    Content = BuildJoystick(obj);
                    break;

                case "Matrix":
                    Content = BuildMatrix(obj);
                    break;
            }


        }

        private View BuildLabel(Objects.Object obj)
        {
            int.TryParse(obj.Arguments["FontSize"], out int fontsize);
            Label label = new()
            {
                WidthRequest = obj.Width,
                TextColor = (Color)colorConverter.Convert(obj.Arguments["TextColor"]),
                BackgroundColor = (Color)colorConverter.Convert(obj.Arguments["BackgroundColor"]),
                Text = obj.Arguments["Text"],
                FontSize = fontsize,
                IsEnabled = false,
            };
            return label;
        }
        private View BuildValueDisplay(Objects.Object obj)
        {
            int.TryParse(obj.Arguments["FontSize"], out int fontsize);
            Label label = new()
            {
                WidthRequest = obj.Width,
                TextColor = (Color)colorConverter.Convert(obj.Arguments["TextColor"]),
                BackgroundColor = (Color)colorConverter.Convert(obj.Arguments["BackgroundColor"]),
                FontSize = fontsize,
                Text = obj.Arguments["BeforeText"] + obj.Arguments["AfterText"],
                IsEnabled = false,
            };

            //mqtt syncing function
            //MQTT.MqttMessageReceived += (_, e) =>
            //{
            //    if (e.Topic != obj.Arguments[3]) return;
            //    label.Text = obj.Arguments[2] + e.Message + obj.Arguments[4];
            //};
            //if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[3]);
            return label;
        } //buggy
        private View BuildButton(Objects.Object obj)
        {
            var button = new Button
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                BackgroundColor = (Color)colorConverter.Convert(obj.Arguments["BackgroundColor"]),
                TextColor = (Color)colorConverter.Convert(obj.Arguments["TextColor"]),
                Text = obj.Arguments["Text"],
                TextTransform = TextTransform.None,
            };

            button.Clicked += async (_, e) =>
            {
                await MQTT.SendMQTT(obj.Arguments["Topic"], obj.Arguments["Message"]);
            };
            return button;
        }
        private View BuildSwitchButton(Objects.Object obj)
        {
            bool toggled = false;
            var switchButton = new Button
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                BackgroundColor = (Color)colorConverter.Convert(obj.Arguments["BackgroundColor"]),
                TextColor = (Color)colorConverter.Convert(obj.Arguments["TextColor"]),
                Text = obj.Arguments["Text"],
                TextTransform = TextTransform.None,
            };

            switchButton.Clicked += async (_, e) =>
            {
                toggled = !toggled;
                if (toggled)
                {
                    switchButton.BackgroundColor = (Color)colorConverter.Convert(obj.Arguments["OnColor"]);
                    await MQTT.SendMQTT(obj.Arguments["Topic"], obj.Arguments["HighMessage"]);
                }
                else
                {
                    switchButton.BackgroundColor = (Color)colorConverter.Convert(obj.Arguments["BackgroundColor"]);
                    await MQTT.SendMQTT(obj.Arguments["Topic"], obj.Arguments["LowMessage"]);
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
        private View BuildSwitch(Objects.Object obj)
        {
            bool block = false; //avoid loopback from mqtt delay
            var temp = new Xamarin.Forms.Switch
            {
                ThumbColor = (Color)colorConverter.Convert(obj.Arguments["ThumbColor"]),
                OnColor = (Color)colorConverter.Convert(obj.Arguments["OnColor"]),
            };
            temp.Toggled += async (_, e) =>
            {
                if (!block)
                {
                    string message = obj.Arguments["LowMessage"];
                    if (e.Value) message = obj.Arguments["HighMessage"];
                    await MQTT.SendMQTT(obj.Arguments["Topic"], message);
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
        private View BuildSlider(Objects.Object obj)
        {
            bool block = false; //avoid loopback from mqtt delay
            if (int.Parse(obj.Arguments["Minimum"]) >= int.Parse(obj.Arguments["Maximum"]))
                obj.Arguments["Minimum"] = (int.Parse(obj.Arguments["Maximum"]) - 1).ToString();
            var slider = new Slider
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                ThumbColor = (Color)colorConverter.Convert(obj.Arguments["ThumbColor"]),
                MinimumTrackColor = (Color)colorConverter.Convert(obj.Arguments["MinimumTrackColor"]),
                MaximumTrackColor = (Color)colorConverter.Convert(obj.Arguments["MaximumTrackColor"]),
                Maximum = int.Parse(obj.Arguments["Maximum"]),
                Minimum = int.Parse(obj.Arguments["Minimum"]),
            };
            slider.ValueChanged += async (_, e) =>
            {
                if (!block)
                    await MQTT.SendMQTT(obj.Arguments["Topic"], ((int)e.NewValue).ToString());
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
        private View BuildJoystick(Objects.Object obj)
        {
            var touchEffect = new TouchTracking.Forms.TouchEffect() { Capture = true };
            SKCanvasView canvas = new()
            {
                HeightRequest = obj.Width,
                WidthRequest = obj.Width,
                EnableTouchEvents = false,
                BackgroundColor = (Color)colorConverter.Convert(obj.Arguments["BackgroundColor"]),
            };
            SKPoint touch = new();
            Timer timer = new()
            {
                AutoReset = true,
                Interval = 100,
            };
            bool pressed = false;
            Point coordinates = new();
            SKSize canvassize = new();

            canvas.PaintSurface += (_, e) =>
            {
                SKCanvas surface = e.Surface.Canvas;
                surface.Clear();

                float radius = canvassize.Width / 2;

                SKPaint thumbPaint = new()
                {
                    Style = SKPaintStyle.Fill,
                    Color = ((Color)colorConverter.Convert(obj.Arguments["ThumbColor"])).ToSKColor(),
                    IsAntialias = true,
                };
                surface.DrawCircle((float)touch.X, (float)touch.Y, radius / 3, thumbPaint);
            };
            touchEffect.TouchAction += (_, e) =>
            {
                switch (e.Type)
                {
                    case TouchTracking.TouchActionType.Pressed:
                        canvassize = canvas.CanvasSize;
                        pressed = true;
                        touch = new SKPoint((float)(canvas.CanvasSize.Width * e.Location.X / canvas.Width),
                                            (float)(canvas.CanvasSize.Height * e.Location.Y / canvas.Height));
                        canvas.InvalidateSurface();
                        timer.Start();
                        break;
                    case TouchTracking.TouchActionType.Moved:
                        if (!pressed) break;
                        touch = new SKPoint((float)(canvas.CanvasSize.Width * e.Location.X / canvas.Width),
                                            (float)(canvas.CanvasSize.Height * e.Location.Y / canvas.Height));
                        if (touch.X < canvas.CanvasSize.Width / 6) touch.X = canvas.CanvasSize.Width / 6;   // so that the circle doesnt go further than the edge of the canvas
                        if (touch.X > canvas.CanvasSize.Width / 6 * 5) touch.X = canvas.CanvasSize.Width / 6 * 5;
                        if (touch.Y < canvas.CanvasSize.Width / 6) touch.Y = canvas.CanvasSize.Width / 6;
                        if (touch.Y > canvas.CanvasSize.Width / 6 * 5) touch.Y = canvas.CanvasSize.Width / 6 * 5;

                        canvas.InvalidateSurface();
                        break;
                    case TouchTracking.TouchActionType.Released:
                        touch = new SKPoint((float)(canvas.CanvasSize.Width / 2),
                                            (float)(canvas.CanvasSize.Height / 2));
                        canvas.InvalidateSurface();
                        pressed = false;
                        timer.Stop();
                        break;
                }
            };
            timer.Elapsed += async (_, e) =>
            {
                coordinates.X += (touch.X - (canvassize.Width / 2)) * float.Parse(obj.Arguments["SensitivityX"]) * 0.01;
                coordinates.Y += (touch.Y - (canvassize.Height / 2)) * float.Parse(obj.Arguments["SensitivityY"]) * 0.01;

                int minimumx = int.Parse(obj.Arguments["MinimumX"]),
                    maximumx = int.Parse(obj.Arguments["MaximumX"]),
                    minimumy = int.Parse(obj.Arguments["MinimumY"]),
                    maximumy = int.Parse(obj.Arguments["MaximumY"]);
                if (coordinates.X < minimumx) coordinates.X = minimumx;
                if (coordinates.X > maximumx) coordinates.X = maximumx;
                if (coordinates.Y < minimumy) coordinates.Y = minimumy;
                if (coordinates.Y > maximumy) coordinates.Y = maximumy;

                await MQTT.SendMQTT(obj.Arguments["TopicX"], ((int)coordinates.X).ToString());
                await MQTT.SendMQTT(obj.Arguments["TopicY"], ((int)coordinates.Y).ToString());
            };
            canvas.Effects.Add(touchEffect);

            //syncing function
            //MQTT.MqttMessageReceived += (_, e) =>
            //{
            //    if (timer.Enabled) return;
            //    if (e.Topic == obj.Arguments[2])
            //    {
            //        float.TryParse(e.Message, out float messageFloat);
            //        coordinates.X = messageFloat;
            //    }
            //    else if (e.Topic == obj.Arguments[3])
            //    {
            //        float.TryParse(e.Message, out float messageFloat);
            //        coordinates.Y = messageFloat;
            //    }
            //};
            //if (MainPage.topicList.IndexOf(obj.Arguments[2]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[2]);
            //if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[3]);



            return new Frame() { Content = canvas, CornerRadius = (float)obj.Width / 2, BackgroundColor = Color.Transparent };
        }
        private View BuildMatrix(Objects.Object obj)
        {
            float thumbRadius = 50;
            var touchEffect = new TouchTracking.Forms.TouchEffect() { Capture = true };
            SKCanvasView canvas = new()
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                EnableTouchEvents = false,
                BackgroundColor = (Color)colorConverter.Convert(obj.Arguments["BackgroundColor"]),
            };
            SKPoint touch = new();
            canvas.PaintSurface += (_, e) =>
            {
                var surface = e.Surface.Canvas;
                surface.Clear();

                SKPaint thumbPaint = new()
                {
                    Style = SKPaintStyle.Fill,
                    Color = ((Color)colorConverter.Convert(obj.Arguments["ThumbColor"])).ToSKColor(),
                    IsAntialias = true,
                };
                surface.DrawCircle((float)touch.X, (float)touch.Y, thumbRadius / 3, thumbPaint);
            };
            bool isTouchDown = false;
            touchEffect.TouchAction += async (_, e) =>
            {
                switch (e.Type)
                {
                    case TouchTracking.TouchActionType.Pressed:
                        isTouchDown = true;
                        goto case TouchTracking.TouchActionType.Moved;
                    case TouchTracking.TouchActionType.Released:
                        isTouchDown = false;
                        break;
                    case TouchTracking.TouchActionType.Moved:
                        if (!isTouchDown) break;
                        //clamped touch point
                        Point clamped = new(
                            (e.Location.X / canvas.Width).Clamp(0, 1),
                            (e.Location.Y / canvas.Height).Clamp(0, 1));

                        touch = new SKPoint((float)(clamped.X * canvas.CanvasSize.Width),
                                            (float)(clamped.Y * canvas.CanvasSize.Height));

                        //map touch point to defined min and max
                        Point coordinates = new()
                        {
                            X = clamped.X * (float.Parse(obj.Arguments["MaximumX"]) - float.Parse(obj.Arguments["MinimumX"]))
                                + float.Parse(obj.Arguments["MinimumX"]),
                            Y = clamped.Y * (float.Parse(obj.Arguments["MaximumY"]) - float.Parse(obj.Arguments["MinimumY"]))
                                + float.Parse(obj.Arguments["MinimumY"]),
                        };

                        await MQTT.SendMQTT(obj.Arguments["TopicX"], ((int)coordinates.X).ToString());
                        await MQTT.SendMQTT(obj.Arguments["TopicY"], ((int)coordinates.Y).ToString());
                        canvas.InvalidateSurface();

                        break;
                }
            };

            canvas.Effects.Add(touchEffect);

            //syncing function
            //MQTT.MqttMessageReceived += (_, e) =>
            //{
            //    if (e.Topic == obj.Arguments[2])
            //    {
            //        float.TryParse(e.Message, out float messageFloat);
            //        touch.X = (messageFloat - float.Parse(obj.Arguments[4])) / (float.Parse(obj.Arguments[5]) - float.Parse(obj.Arguments[4])) * canvas.CanvasSize.Width;
            //        canvas.InvalidateSurface();
            //    }
            //    else if (e.Topic == obj.Arguments[3])
            //    {
            //        float.TryParse(e.Message, out float messageFloat);
            //        touch.Y = (messageFloat - float.Parse(obj.Arguments[6])) / (float.Parse(obj.Arguments[7]) - float.Parse(obj.Arguments[6])) * canvas.CanvasSize.Width;
            //        canvas.InvalidateSurface();
            //    }
            //};
            //if (MainPage.topicList.IndexOf(obj.Arguments[2]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[2]);
            //if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
            //    MainPage.topicList.Add(obj.Arguments[3]);


            return canvas;
        }


    }
}