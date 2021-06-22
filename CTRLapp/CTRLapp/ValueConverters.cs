using System;
using Xamarin.Forms;

namespace CTRLapp
{
    public class HexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType = null, object parameter = null, System.Globalization.CultureInfo culture = null)
        {
            if ((string)value == "color_primary") return Variables.Variables.PrimaryColor;
            if ((string)value == "color_secondary") return Variables.Variables.SecondaryColor;
            return Color.FromHex((string)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((Color)value == Variables.Variables.PrimaryColor) return "color_primary";
            if ((Color)value == Variables.Variables.SecondaryColor) return "color_secondary";
            return ((Color)value).ToHex();
        }
    }
    public class FloatToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)(double)value;
        }
    }
    public class IntToFloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)(double)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (string)value;
        }
    }
}
