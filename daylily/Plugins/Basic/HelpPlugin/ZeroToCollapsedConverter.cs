using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace daylily.Plugins.Basic.HelpPlugin;

public class ZeroToCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            var i = System.Convert.ToInt32(value);
            if (i == 0) return Visibility.Collapsed;
            return Visibility.Visible;
        }
        catch (Exception e)
        {
            return Visibility.Visible;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}