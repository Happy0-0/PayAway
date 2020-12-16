using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PhoneNumbers;

namespace PayAway.WebAPI.DataAccess.Utilities
{
    internal static class PhoneNoHelpers
    {        
        internal static (bool isValidPhoneNo, string formattedPhoneNo, string normalizedPhoneNo) NormalizePhoneNo(string rawPhoneNo, string regionCode = @"US")
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();

            var parsedPhoneNumber = phoneNumberUtil.Parse(rawPhoneNo, regionCode);

            bool isValidPhoneNo = phoneNumberUtil.IsValidNumberForRegion(parsedPhoneNumber, regionCode);

            var normalizedPhoneNo = phoneNumberUtil.Format(parsedPhoneNumber, PhoneNumberFormat.E164);
            var formattedPhoneNo = phoneNumberUtil.Format(parsedPhoneNumber, PhoneNumberFormat.NATIONAL);

            return (isValidPhoneNo, formattedPhoneNo, normalizedPhoneNo);
        }
    }
}
