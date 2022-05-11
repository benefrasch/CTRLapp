using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace CTRLapp.Objects.Objects
{
    public class JoystickObject : BaseObject
    {
        public Color BackgroundColor { get; set; }
        public Color ThumbColor { get; set; }
        public string TopicX { get; set; }
        public int MinimumX { get; set; }
        public int MaximumX { get; set; }
        public float SensitivityX { get; set; }   
        public string TopicY { get; set; }
        public int MinimumY { get; set; }
        public int MaximumY { get; set; }
        public float SensitivityY { get; set; }


        public static View BuildJoystick(JoystickObject obj)
        {
            var touchEffect = new TouchTracking.Forms.TouchEffect() { Capture = true };
            SKCanvasView canvas = new()
            {
                HeightRequest = obj.Width,
                WidthRequest = obj.Width,
                EnableTouchEvents = false,
                BackgroundColor = obj.BackgroundColor,
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
                    Color = obj.ThumbColor.ToSKColor(),
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
                        float width6 = canvas.CanvasSize.Width / 6;
                        touch = new SKPoint((float)(canvas.CanvasSize.Width * e.Location.X / canvas.Width).Clamp(width6,width6* 5 ),
                                            (float)(canvas.CanvasSize.Height * e.Location.Y / canvas.Height).Clamp(width6, width6 *5));

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
                coordinates.X += ((touch.X - (canvassize.Width / 2)) * obj.SensitivityX * 0.01).Clamp(obj.MinimumX, obj.MaximumX);
                coordinates.Y += ((touch.Y - (canvassize.Height / 2)) * obj.SensitivityY * 0.01).Clamp(obj.MinimumY, obj.MaximumY);

                await MQTT.SendMQTT(obj.TopicX, ((int)coordinates.X).ToString());
                await MQTT.SendMQTT(obj.TopicY, ((int)coordinates.Y).ToString());
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

    }
}
