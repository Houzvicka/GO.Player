using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GO.UWP.Player.Converters
{
    public class ValueToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// If set to true, the converter returns inverted results.
        /// </summary>
        public bool VisibleWhenFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool visible = (value is string ? !string.IsNullOrWhiteSpace((string)(value)) : (value != null)) ^ VisibleWhenFalse;
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
