using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SteamIdler.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(string))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolean && parameter is string visibilityStr))
            {
                return Binding.DoNothing;
            }

            return boolean ? Visibility.Visible : Enum.Parse(typeof(Visibility), visibilityStr);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility visibility))
            {
                return Binding.DoNothing;
            }

            return visibility == Visibility.Visible;
        }
    }
}
