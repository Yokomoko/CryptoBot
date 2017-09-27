using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CryptoBot.Resources.Converters
{
    public class NegativeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.DarkGreen);
            double doubleValue = 0.0;
            Double.TryParse(value.ToString(), out doubleValue);

            if (doubleValue < 0)
                brush = new SolidColorBrush(Colors.Red);

            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
