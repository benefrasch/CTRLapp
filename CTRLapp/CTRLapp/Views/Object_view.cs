using CTRLapp.Views.Settings_pages;
using CTRLapp.Views.Settings_pages.GUI;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using System.Timers;
using Xamarin.Forms;

namespace CTRLapp.Views
{
    public class Object_view : ContentView
    {
        private int master_menu, bottom_menu, obj_index;
        public Object_view(int master_menu, int bottom_menu, int obj_index)
        {
            this.master_menu = master_menu; this.bottom_menu = bottom_menu; this.obj_index = obj_index;


            Objects.Object obj = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];


            switch (obj.Type)
            {

                case "Label":
                    Content = Build_Label(obj);
                    break;
                case "Button":
                    Content = Build_Button(obj);
                    break;

                case "Switch":
                    Content = Build_Switch(obj);
                    break;

                case "Slider":
                    Content = Build_Slider(obj);
                    break;

                case "Joystick":
                    Content = Build_Joystick(obj);
                    break;
                case "Matrix":
                    Content = Build_Matrix(obj);
                    break;
            }


        }

        private View Build_Label(Objects.Object obj)
        {
            Int32.TryParse(obj.Arguments[3], out int fontsize);
            Label label = new Label()
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                TextColor = Color.FromHex(obj.Arguments[0]),
                BackgroundColor = Color.FromHex(obj.Arguments[1]),
                Text = obj.Arguments[2],
                FontSize = fontsize,
            };
            return label;
        }
        private View Build_Button(Objects.Object obj)
        {
            var temp1 = new Button
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                BackgroundColor = Color.FromHex(obj.Arguments[1]),
                TextColor = Color.FromHex(obj.Arguments[0]),
                Text = obj.Arguments[2],
                TextTransform = TextTransform.None,
            };

            temp1.Clicked += async (s, e) =>
            {
                Debug.WriteLine("sending mqtt message");
                await MQTT.SendMQTT(obj.Arguments[3], obj.Arguments[4]);
            };
            return temp1;
        }
        private View Build_Switch(Objects.Object obj)
        {
            var temp2 = new Xamarin.Forms.Switch
            {
                ThumbColor = Color.FromHex(obj.Arguments[0]),
                OnColor = Color.FromHex(obj.Arguments[1]),
            };
            temp2.Toggled += async (s, e) =>
            {
                Debug.WriteLine("sending mqtt message");
                string message = obj.Arguments[3];
                if (e.Value) message = obj.Arguments[4];
                await MQTT.SendMQTT(obj.Arguments[2], message);
            };
            return temp2;
        }
        private View Build_Slider(Objects.Object obj)
        {
            var temp3 = new Slider
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                ThumbColor = Color.FromHex(obj.Arguments[0]),
                MinimumTrackColor = Color.FromHex(obj.Arguments[1]),
                MaximumTrackColor = Color.FromHex(obj.Arguments[2]),
                Minimum = int.Parse(obj.Arguments[4]),
                Maximum = int.Parse(obj.Arguments[5]),
            };
            temp3.ValueChanged += async (s, e) =>
            {
                Debug.WriteLine("sending mqtt message");
                await MQTT.SendMQTT(obj.Arguments[3], e.NewValue.ToString());
            };
            return temp3;
        }
        private View Build_Joystick(Objects.Object obj)
        {
            var touchEffect = new TouchTracking.Forms.TouchEffect() { Capture = true };
            SKCanvasView canvas = new SKCanvasView
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                EnableTouchEvents = false,
                BackgroundColor = Color.Gray,
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

            canvas.PaintSurface += (s, e) =>
            {
                var surface = e.Surface.Canvas;
                surface.Clear();

                SKPaint background_paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.FromHex(obj.Arguments[1]).ToSKColor(),
                };
                float radius = canvassize.Width / 2;
                if (canvassize.Width < canvassize.Height) radius = canvassize.Height / 2;
                surface.DrawCircle(canvassize.Width / 2, canvassize.Height / 2, radius, background_paint);

                SKPaint thumb_paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.FromHex(obj.Arguments[0]).ToSKColor(),
                };
                surface.DrawCircle((float)touch.X, (float)touch.Y, radius / 3, thumb_paint);
            };
            touchEffect.TouchAction += (s, e) =>
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
            timer.Elapsed += async (s, e) =>
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

                Debug.WriteLine("sending mqtt message");
                await MQTT.SendMQTT(obj.Arguments[2], coordinates.X.ToString());
                await MQTT.SendMQTT(obj.Arguments[3], coordinates.Y.ToString());
            };
            canvas.Effects.Add(touchEffect);
            return canvas;
        }
        private View Build_Matrix(Objects.Object obj)
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
            canvas.PaintSurface += (s, e) =>
            {
                var surface = e.Surface.Canvas;
                surface.Clear();

                float radius = 50;
                SKPaint thumb_paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.FromHex(obj.Arguments[0]).ToSKColor(),
                };
                surface.DrawCircle((float)touch.X, (float)touch.Y, radius / 3, thumb_paint);
            };
            touchEffect.TouchAction += async (s, e) =>
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
                            Y = (e.Location.X / canvas.Width) * (float.Parse(obj.Arguments[7]) - float.Parse(obj.Arguments[6])) + float.Parse(obj.Arguments[6]),
                        };

                        Debug.WriteLine("sending mqtt message");
                        await MQTT.SendMQTT(obj.Arguments[2], coordinates.X.ToString());
                        await MQTT.SendMQTT(obj.Arguments[3], coordinates.Y.ToString());
                        canvas.InvalidateSurface();

                        break;
                }
            };


            canvas.Effects.Add(touchEffect);
            return canvas;
        }







        private async void Check_Error(string result)
        {
            return; // not errorcheck not working properly right now
#pragma warning disable CS0162 // Unerreichbarer Code wurde entdeckt.
            if (result == "connection_failed")
            {
                if (!Variables.Variables.Alert_active)
                {
                    Variables.Variables.Alert_active = true;
                    await App.Current.MainPage.DisplayAlert("Error", "Connecting to Broker failed", "ok");
                    await App.Current.MainPage.Navigation.PushModalAsync(new Settings_page());
                    Variables.Variables.Alert_active = false;
                }
            }
            else if (result == "message_sending_failed")
            {
                if (!Variables.Variables.Alert_active)
                {
                    Variables.Variables.Alert_active = true;
                    await App.Current.MainPage.DisplayAlert("Error", "MQTT topic not set", "ok");
                    await App.Current.MainPage.Navigation.PushModalAsync(new Object_page(master_menu, bottom_menu, obj_index));
                    Variables.Variables.Alert_active = false;
                }

            }
#pragma warning restore CS0162 // Unerreichbarer Code wurde entdeckt.
        }
    }
}