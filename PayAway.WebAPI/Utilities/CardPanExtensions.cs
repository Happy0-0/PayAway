using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayAway.WebAPI.Utilities
{
    /// <summary>
    /// Useful methods for Card Pans
    /// </summary>
    public static class CardPanExtensions
    {
        // Convert to int.
        /// <summary>
        /// The character to int
        /// </summary>
        private static readonly Func<char, int> CharToInt = c => c - '0';
        /// <summary>
        /// The is even
        /// </summary>
        private static readonly Func<int, bool> IsEven = i => i % 2 == 0;

        // New Double Concept => 7 * 2 = 14 => 1 + 4 = 5.
        /// <summary>
        /// The double digit
        /// </summary>
        private static readonly Func<int, int> DoubleDigit = i => (i * 2).ToString().ToCharArray().Select(CharToInt).Sum();

        /// <summary>
        /// Verify if the card number is valid.
        /// </summary>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">Invalid number. Just numbers and white spaces are accepted on the string.</exception>
        public static bool CheckLuhn(this string creditCardNumber)
        {
            if (!ValidationHelper.IsAValidNumber(creditCardNumber))
            {
                throw new ArgumentException("Invalid number. Just numbers and white spaces are accepted on the string.");
            }

            var checkSum = creditCardNumber
                .CleanUp()
                .ToCharArray()
                .Select(CharToInt)
                .Reverse()
                .Select((digit, index) => IsEven(index + 1) ? DoubleDigit(digit) : digit)
                .Sum();

            return checkSum % 10 == 0;
        }

        /// <summary>
        /// Creates the check digit.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">Invalid number. Just numbers and white spaces are accepted on the string.</exception>
        public static string CreateCheckDigit(this string number)
        {
            if (!ValidationHelper.IsAValidNumber(number))
            {
                throw new ArgumentException("Invalid number. Just numbers and white spaces are accepted on the string.");
            }

            var digitsSum = number
                .RemoveWhiteSpace()
                .ToCharArray()
                .Reverse()
                .Select(CharToInt)
                .Select((digit, index) => IsEven(index) ? DoubleDigit(digit) : digit)
                .Sum();

            digitsSum *= 9;

            return digitsSum
                .ToString()
                .ToCharArray()
                .Last()
                .ToString();
        }
    
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
