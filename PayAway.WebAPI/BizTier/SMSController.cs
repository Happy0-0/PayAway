using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Twilio;
using Twilio.Rest.Api.V2010.Account;

using PayAway.WebAPI.Entities.Config;

namespace PayAway.WebAPI.BizTier
{
    /// <summary>
    /// This class is an abstraction over a SMS WebAPI (currently Twilio)
    /// </summary>
    public static class SMSController
    {
        static SMSServiceConfigBE _config;

        /// <summary>
        /// Extension method to support DI of config info
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void AddSMSServiceConfig(this IServiceCollection services, SMSServiceConfigBE config)
        {
            _config = config;
        }

        /// <summary>
        /// Send a Test SMS Message
        /// </summary>
        /// <param name="toPhoneNumber"></param>
        /// <returns></returns>
        public static string SendTestSMSMessage(string toPhoneNumber)
        {
            // ex: toPhoneNumber  => @"+15134986016"

            return SendSMSMessage(null, toPhoneNumber, $"Test SMS Message.");
        }

        /// <summary>
        /// Send (Push) a SMS (Text) message to a phone number using Twilio
        /// </summary>
        /// <param name="fromPhoneNo"></param>
        /// <param name="toPhoneNumber"></param>
        /// <param name="messageBody"></param>
        /// <returns>The Message SID</returns>
        public static string SendSMSMessage(string fromPhoneNo, string toPhoneNumber, string messageBody)
        {
            if (_config == null)
            {
                throw new ApplicationException($"SMSController DI not configured correctly");
            }

            //(bool isIPReachable, string ipErrMsg) = Utilities.TestTCPPort(@"18.212.47.248", 443); //  18.212.47.248 // api.twilio2.com
            //(bool isDNSReachable, string dnsErrMsg) = Utilities.TestTCPPort(@"api.twilio.com", 443);

            // init the client
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
            var restClient = TwilioClient.GetRestClient();

            try
            {
                // create and send a SMS message
                var message = MessageResource.Create(
                    body: messageBody,
                    from: new Twilio.Types.PhoneNumber(!String.IsNullOrEmpty(fromPhoneNo) ? fromPhoneNo : _config.PhoneNumber),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
                );

                return message.Sid;
            }
            catch (Exception ex)
            {
                //var lastRequest = restClient.HttpClient.LastRequest;
                throw new ApplicationException(ex.ToString());
            }
        }
    }
}
