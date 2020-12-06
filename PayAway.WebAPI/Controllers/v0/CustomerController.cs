using System;
using System.Collections.Generic;
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
        public ActionResult<CustomerOrderMBE> GetCustomerOrder([FromRoute] Guid orderGuid)
        {
            if (orderGuid != GeneralConstants.ORDER_1_GUID)
            {
                return NotFound($"Customer order: [{orderGuid}] not found");
            }

            return Ok(new CustomerOrderMBE
            {
                OrderGuid = orderGuid,
                MerchantName = @"Domino's Pizza",
                IsSupportsTips = true,
                LogoUrl = HttpHelpers.BuildFullURL(this.Request, GeneralConstants.MERCHANT_1_LOGO_FILENAME),
                CustomerName = "Joe Smith",
                CustomerPhoneNo = "(666) 666-6666",
                OrderTotal = 15.46M,
                OrderDateTimeUTC = DateTime.UtcNow,
                IsPaymentAvailable = true
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
        public ActionResult SubmitOrderPayment([FromRoute] Guid orderGuid, PaymentInfoMBE paymentInfo)
        {
            #region === Validation =====================
            if (orderGuid != GeneralConstants.ORDER_1_GUID)
            {
                return NotFound($"Merchant order with ID: {orderGuid} not found");
            }
            if (paymentInfo.ExpYear > (DateTime.UtcNow.Year) && paymentInfo.ExpYear < (DateTime.UtcNow.Year + 10))
            {
                return BadRequest($"Payment info with expiration year: {paymentInfo.ExpYear} is not valid. ");
            }
            if (paymentInfo.ExpMonth < 12 && paymentInfo.ExpMonth > 1)
            {
                return BadRequest($"Payment info with expiration month: {paymentInfo.ExpMonth} is not valid. ");
            }
            if ((paymentInfo.ExpMonth > DateTime.UtcNow.Month && paymentInfo.ExpYear >= DateTime.UtcNow.Year))
            {
                return BadRequest($"Payment info with expiration month and year: {paymentInfo.ExpMonth} / {paymentInfo.ExpYear} is not valid. ");
            }
            if (paymentInfo.TipAmount < 0)
            {
                return BadRequest($"Your tip amount cannot be less then zero.");
            }
            if (String.IsNullOrEmpty(paymentInfo.PAN))
            {
                return BadRequest($"Your Credit card number cannot be empty. ");
            }
            #endregion

            _messageHub.Clients.All.SendAsync("ReceiveMessage", "Server", $"Order: [{orderGuid}] updated");

            return NoContent();
        }
    }
}