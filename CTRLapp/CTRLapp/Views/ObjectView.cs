using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace CTRLapp.Views
{
    public class ObjectView : ContentView
    {
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
            int.TryParse(obj.Arguments[3], out int fontsize);
            Label label = new Label()
            {
                WidthRequest = obj.Width,
                TextColor = Color.FromHex(obj.Arguments[0]),
                BackgroundColor = Color.FromHex(obj.Arguments[1]),
                Text = obj.Arguments[2],
                FontSize = fontsize,
                IsEnabled = false,
            };
            return label;
        }
        private View BuildValueDisplay(Objects.Object obj)
        {
            int.TryParse(obj.Arguments[5], out int fontsize);
            Label label = new Label()
            {
                WidthRequest = obj.Width,
                TextColor = Color.FromHex(obj.Arguments[0]),
                BackgroundColor = Color.FromHex(obj.Arguments[1]),
                FontSize = fontsize,
                Text = obj.Arguments[2] + obj.Arguments[4],
                IsEnabled = false,
            };

            //mqtt syncing function
            MQTT.MqttMessageReceived += (_, e) =>
            {
                if (e.Topic != obj.Arguments[3]) return;
                label.Text = obj.Arguments[2] + e.Message + obj.Arguments[4];
            };
            if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
                MainPage.topicList.Add(obj.Arguments[3]);
            return label;
        } //buggy
        private View BuildButton(Objects.Object obj)
        {
            var button = new Button
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                BackgroundColor = Color.FromHex(obj.Arguments[1]),
                TextColor = Color.FromHex(obj.Arguments[0]),
                Text = obj.Arguments[2],
                TextTransform = TextTransform.None,
            };

            button.Clicked += async (_, e) =>
            {
                await MQTT.SendMQTT(obj.Arguments[3], obj.Arguments[4]);
            };
            return button;
        }
        private View BuildTwoWayButton(Objects.Object obj)
        {
            var button = new Button
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                BackgroundColor = Color.FromHex(obj.Arguments[1]),
                TextColor = Color.FromHex(obj.Arguments[0]),
                Text = obj.Arguments[2],
                TextTransform = TextTransform.None,
            };

            button.Clicked += async (_, e) =>
            {
                await MQTT.SendMQTT(obj.Arguments[3], obj.Arguments[4]);
            };
            return button;
        }
        private View BuildSwitch(Objects.Object obj)
        {
            bool block = false; //avoid loopback from mqtt delay
            var temp = new Xamarin.Forms.Switch
            {
                ThumbColor = Color.FromHex(obj.Arguments[0]),
                OnColor = Color.FromHex(obj.Arguments[1]),
            };
            temp.Toggled += async (_, e) =>
            {
                if (!block)
                {
                    string message = obj.Arguments[3];
                    if (e.Value) message = obj.Arguments[4];
                    await MQTT.SendMQTT(obj.Arguments[2], message);
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
            var slider = new Slider
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                ThumbColor = Color.FromHex(obj.Arguments[0]),
                MinimumTrackColor = Color.FromHex(obj.Arguments[1]),
                MaximumTrackColor = Color.FromHex(obj.Arguments[2]),
                Minimum = int.Parse(obj.Arguments[4]),
                Maximum = int.Parse(obj.Arguments[5]),
            };
            slider.ValueChanged += async (_, e) =>
            {
                if (!block)
                    await MQTT.SendMQTT(obj.Arguments[3], e.NewValue.ToString());
            };

            //syncing function
            MQTT.MqttMessageReceived += (_, e) =>
            {
                if (e.Topic != obj.Arguments[3]) return;
                float.TryParse(e.Message, out float messageFloat);
                block = true;
                slider.Value = messageFloat;
                block = false;
            };
            if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
                MainPage.topicList.Add(obj.Arguments[3]);
            return slider;
        }
        private View BuildJoystick(Objects.Object obj)
        {
            var touchEffect = new TouchTracking.Forms.TouchEffect() { Capture = true };
            SKCanvasView canvas = new SKCanvasView
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                EnableTouchEvents = false,
                BackgroundColor = Color.FromHex(obj.Arguments[1]),
            };
            SKPoint touch = new SKPoint();
            Timer timer = new Timer
            {
                AutoReset = true,
                Interval = 100,
            };
            bool pressed = false;
            Point coordinates = new Point();
            SKSize canvassize = new SKSize();

            canvas.PaintSurface += (_, e) =>
            {
                var surface = e.Surface.Canvas;
                surface.Clear();

                SKPaint backgroundPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.FromHex(obj.Arguments[1]).ToSKColor(),
                };
                float radius = canvassize.Width / 2;
                if (canvassize.Width < canvassize.Height) radius = canvassize.Height / 2;
                surface.DrawCircle(canvassize.Width / 2, canvassize.Height / 2, radius, backgroundPaint);

                SKPaint thumbPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.FromHex(obj.Arguments[0]).ToSKColor(),
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
                coordinates.X += (touch.X - (canvassize.Width / 2)) * float.Parse(obj.Arguments[6]) * 0.1;
                coordinates.Y += (touch.Y - (canvassize.Height / 2)) * float.Parse(obj.Arguments[9]) * 0.1;

                int minimumx = int.Parse(obj.Arguments[4]),
                    maximumx = int.Parse(obj.Arguments[5]),
                    minimumy = int.Parse(obj.Arguments[7]),
                    maximumy = int.Parse(obj.Arguments[8]);
                if (coordinates.X < minimumx) coordinates.X = minimumx;
                if (coordinates.X > maximumx) coordinates.X = maximumx;
                if (coordinates.Y < minimumy) coordinates.Y = minimumy;
                if (coordinates.Y > maximumy) coordinates.Y = maximumy;

                await MQTT.SendMQTT(obj.Arguments[2], coordinates.X.ToString());
                await MQTT.SendMQTT(obj.Arguments[3], coordinates.Y.ToString());
            };
            canvas.Effects.Add(touchEffect);

            //syncing function
            MQTT.MqttMessageReceived += (_, e) =>
            {
                if (timer.Enabled) return;
                if (e.Topic == obj.Arguments[2])
                {
                    float.TryParse(e.Message, out float messageFloat);
                    coordinates.X = messageFloat;
                }
                else if (e.Topic == obj.Arguments[3])
                {
                    float.TryParse(e.Message, out float messageFloat);
                    coordinates.Y = messageFloat;
                }
            };
            if (MainPage.topicList.IndexOf(obj.Arguments[2]) == -1)
                MainPage.topicList.Add(obj.Arguments[2]);
            if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
                MainPage.topicList.Add(obj.Arguments[3]);

            return canvas;
        }
        private View BuildMatrix(Objects.Object obj)
        {
            var touchEffect = new TouchTracking.Forms.TouchEffect() { Capture = true };
            SKCanvasView canvas = new SKCanvasView
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                EnableTouchEvents = false,
                BackgroundColor = Color.FromHex(obj.Arguments[1]),
            };
            SKPoint touch = new SKPoint();
            canvas.PaintSurface += (_, e) =>
            {
                var surface = e.Surface.Canvas;
                surface.Clear();

                float radius = 50;
                SKPaint thumbPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.FromHex(obj.Arguments[0]).ToSKColor(),
                };
                surface.DrawCircle((float)touch.X, (float)touch.Y, radius / 3, thumbPaint);
            };
            touchEffect.TouchAction += async (_, e) =>
            {
                switch (e.Type)
                {
                    case TouchTracking.TouchActionType.Pressed:
                    case TouchTracking.TouchActionType.Moved:
                    case TouchTracking.TouchActionType.Released:
                        //map touch point to Canvas because different size
                        touch = new SKPoint((float)(canvas.CanvasSize.Width * e.Location.X / canvas.Width),
                                            (float)(canvas.CanvasSize.Height * e.Location.Y / canvas.Height));

                        if (touch.X < 0) touch.X = 0;   // so that the circle doesnt go further than the edge of the canvas
                        if (touch.X > canvas.CanvasSize.Width) touch.X = canvas.CanvasSize.Width;
                        if (touch.Y < 0) touch.Y = 0;
                        if (touch.Y > canvas.CanvasSize.Width) touch.Y = canvas.CanvasSize.Width;

                        //map touch point to defined min and max
                        Point coordinates = new Point()
                        {
                            X = (e.Location.X / canvas.Width) * (float.Parse(obj.Arguments[5]) - float.Parse(obj.Arguments[4])) + float.Parse(obj.Arguments[4]),
                            Y = (e.Location.Y / canvas.Width) * (float.Parse(obj.Arguments[7]) - float.Parse(obj.Arguments[6])) + float.Parse(obj.Arguments[6]),
                        };

                        await MQTT.SendMQTT(obj.Arguments[2], coordinates.X.ToString());
                        await MQTT.SendMQTT(obj.Arguments[3], coordinates.Y.ToString());
                        canvas.InvalidateSurface();

                        break;
                }
            };

            canvas.Effects.Add(touchEffect);

            //syncing function
            MQTT.MqttMessageReceived += (_, e) =>
            {
                if (e.Topic == obj.Arguments[2])
                {
                    float.TryParse(e.Message, out float messageFloat);
                    touch.X = (messageFloat - float.Parse(obj.Arguments[4])) / (float.Parse(obj.Arguments[5]) - float.Parse(obj.Arguments[4])) * canvas.CanvasSize.Width;
                    canvas.InvalidateSurface();
                }
                else if (e.Topic == obj.Arguments[3])
                {
                    float.TryParse(e.Message, out float messageFloat);
                    touch.Y = (messageFloat - float.Parse(obj.Arguments[6])) / (float.Parse(obj.Arguments[7]) - float.Parse(obj.Arguments[6])) * canvas.CanvasSize.Width;
                    canvas.InvalidateSurface();
                }
            };
            if (MainPage.topicList.IndexOf(obj.Arguments[2]) == -1)
                MainPage.topicList.Add(obj.Arguments[2]);
            if (MainPage.topicList.IndexOf(obj.Arguments[3]) == -1)
                MainPage.topicList.Add(obj.Arguments[3]);

            return canvas;
        }


    }
}