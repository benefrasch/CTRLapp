using CTRLapp.Views.Settings_pages;
using CTRLapp.Views.Settings_pages.GUI;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using System.Timers;
using Xamarin.Forms;

namespace CTRLapp.Views
{
    public class Object_view : ContentView
    {
        private int master_menu, bottom_menu, obj_index;
        public Object_view(int master_menu, int bottom_menu, int obj_index, bool noTranslation = false)
        {
            this.master_menu = master_menu; this.bottom_menu = bottom_menu; this.obj_index = obj_index;

            Objects.Object obj = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];
            var frame = new Frame
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                TranslationX = obj.X,
                TranslationY = obj.Y,
                CornerRadius = 5,
                Rotation = obj.Rotation,
                BorderColor = Color.Transparent,
                BackgroundColor = Color.Transparent,
                HasShadow = false
            };

            if (noTranslation)
            {
                frame.TranslationX = 0; frame.TranslationY = 0;
            }
            frame.BindingContext = Variables.Variables.Layout[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];

            switch (obj.Type)
            {
                case "Rectangle":
                    frame.BackgroundColor = Color.FromHex(obj.Arguments[0]);
                    break;

                case "Button":
                    Build_Button(frame, obj);
                    break;

                case "Switch":
                    Build_Switch(frame, obj);
                    break;

                case "Slider":
                    Build_Slider(frame, obj);
                    break;

                case "Joystick":
                    Build_Joystick(frame, obj);
                    break;

            }

            //return frame;
            Content = frame;
        }



        private void Build_Button(Frame frame, Objects.Object obj)
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
                string result = await MQTT.SendMQTT(obj.Arguments[3], obj.Arguments[4]);
            };
            frame.Content = temp1;
        }
        private static void Build_Switch(Frame frame, Objects.Object obj)
        {
            var temp2 = new Xamarin.Forms.Switch
            {
                ThumbColor = Color.FromHex(obj.Arguments[0]),
                OnColor = Color.FromHex(obj.Arguments[1]),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            temp2.Toggled += async (s, e) =>
            {
                Debug.WriteLine("sending mqtt message");
                string message = obj.Arguments[3];
                if (e.Value) message = obj.Arguments[4];
                string result = await MQTT.SendMQTT(obj.Arguments[2], message);
            };
            frame.Content = temp2;
        }
        private static void Build_Slider(Frame frame, Objects.Object obj)
        {
            var temp3 = new Slider
            {
                HeightRequest = obj.Height,
                WidthRequest = obj.Width,
                ThumbColor = Color.FromHex(obj.Arguments[0]),
                MinimumTrackColor = Color.FromHex(obj.Arguments[1]),
                MaximumTrackColor = Color.FromHex(obj.Arguments[2]),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Minimum = int.Parse(obj.Arguments[4]),
                Maximum = int.Parse(obj.Arguments[5]),
            };
            temp3.ValueChanged += async (s, e) =>
            {
                Debug.WriteLine("sending mqtt message");
                string result = await MQTT.SendMQTT(obj.Arguments[3], e.NewValue.ToString());
            };
            frame.Content = temp3;
        }
        private static void Build_Joystick(Frame frame, Objects.Object obj)
        {
            frame.BorderColor = Color.LightGray;
            var touchEffect = new TouchTracking.Forms.TouchEffect() { Capture = true };
            SKCanvasView canvas = new SKCanvasView
            {
                EnableTouchEvents = false,
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
                Debug.WriteLine("drawing on canvas surface");
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
                coordinates.X += (touch.X - canvassize.Width / 2) * float.Parse(obj.Arguments[6]) * 0.1;
                coordinates.Y += (touch.Y - canvassize.Height / 2) * float.Parse(obj.Arguments[9]) * 0.1;

                int minimumx = int.Parse(obj.Arguments[4]),
                    maximumx = int.Parse(obj.Arguments[5]),
                    minimumy = int.Parse(obj.Arguments[7]),
                    maximumy = int.Parse(obj.Arguments[8]);
                if (coordinates.X < minimumx) coordinates.X = minimumx;
                if (coordinates.X > maximumx) coordinates.X = maximumx;
                if (coordinates.Y < minimumy) coordinates.Y = minimumy;
                if (coordinates.Y > maximumy) coordinates.Y = maximumy;

                Debug.WriteLine("sending mqtt message");
                string result0 = await MQTT.SendMQTT(obj.Arguments[2], coordinates.X.ToString());
                string result1 = await MQTT.SendMQTT(obj.Arguments[3], coordinates.Y.ToString());
            };
            canvas.Effects.Add(touchEffect);
            frame.Content = canvas;
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
    };
}