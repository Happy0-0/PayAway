using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using PayAway.WebAPI.Interfaces;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Utilities;
using PayAway.WebAPI.PushNotifications;

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
    public class CustomerController : ControllerBase
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


        [HttpGet("orders/{orderGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerOrderMBE> GetCustomerOrder([FromRoute] Guid orderGuid)
        {
            if (orderGuid != Constants.ORDER_1_GUID)
            {
                return NotFound($"Customer order: [{orderGuid}] not found");
            }
            
            return Ok(new CustomerOrderMBE
            {
                OrderGuid = orderGuid,
                MerchantName = @"Domino's Pizza",
                IsSupportsTips = true,
                LogoUrl = HttpHelpers.BuildFullURL(this.Request, Constants.MERCHANT_1_LOGO_FILENAME),
                CustomerName = "Joe Smith",
                CustomerPhoneNo = "(666) 666-6666",
                OrderTotal = 15.46M,
                OrderDateTimeUTC = DateTime.UtcNow,
                IsPaymentAvailable = true
            });
        }

        [HttpPost("orders/{orderGuid:Guid}/sendOrderPayment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<PaymentInfoMBE> SendOrderPayment([FromRoute] Guid orderGuid, [FromBody] PaymentInfoMBE paymentInfo)
        {
            if (orderGuid != Constants.ORDER_1_GUID)
            {
                return NotFound($"Merchant order with ID: {orderGuid} not found");
            }

            if(paymentInfo.ExpYear < DateTime.UtcNow.Year)
            {
                return BadRequest($"Payment info with expiration year: {paymentInfo.ExpYear} is not valid. ");
            }

            _messageHub.Clients.All.SendAsync("OrderUpdated", "foobar");

            return NoContent();
        }
    }
}
