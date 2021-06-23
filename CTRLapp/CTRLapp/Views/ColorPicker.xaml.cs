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
                nameof(SelectedColor), typeof(Color), typeof(ColorPicker));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set
            {
                Debug.WriteLine("color set----------------------------");
                SetValue(SelectedColorProperty, value);
                OnPropertyChanged();
                ColorPicked?.Invoke(null, new ColorPickedEventArgs { newColor = value });


                //visuals
                HueSlider.Value = SelectedColor.Hue;
                SaturationSlider.Value = SelectedColor.Saturation;
                LuminiocitySlider.Value = SelectedColor.Luminosity;
                AlphaSlider.Value = SelectedColor.A;
                HexEntry.Text = SelectedColor.ToHex();

                SaturationSlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(SelectedColor.Hue, 0, SelectedColor.Luminosity, SelectedColor.A), 0),
                    new GradientStop(Color.FromHsla(SelectedColor.Hue, 1, SelectedColor.Luminosity, SelectedColor.A), 1),
                    }, new Point(0, 0), new Point(1, 0));
                LuminiocitySlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(SelectedColor.Hue, SelectedColor.Saturation, 0, SelectedColor.A), 0),
                    new GradientStop(Color.FromHsla(SelectedColor.Hue, SelectedColor.Saturation, 0.5, SelectedColor.A), (float)0.5),
                    new GradientStop(Color.FromHsla(SelectedColor.Hue, SelectedColor.Saturation, 1, SelectedColor.A), 1),
                    }, new Point(0, 0), new Point(1, 0));
                AlphaSlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(SelectedColor.Hue, SelectedColor.Saturation, SelectedColor.Luminosity, 0), 0),
                    new GradientStop(Color.FromHsla(SelectedColor.Hue, SelectedColor.Saturation, SelectedColor.Luminosity, 1), 1),
                    }, new Point(0, 0), new Point(1, 0));
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