using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using PayAway.WebAPI.Interfaces;
using PayAway.WebAPI.Entities.v1;
using PayAway.WebAPI.Utilities;
using PayAway.WebAPI.PushNotifications;
using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.Database;
using PayAway.WebAPI.Entities.Config;

using PhoneNumbers;

namespace PayAway.WebAPI.Controllers.v0
{
    /// <summary>
    /// This is v0 of the CustomerController
    /// </summary>
    /// <remarks>
    /// This version is for the front-end team to have data to develop on until the working WebAPI is available
    /// </remarks>
    [Route("api/[controller]/v0")]
    [ApiController]
    public class CustomerController : ControllerBase, ICustomerController
    {
        private readonly IHubContext<MessageHub> _messageHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="messageHub">The message hub.</param>
        public CustomerController(IHubContext<MessageHub> messageHub)
        {
            _messageHub = messageHub;
        }

        /// <summary>
        /// Gets customer orders
        /// </summary>
        /// <param name="orderGuid">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <returns></returns>
        [HttpGet("orders/{orderGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CustomerOrderMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerOrderMBE>> GetCustomerOrder([FromRoute] Guid orderGuid)
        {
            if (orderGuid != GeneralConstants.ORDER_1_GUID)
            {
                return NotFound($"Customer order: [{orderGuid}] not found");
            }

            await Task.Delay(100);

            return Ok(new CustomerOrderMBE
            {
                OrderGuid = orderGuid,
                OrderId = 99,
                OrderStatus = Enums.ORDER_STATUS.SMS_Sent,
                MerchantName = @"Domino's Pizza",
                IsSupportsTips = true,
                LogoUrl = HttpHelpers.BuildFullURL(this.Request, GeneralConstants.MERCHANT_1_LOGO_FILENAME),
                CustomerName = "Joe Smith",
                CustomerPhoneNo = "(666) 666-6666",
                OrderSubTotal = 15.46M,
                OrderDateTimeUTC = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Send Payment information to merchant to be processed.
        /// </summary>
        /// <param name="orderGuid">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <param name="paymentInfo"></param>
        /// <returns></returns>
        [HttpPost("orders/{orderGuid:Guid}/sendOrderPayment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SubmitOrderPayment([FromRoute] Guid orderGuid, PaymentInfoMBE paymentInfo)
        {
            #region === Validation =====================
            if (orderGuid != GeneralConstants.ORDER_1_GUID)
            {
                return NotFound($"Merchant order with ID: {orderGuid} not found");
            }
            // Step: Is it even a valid date (this takes care of wacky month values)
            if (!DateTime.TryParse($"{paymentInfo.ExpMonth}/1/{ paymentInfo.ExpYear}", out DateTime parsedDate))
            {
                return BadRequest($"{paymentInfo.ExpMonth}/{paymentInfo.ExpYear} is not a valid expiration date");
            }

            // Step 2: The expiration date cannot be to far into the future (this takes care of yrs too far into the future)
            if (parsedDate > DateTime.Today.AddYears(5))
            {
                return BadRequest($"{paymentInfo.ExpMonth}/{ paymentInfo.ExpYear} is not a valid expiration date");
            }

            // Step 3: Is the card still valid today (cards are valid thru the last day of the month  (this check prevents dates in the past)
            DateTime calcExpireDate = parsedDate.AddMonths(1).AddDays(-1);
            if (DateTime.Today > calcExpireDate)
            {
                return BadRequest($"Payment Instrument is no longer valid, expired on {calcExpireDate:MM/dd/yyyy}");
            }
            if (paymentInfo.TipAmount < 0)
            {
                return BadRequest($"Your tip amount cannot be less then zero.");
            }
            if (String.IsNullOrEmpty(paymentInfo.PAN))
            {
                return BadRequest($"Your Credit card number cannot be empty. ");
            }
            // == CC Number ===================================
            string paymentPan = paymentInfo.PAN.Trim().CleanUp();
            // // Step 4a: Biz Logi  cc: check to see if credit card number is fine.
            if (paymentPan.Length != 16)
            {
                return BadRequest("Your Credit card number must be 16 digits.");
            }

            if (!paymentPan.CheckLuhn())
            {
                return BadRequest("Your Credit card number must pass a LUN check.");
            }
            #endregion

            await _messageHub.Clients.All.SendAsync("ReceiveMessage", "Server", $"Order: [{orderGuid}] updated");

            return NoContent();
        }
    }
}