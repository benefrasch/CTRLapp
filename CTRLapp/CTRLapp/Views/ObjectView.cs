using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using CTRLapp.Objects.Objects;

namespace CTRLapp.Views
{
    public class ObjectView : ContentView
    {
        private static readonly HexToColorConverter colorConverter = new();
        public ObjectView(int masterMenu, int bottomMenu, int objIndex)
        {

            Objects.BaseObject obj = Variables.Variables.Layout[masterMenu].BottomMenuItems[bottomMenu].Objects[objIndex];

            nameof(obj);
            
            switch (obj.Type)
            {

                case "Label":
                    Content = LabelObject.BuildLabel((LabelObject)obj);
                    break;
                    
                case "ValueDisplay":
                    Content = ValueDisplayObject.BuildValueDisplay((ValueDisplayObject)obj);
                    break;

                case "Button":
                    Content = ButtonObject.BuildButton((ButtonObject)obj);
                    break;

                case "SwitchButton":
                    Content = SwitchButtonObject.BuildSwitchButton((SwitchButtonObject)obj);
                    break;

                case "Switch":
                    Content = SwitchObject.BuildSwitch((SwitchObject)obj);
                    break;

                case "Slider":
                    Content = SliderObject.BuildSlider((SliderObject)obj);
                    break;

                case "Joystick":
                    Content = JoystickObject.BuildJoystick((JoystickObject)obj);
                    break;

                case "Matrix":
                    Content = MatrixObject.BuildMatrix((MatrixObject)obj);
                    break;
            }


        }


    }
}