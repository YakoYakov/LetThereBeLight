using System;
using System.Globalization;
using Xamarin.Forms;

namespace LetThereBeLightApp.Helpers
{
    public class NameToPowerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "On" ? "LightbulbOn.png" : "LightbulbOff.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "LightbulbOn.svg" ? "On" : "Off";
        }
    }
}
