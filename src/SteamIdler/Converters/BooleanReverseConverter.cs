using System;
using System.Globalization;
using System.Windows.Data;

namespace SteamIdler.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolean))
            {
                return Binding.DoNothing;
            }

            return !boolean;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolean))
            {
                return Binding.DoNothing;
            }

            return !boolean;
        }
    }
}
