using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using AccoutingDocs.Models;
using Type = System.Type;

namespace AccoutingDocs.Converter
{
    public class SearchConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null || String.IsNullOrEmpty(values[0].ToString()))
                return new SolidColorBrush(Colors.Transparent);
            if (values[1].GetType() == typeof(Documents))
            {
                Documents document = (Documents)values[1];
                return document.Search(values[0].ToString()) ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#488228")) : new SolidColorBrush(Colors.Transparent);
            }
            if (values[1].GetType() == typeof(Register))
            {
                Register document = (Register)values[1]; 
                return document.Search(values[0].ToString()) ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#488228")) : new SolidColorBrush(Colors.Transparent);
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
