using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace NetClient.Common.Schedule.Formatters
{
    internal class BoolToInvisibilityFormatter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return System.Windows.Visibility.Visible;
                if ((bool)value)
                    return System.Windows.Visibility.Collapsed;
                return System.Windows.Visibility.Visible;
            }
            catch (Exception)
            {
                return System.Windows.Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IValueConverter Members
    }
}