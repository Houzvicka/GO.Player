using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using HBO.UWP.Player.Model;

namespace HBO.UWP.Player.Converters
{
    public class ContentToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// If set to true, the converter returns inverted results.
        /// </summary>
        public bool VisibleWhenFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is long)) return DependencyProperty.UnsetValue;

            bool retValue = false;

            switch ((long)value)
            {
                case 1L: // movie detail
                    retValue = true;
                    break;
                case 5L: // show list
                    retValue = false;
                    break;
                case 3L: // ????
                    retValue = true;
                    break;
            }
            
            return retValue ^ VisibleWhenFalse ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
