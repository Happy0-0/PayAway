using System;
using System.Collections.Generic;
using System.Text;

namespace PayAway.WebAPI.Utilities
{
    /// <summary>
    /// Useful methods for Card Pans
    /// </summary>
    public static class CardPanExtensions
    {
        /// <summary>Cleanups the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string CleanUp(this string input)
        {
            string newString = input.RemoveWhiteSpace();
            newString = newString.Replace(@"-", string.Empty);

            return newString;
        }

        /// <summary>Masks the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string Mask(this string input)
        {
            string first6 = input.GetFirst(6);
            string last4 = input.GetLast(4);

            return $"{first6.GetFirst(4)}-{first6.GetLast(2)}XX-XXXX-{last4}";
        }
    }
}
