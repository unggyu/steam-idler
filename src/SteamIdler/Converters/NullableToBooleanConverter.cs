using System;
using System.Globalization;
using System.Windows.Data;

namespace SteamIdler.Converters
{
    [ValueConversion(typeof(Nullable), typeof(bool))]
    public class NullableToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
