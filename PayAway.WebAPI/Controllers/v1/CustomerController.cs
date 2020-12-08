using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.Config;
using PayAway.WebAPI.Entities.v1;
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
        public async Task<ActionResult<CustomerOrderMBE>> GetCustomerOrder(Guid orderGuid)
        {
            //query the db
            var dbCustomerOrder = await _dbContext.GetOrderExplodedAsync(orderGuid);

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
        public async Task<ActionResult> SubmitOrderPayment([FromRoute] Guid orderGuid, [FromBody] PaymentInfoMBE paymentInfo)
        {
            //query the db
            var dbOrderExploded = await _dbContext.GetOrderExplodedAsync(orderGuid);

            #region === Validation =====================
            //Biz Logic: check to see if the order guid is correct
            if (dbOrderExploded == null)
            {
                return NotFound($"Customer order with ID: {orderGuid} not found");
            }

            // == Expiration Date =====================================
            // Step 2a: Is it even a valid date (this takes care of wacky month values)
            DateTime parsedDate;
            if (!DateTime.TryParse($"{paymentInfo.ExpMonth}/1/{ paymentInfo.ExpYear}", out parsedDate))
            {
                return BadRequest($"{paymentInfo.ExpMonth}/{paymentInfo.ExpYear} is not a valid expiration date");
            }

            // Step 2b: The expiration date cannot be to far into the future (this takes care of yrs too far into the future)
            if (parsedDate > DateTime.Today.AddYears(5))
            {
                return BadRequest($"{paymentInfo.ExpMonth}/{ paymentInfo.ExpYear} is not a valid expiration date");
            }

            // Step 2c: Is the card still valid today (cards are valid thru the last day of the month  (this check prevents dates in the past)
            DateTime calcExpireDate = parsedDate.AddMonths(1).AddDays(-1);
            if (DateTime.Today > calcExpireDate)
            {
                return BadRequest($"Payment Instrument is no longer valid, expired on {calcExpireDate:MM/dd/yyyy}");
            }

            // == Tip Amount ====================================
            // Step 3a: Check to see if the order has a tip even when the merchant doesn't support tips.
            if (dbOrderExploded.Merchant.IsSupportsTips == false 
                    && paymentInfo.TipAmount.HasValue 
                    && paymentInfo.TipAmount.Value != 0.0M)
            {
                return BadRequest($"This merchant does NOT support tips.");
            }

            // Step 3b: Biz Logic: check to see if tip is less than zero
            if (paymentInfo.TipAmount.HasValue
                && paymentInfo.TipAmount.Value < 0.0M)
            {
                return BadRequest($"The tip amount: {paymentInfo.TipAmount} cannot less than $0.00.");
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

            // == Auth Code ===================================
            // check to see if auth code is present
            if (!String.IsNullOrEmpty(dbOrderExploded.AuthCode))
            {
                return BadRequest("Order has already been marked paid.");
            }
            #endregion

            try
            {
                //update the dbOrder with the values we just got
                dbOrderExploded.CreditCardNumber = paymentPan;
                dbOrderExploded.ExpMonth = paymentInfo.ExpMonth;
                dbOrderExploded.ExpYear = paymentInfo.ExpYear;
                dbOrderExploded.AuthCode = CardNetworkHelper.GenerateAuthCode();
                dbOrderExploded.TipAmount = paymentInfo.TipAmount?? 0.9M;

                //update order
                dbOrderExploded.Status = Enums.ORDER_STATUS.Paid;
                await _dbContext.UpdateOrderAsync(dbOrderExploded);

                //Write the order payment event
                var dbOrderEvent = new OrderEventDBE()
                {
                    OrderId = dbOrderExploded.OrderId,
                    EventDateTimeUTC = DateTime.UtcNow,
                    OrderStatus = Enums.ORDER_STATUS.Paid,
                    EventDescription = $"Order has been paid."
                };

                //save order event
                await _dbContext.InsertOrderEventAsync(dbOrderEvent);

                // send notification to all connected clients
                await _messageHub.Clients.All.SendAsync("ReceiveMessage", "Server", $"Order: [{orderGuid}] updated");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] Failed to send order payment."));
            }
        }
    }
}
