using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PayAway.WebAPI.DataAccess.Utilities
{
    /// <summary>
    /// Class CardNetworkHelper.
    /// </summary>
    public static class CardNetworkHelper
    {
        /// <summary>
        /// The prefixes american express
        /// </summary>
        static readonly List<string> PREFIXES_AMERICAN_EXPRESS = new List<string>() { "34", "37" };
        /// <summary>
        /// The prefixes discover
        /// </summary>
        static readonly List<string> PREFIXES_DISCOVER = new List<string>() { "60", "62", "64", "65" };
        /// <summary>
        /// The prefixes JCB
        /// </summary>
        static readonly List<string> PREFIXES_JCB = new List<string>() { "35" };
        /// <summary>
        /// The prefixes diners club
        /// </summary>
        static readonly List<string> PREFIXES_DINERS_CLUB = new List<string>() { "300", "301", "302", "303", "304", "305", "309", "36", "38", "39" };
        /// <summary>
        /// The prefixes visa
        /// </summary>
        static readonly List<string> PREFIXES_VISA = new List<string>() { "4" };
        /// <summary>
        /// The prefixes mastercard
        /// </summary>
        static readonly List<string> PREFIXES_MASTERCARD = new List<string>() {
                "2221", "2222", "2223", "2224", "2225", "2226", "2227", "2228", "2229",
                "223", "224", "225", "226", "227", "228", "229",
                "23", "24", "25", "26",
                "270", "271", "2720",
                "50", "51", "52", "53", "54", "55"
            };

        //public static string GetNetworkLogoFileName(string networkToken)
        //{
        //    string configItemValue = MongoDBContext.GetConfigValue($"{GeneralConstants.NETWORK_LOGO_FILENAME_PREFIX}-{networkToken}");

        //    return configItemValue;
        //}

        /// <summary>
        /// Gets the card type from number.
        /// </summary>
        /// <param name="cardPan">The card pan.</param>
        /// <returns>System.String.</returns>
        public static string GetCardTypeFromNumber(string cardPan)
        {
            String evaluatedType;
            if (PREFIXES_AMERICAN_EXPRESS.Any(cardPan.StartsWith))
            {
                evaluatedType = GeneralConstants.AMERICAN_EXPRESS_CARD_TYPE;
            }
            else if (PREFIXES_DISCOVER.Any(cardPan.StartsWith))
            {
                evaluatedType = GeneralConstants.DISCOVER_CARD_TYPE;
            }
            else if (PREFIXES_VISA.Any(cardPan.StartsWith))
            {
                evaluatedType = GeneralConstants.VISA_CARD_TYPE;
            }
            else if (PREFIXES_MASTERCARD.Any(cardPan.StartsWith))
            {
                evaluatedType = GeneralConstants.MASTERCARD_CARD_TYPE;
            }
            else
            {
                evaluatedType = @"UNKNOWN";
            }
            return evaluatedType;
        }

        /// <summary>
        /// Generates the authentication code.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GenerateAuthCode()
        {
            int randomBase = RandomNumberGenerator.GetInt32(1000, 9999);

            var chars = "ABCDEFGHJKMNPQRSTUVWXYZ";
            int index = RandomNumberGenerator.GetInt32(0, chars.Length - 1);
            string trailingChar = chars.Substring(index, 1);

            return $"0{randomBase}{trailingChar}";
        }
    }
}
