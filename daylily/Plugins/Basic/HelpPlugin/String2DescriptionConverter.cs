using System.Globalization;
using System.Windows.Data;

namespace daylily.Plugins.Basic.HelpPlugin;

public class String2DescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
            return str.Trim();
        return "暂无帮助信息";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}