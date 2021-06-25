using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPicker : ContentView
    {
        public event EventHandler<ColorPickedEventArgs> ColorPicked;

        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
                "SelectedColor", typeof(Color), typeof(ColorPicker), defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColorPropertyChanged);

        private static void ColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ColorPicker)bindable;
            control.ColorPicked?.Invoke(null, new ColorPickedEventArgs { newColor = newValue });

            control.changing = true;
            control.HueSlider.Value = control.SelectedColor.Hue;
            control.SaturationSlider.Value = control.SelectedColor.Saturation;
            control.LuminiocitySlider.Value = control.SelectedColor.Luminosity;
            control.AlphaSlider.Value = control.SelectedColor.A;
            control.HexEntry.Text = control.SelectedColor.ToHex();

            control.SaturationSlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(control.SelectedColor.Hue, 0, control.SelectedColor.Luminosity, control.SelectedColor.A), 0),
                    new GradientStop(Color.FromHsla(control.SelectedColor.Hue, 1, control.SelectedColor.Luminosity, control.SelectedColor.A), 1),
                    }, new Point(0, 0), new Point(1, 0));
            control.LuminiocitySlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(control.SelectedColor.Hue, control.SelectedColor.Saturation, 0, control.SelectedColor.A), 0),
                    new GradientStop(Color.FromHsla(control.SelectedColor.Hue, control.SelectedColor.Saturation, 0.5, control.SelectedColor.A), (float)0.5),
                    new GradientStop(Color.FromHsla(control.SelectedColor.Hue, control.SelectedColor.Saturation, 1, control.SelectedColor.A), 1),
                    }, new Point(0, 0), new Point(1, 0));
            control.AlphaSlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(control.SelectedColor.Hue, control.SelectedColor.Saturation, control.SelectedColor.Luminosity, 0), 0),
                    new GradientStop(Color.FromHsla(control.SelectedColor.Hue, control.SelectedColor.Saturation, control.SelectedColor.Luminosity, 1), 1),
                    }, new Point(0, 0), new Point(1, 0));
            control.changing = false;
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set
            {
                SetValue(SelectedColorProperty, value);
                OnPropertyChanged();
            }
        }


        private bool changing = false;

        public ColorPicker()
        {
            InitializeComponent();
        }

        private void HexColorChanged(object sender, ValueChangedEventArgs e)
        {
            if (changing) return;
            changing = true;
            SelectedColor = Color.FromHex(HexEntry.Text);
            changing = false;
        }
        private void SliderColorChanged(object sender, ValueChangedEventArgs e)
        {
            if (changing) return;
            changing = true;
            SelectedColor = Color.FromHsla(HueSlider.Value, SaturationSlider.Value, LuminiocitySlider.Value, AlphaSlider.Value);
            changing = false;
        }
    }

    public class ColorPickedEventArgs : EventArgs
    {
        public Color newColor;
    }
}