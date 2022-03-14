using System.Globalization;
using System.Windows.Data;
using daylily.Utils;

namespace daylily.Plugins.Basic.HelpPlugin;

public class String2LowerSnakeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
            return str.ToLowerSnake();
        return value?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}