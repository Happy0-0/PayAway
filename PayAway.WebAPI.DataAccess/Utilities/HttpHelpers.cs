using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.DataAccess.Utilities
{
    internal class HttpHelpers
    {
        internal static Uri BuildFullURL(HttpRequest httpRequest, string imageFileName)
        {
            var baseUrl = $"{httpRequest.Scheme}://{httpRequest.Host.Value}{httpRequest.PathBase.Value}";
            var fullURL = $"{baseUrl}/{GeneralConstants.LOGO_IMAGES_URI_FOLDER}/{imageFileName}";

            return new Uri(fullURL);
        }
    }
}
