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
    public class MatrixObject : BaseObject
    {
        public Color BackgroundColor { get; set; }
        public Color ThumbColor { get; set; }
        public string TopicX { get; set; }
        public int MinimumX { get; set; }
        public int MaximumX { get; set; }  
        public string TopicY { get; set; }
        public int MinimumY { get; set; }
        public int MaximumY { get; set; }


        public static View BuildMatrix(MatrixObject obj)
        {
            float thumbRadius = 50;
            var touchEffect = new TouchTracking.Forms.TouchEffect() { Capture = true };
            SKCanvasView canvas = new()
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                EnableTouchEvents = false,
                BackgroundColor = obj.BackgroundColor,
            };
            SKPoint touch = new();
            canvas.PaintSurface += (_, e) =>
            {
                var surface = e.Surface.Canvas;
                surface.Clear();

                SKPaint thumbPaint = new()
                {
                    Style = SKPaintStyle.Fill,
                    Color = obj.ThumbColor.ToSKColor(),
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
                            X = clamped.X * (obj.MaximumX - obj.MinimumX) + obj.MinimumX,
                            Y = clamped.Y * (obj.MaximumY - obj.MinimumY) + obj.MinimumY,
                        };

                        await MQTT.SendMQTT(obj.TopicX, ((int)coordinates.X).ToString());
                        await MQTT.SendMQTT(obj.TopicY, ((int)coordinates.Y).ToString());
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
