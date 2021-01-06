using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using GO.UWP.Player.Extensions;
using GO.UWP.Player.Static;

namespace GO.UWP.Player.Converters
{
    class CountryItemToFlagStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!(value is CountryItem CI)) return DependencyProperty.UnsetValue;

            return CI.NationalDomain.IsoCountryCodeToFlagEmoji() + " - " + CI.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
