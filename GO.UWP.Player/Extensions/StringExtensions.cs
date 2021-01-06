using System.Linq;

namespace GO.UWP.Player.Extensions
{
    public static class StringExtensions
    {
        public static string IsoCountryCodeToFlagEmoji(this string country)
        {
            return string.Concat(country.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
        }
    }
}