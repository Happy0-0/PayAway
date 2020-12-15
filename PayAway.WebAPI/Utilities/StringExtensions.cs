using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Utilities
{
    /// <summary>
    /// Class StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>Removes the white space.</summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string RemoveWhiteSpace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        /// <summary>
        /// Gets the last.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="numberOfChars">The number of chars.</param>
        /// <returns>System.String.</returns>
        public static string GetLast(this string source, int numberOfChars)
        {
            if (numberOfChars >= source.Length)
                return source;
            //return source.Substring(source.Length - numberOfChars);
            return source[^numberOfChars..];
        }

        /// <summary>Gets the first.</summary>
        /// <param name="source">The source.</param>
        /// <param name="numberOfChars">The number of chars.</param>
        /// <returns>System.String.</returns>
        public static string GetFirst(this string source, int numberOfChars)
        {
            if (numberOfChars >= source.Length)
                return source;
            return source.Substring(0, numberOfChars);
        }
    }
}
