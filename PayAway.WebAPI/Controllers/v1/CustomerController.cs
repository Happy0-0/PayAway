using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.Config;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Entities.Database;
using PayAway.WebAPI.Interfaces;
using PayAway.WebAPI.BizTier;
using PayAway.WebAPI.Utilities;
using PayAway.WebAPI.PushNotifications;

namespace PayAway.WebAPI.Controllers.v1
{
    /// <summary>
    /// This is v1 of the CustomerController
    /// </summary>
    /// <remarks>
    /// This version is a fully functional version of v0.
    /// </remarks>
    [Route("api/[controller]/v1")]
    [ApiController]
    public class CustomerController : Controller, ICustomerController
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
        /// <param name="orderGuid">unique identifier for the order</param>
        /// <returns></returns>
        [HttpGet("orders/{orderGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CustomerOrderMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerOrderMBE> GetCustomerOrder(Guid orderGuid)
        {
            //query the db
            var dbCustomerOrder = SQLiteDBContext.GetOrderExploded(orderGuid);

            if (dbCustomerOrder == null)
            {
                return NotFound($"Customer order: [{orderGuid}] not found");
            }

            var customerOrder = (CustomerOrderMBE)dbCustomerOrder;

            return Ok(customerOrder);
        }

        /// <summary>
        /// Send Payment information to merchant to be processed.
        /// </summary>
        /// <param name="orderGuid">unique identifier for order</param>
        /// <param name="paymentInfo"></param>
        /// <returns></returns>
        [HttpPost("orders/{orderGuid:Guid}/sendOrderPayment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult SendOrderPayment([FromRoute] Guid orderGuid, [FromBody] PaymentInfoMBE paymentInfo)
        {
            #region === Validation =====================
            if (orderGuid != Constants.ORDER_1_GUID)
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

            // send notification to all connected clients
            _messageHub.Clients.All.SendAsync("ReceiveMessage", "Server", $"Order: [{orderGuid}] updated");

            return NoContent();
        }
    }
}
