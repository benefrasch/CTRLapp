using CTRLapp.Variables;
using CTRLapp.Views.Settings_pages;
using CTRLapp.Views.Settings_pages.GUI;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace CTRLapp.Views
{
    public class Object_view : ContentView
    {
        public static View View(int master_menu, int bottom_menu, int obj_index)
        {
            //Objects.Object obj = JsonConvert.DeserializeObject<List<Master_Menu_Item>>(Json_string.Config)[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];
            Objects.Object obj = Json_string.Array[master_menu].Bottom_Menu_Items[bottom_menu].Objects[obj_index];

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
            };

            switch (obj.Type)
            {
                case "Rectangle":
                    frame.BackgroundColor = Color.FromHex(obj.Arguments[0]);
                    break;

                case "Button":
                    Build_Button(frame, obj, master_menu, bottom_menu, obj_index);
                    break;

                case "Switch":
                    Build_Switch(frame, obj, master_menu, bottom_menu, obj_index);
                    break;

                case "Slider":
                    Build_Slider(frame, obj, master_menu, bottom_menu, obj_index);
                    break;

                case "Joystick":
                    Build_Joystick(frame, obj, master_menu, bottom_menu, obj_index);
                    break;

            }

            return frame;
        }



        private static void Build_Button(Frame frame, Objects.Object obj, int master_menu, int bottom_menu, int obj_index)
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
                Check_Error(result, master_menu, bottom_menu, obj_index);
            };
            frame.Content = temp1;
        }
        private static void Build_Switch(Frame frame, Objects.Object obj, int master_menu, int bottom_menu, int obj_index)
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
                Check_Error(result, master_menu, bottom_menu, obj_index);
            };
            frame.Content = temp2;
        }
        private static void Build_Slider(Frame frame, Objects.Object obj, int master_menu, int bottom_menu, int obj_index)
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
                Check_Error(result, master_menu, bottom_menu, obj_index);
            };
            frame.Content = temp3;
        }
        private static void Build_Joystick(Frame frame, Objects.Object obj, int master_menu, int bottom_menu, int obj_index)
        {
            Grid grid = new Grid();
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
                surface.DrawCircle((float)touch.X, (float)touch.Y, 50, thumb_paint);
            };
            touchEffect.TouchAction += (s, e) =>
            {
                Debug.WriteLine("touchaction" + e.Type);
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
                        canvas.InvalidateSurface();
                        break;
                    case TouchTracking.TouchActionType.Released:
                        touch.X = canvas.CanvasSize.Width / 2;
                        touch.Y = canvas.CanvasSize.Height / 2;
                        canvas.InvalidateSurface();
                        pressed = false;
                        timer.Stop();
                        break;
                }
            };
            canvas.SizeChanged += (s, e) =>
            {
                touch.X = canvas.CanvasSize.Width / 2;
                touch.Y = canvas.CanvasSize.Height / 2;
                canvas.InvalidateSurface();
                Debug.WriteLine("binding context changed");
            };
            timer.Elapsed += async (s, e) =>
            {
                coordinates.X += (touch.X - canvassize.Width / 2) * float.Parse(obj.Arguments[6]) * 0.1;
                coordinates.Y += (touch.Y - canvassize.Height / 2) * float.Parse(obj.Arguments[9]) * 0.1;

                int maximumx = int.Parse(obj.Arguments[5]),
                    minimumx = int.Parse(obj.Arguments[4]),
                    maximumy = int.Parse(obj.Arguments[8]),
                    minimumy = int.Parse(obj.Arguments[7]);
                if (coordinates.X > maximumx) coordinates.X = maximumx;
                if (coordinates.X < minimumx) coordinates.X = minimumx;
                if (coordinates.Y > maximumy) coordinates.Y = maximumy;
                if (coordinates.Y < minimumy) coordinates.Y = minimumy;

                Debug.WriteLine("sending mqtt message");
                string result0 = await MQTT.SendMQTT(obj.Arguments[2], coordinates.X.ToString());
                Check_Error(result0, master_menu, bottom_menu, obj_index);
                string result1 = await MQTT.SendMQTT(obj.Arguments[3], coordinates.Y.ToString());
                Check_Error(result1, master_menu, bottom_menu, obj_index);
            };
            grid.Children.Add(canvas);
            grid.Effects.Add(touchEffect);
            frame.Content = grid;
        }


        private static async void Check_Error(string result, int master_menu, int bottom_menu, int obj_index)
        {
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
        }
    };
}