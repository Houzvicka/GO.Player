using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO.UWP.Player.Extensions
{
    /// <summary>
    /// Helper class for converting Exception object into comprehensive string.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Converts <see cref="Exception"/> object into nicely formatted string message.
        /// </summary>
        /// <param name="ex">The Exeption to be converted.</param>
        /// <param name="maxLevel">Max level of InnerExceptions to log.</param>
        public static string ToNiceString(this Exception ex, int maxLevel = 3)
        {
            if (ex == null) return string.Empty;

            string str = $"{ex.GetType().FullName}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}\n";

            // get the inner exception and log it, if requested
            ex = ex.InnerException;
            if (ex != null && maxLevel > 0)
            {
                str += ex.ToNiceString(maxLevel - 1);
            }
            return str;
        }
    }
}
