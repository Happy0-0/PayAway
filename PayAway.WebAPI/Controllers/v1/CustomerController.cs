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
        private readonly SQLiteDBContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="messageHub">The message hub.</param>
        public CustomerController(SQLiteDBContext dbContext, IHubContext<MessageHub> messageHub)
        {
            _dbContext = dbContext;
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
            var dbCustomerOrder = _dbContext.GetOrderExploded(orderGuid);

            //if we do not find a matching order
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
        /// <param name="paymentInfo">payment information of the customer</param>
        /// <returns></returns>
        [HttpPost("orders/{orderGuid:Guid}/sendOrderPayment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult SendOrderPayment([FromRoute] Guid orderGuid, [FromBody] PaymentInfoMBE paymentInfo)
        {
            //query the db
            var dbOrderExploded = _dbContext.GetOrderExploded(orderGuid);

            #region === Validation =====================
            //Biz Logic: check to see if the order guid is correct
            if (dbOrderExploded == null)
            {
                return NotFound($"Customer order with ID: {orderGuid} not found");
            }
            //Biz Logic: validate exp year
            if (paymentInfo.ExpYear < (DateTime.UtcNow.Year) && paymentInfo.ExpYear > (DateTime.UtcNow.Year + 10))
            {
                return BadRequest($"Payment info with expiration year: {paymentInfo.ExpYear} is not valid. ");
            }
            //Biz Logic: validate exp month
            if (paymentInfo.ExpMonth < 12 && paymentInfo.ExpMonth > 1)
            {
                return BadRequest($"Payment info with expiration month: {paymentInfo.ExpMonth} is not valid. ");
            }
            //Biz Logic: making sure month and year is valid
            if ((paymentInfo.ExpMonth < DateTime.UtcNow.Month && paymentInfo.ExpYear <= DateTime.UtcNow.Year))
            {
                return BadRequest($"Payment info with expiration month and year: {paymentInfo.ExpMonth} / {paymentInfo.ExpYear} is not valid. ");
            }
            //Biz Logic: check to see if tip is less than zero
            if (paymentInfo.TipAmount < 0)
            {
                return BadRequest($"Payment info with tip amount: {paymentInfo.TipAmount} cannot less than zero.");
            }
            //Biz Logic: check to see if credit card number is fine.
            if (String.IsNullOrEmpty(paymentInfo.PAN))
            {
                return BadRequest("Your Credit card number cannot be empty.");
            }
            //check to see if order line items for the order.
            if (dbOrderExploded.OrderLineItems.Count == 0)
            {
                return BadRequest($"There are no order line items present.");
            }
            //check to see if auth code is present
            if (!String.IsNullOrEmpty(dbOrderExploded.AuthCode))
            {
                return BadRequest("Order has already been marked paid.");
            }
            #endregion

            try
            {
                //update the dbOrder with the values we just got
                dbOrderExploded.CreditCardNumber = paymentInfo.PAN;
                dbOrderExploded.ExpMonth = paymentInfo.ExpMonth;
                dbOrderExploded.ExpYear = paymentInfo.ExpYear;
                dbOrderExploded.AuthCode = @"A1234";

                //update order
                dbOrderExploded.Status = Enums.ORDER_STATUS.Paid;
                SQLiteDBContext.UpdateOrder(dbOrderExploded);

                //Write the order payment event
                var dbOrderEvent = new OrderEventDBE()
                {
                    OrderId = dbOrderExploded.OrderId,
                    EventDateTimeUTC = DateTime.UtcNow,
                    OrderStatus = Enums.ORDER_STATUS.Paid,
                    EventDescription = $"Order has been paid."
                };

                //save order event
                _dbContext.InsertOrderEvent(dbOrderEvent);

                // send notification to all connected clients
                _messageHub.Clients.All.SendAsync("ReceiveMessage", "Server", $"Order: [{orderGuid}] updated");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] Failed to send order payment."));
            }

        }
    }
}
