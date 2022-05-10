using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CTRLapp.Views.SettingsPages.EntryViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPicker : ContentView
    {
        private bool changing = false;

        public event EventHandler<EventArgs> ValueChanged;


        //Selected Color Property
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set
            {
                if (changing) return;
                SetValue(SelectedColorProperty, value);
                OnPropertyChanged();
            }
        }
        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
                propertyName: "SelectedColor",
                returnType: typeof(Color),
                declaringType: typeof(ColorPicker),
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: SelectedColorPropertyChanged);
        private static void SelectedColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ColorPicker control = (ColorPicker)bindable;
            control.ValueChanged?.Invoke(control, null);

            control.changing = true;
            Color newColor = (Color)newValue;
            control.HueSlider.Value = newColor.Hue;
            control.SaturationSlider.Value = newColor.Saturation;
            control.LuminiocitySlider.Value = newColor.Luminosity;
            control.AlphaSlider.Value = newColor.A;
            control.HexEntry.Text = newColor.ToHex();

            control.SaturationSlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(newColor.Hue, 0, control.SelectedColor.Luminosity, newColor.A), 0),
                    new GradientStop(Color.FromHsla(newColor.Hue, 1, control.SelectedColor.Luminosity, newColor.A), 1),
                    }, new Point(0, 0), new Point(1, 0));
            control.LuminiocitySlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(newColor.Hue, control.SelectedColor.Saturation, 0, newColor.A), 0),
                    new GradientStop(Color.FromHsla(newColor.Hue, control.SelectedColor.Saturation, 0.5, newColor.A), (float)0.5),
                    new GradientStop(Color.FromHsla(newColor.Hue, control.SelectedColor.Saturation, 1, newColor.A), 1),
                    }, new Point(0, 0), new Point(1, 0));
            control.AlphaSlider.Background = new LinearGradientBrush(new GradientStopCollection() {
                    new GradientStop(Color.FromHsla(newColor.Hue, newColor.Saturation, newColor.Luminosity, 0), 0),
                    new GradientStop(Color.FromHsla(newColor.Hue, newColor.Saturation, newColor.Luminosity, 1), 1),
                    }, new Point(0, 0), new Point(1, 0));
            control.changing = false;
        }


        // Label Text Property
        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }
        public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(
                propertyName: "LabelText",
                returnType: typeof(string),
                declaringType: typeof(ColorPicker),
                defaultBindingMode: BindingMode.OneTime);



        public ColorPicker()
        {
            InitializeComponent();
            stack.BindingContext = this;
        }

        private void HexColorChanged(object sender, ValueChangedEventArgs e)
        {
            SelectedColor = Color.FromHex(HexEntry.Text);
        }
        private void SliderColorChanged(object sender, ValueChangedEventArgs e)
        {
            SelectedColor = Color.FromHsla(HueSlider.Value, SaturationSlider.Value, LuminiocitySlider.Value, AlphaSlider.Value);
        }
    }
}