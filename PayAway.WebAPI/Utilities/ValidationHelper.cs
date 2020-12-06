using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Utilities
{
    public class ValidationHelper
    {
        public static bool IsAValidNumber(string number)
        {
            number = number.RemoveWhiteSpace();

            return (number
                .ToCharArray()
                .All(char.IsNumber) &&
                    !string.IsNullOrEmpty(number));
        }
    }
}
