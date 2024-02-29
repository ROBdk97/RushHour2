using System;
using System.Globalization;
using System.Windows.Data;
namespace RushHour2;
public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            // Format the TimeSpan to minutes:seconds:milliseconds
            return timeSpan.ToString(@"mm\:ss\:fff");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Implement conversion back if necessary, or throw an exception if not supported
        throw new NotSupportedException();
    }
}
