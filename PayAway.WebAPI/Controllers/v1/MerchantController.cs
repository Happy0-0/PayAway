using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.Config;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Entities.v1;
using PayAway.WebAPI.Interfaces;
using System.Text;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Lookups.V1;
using PayAway.WebAPI.BizTier;

namespace PayAway.WebAPI.Controllers.v1
{
    /// <summary>
    /// This is v1 of the MerchantController.
    /// </summary>
    /// <remarks>
    /// This version is a fully functional version of v0.
    /// </remarks>
    [Route("api/[controller]/v1")]
    [ApiController]
    public class MerchantController : ControllerBase, IMerchantController
    {
        private readonly ILogger<MerchantController> logger;
        private readonly WebUrlConfigurationBE _webUrlConfig;

        /// <summary>Initializes a new instance of the <see cref="T:PrestoPayv3.Server.WebAPI.Controllers.v1.MerchantController" /> class.</summary>
        /// <param name="webUrlConfig">The web URL configuration.</param>
        /// <param name="logger">The logger.</param>
        public MerchantController(WebUrlConfigurationBE webUrlConfig, ILogger<MerchantController> logger)
        {
            this._webUrlConfig = webUrlConfig;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the active merchant
        /// </summary>
        /// <returns>active merchant and order queue</returns>
        [HttpGet()]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ActiveMerchantMBE), StatusCodes.Status200OK)]
        public ActionResult<ActiveMerchantMBE> GetActiveMerchant()
        {
            //Query the db
            var dbMerchant = SQLiteDBContext.GetActiveMerchant();

            if (dbMerchant == null)
            {
                return NotFound($"Active merchant not found");
            }

            //query the db for catalogue Items
            var dbCatalogueItems = SQLiteDBContext.GetCatalogItems(0);

            var activeMerchant = new ActiveMerchantMBE
            {
                MerchantGuid = dbMerchant.MerchantGuid,
                MerchantName = dbMerchant.MerchantName,
                LogoUrl = dbMerchant.LogoUrl,
                CatalogItems = dbCatalogueItems.ConvertAll(dbCI => (CatalogItemMBE)dbCI)
            };

            // return the response
            return Ok(activeMerchant);
        }
               
        /// <summary>
        /// Gets the order queue
        /// </summary>
        /// <returns>order queue</returns>
        [HttpGet("orders")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderQueueMBE), StatusCodes.Status200OK)]
        public ActionResult<OrderQueueMBE> GetOrderQueue()
        {
            //Query the db
            var dbMerchant = SQLiteDBContext.GetActiveMerchant();

            if (dbMerchant == null)
            {
                return NotFound($"Active merchant not found");
            }

            // create the empty return object
            OrderQueueMBE orderQueue = new OrderQueueMBE()
            {
                Orders = new List<OrderHeaderMBE>()
            };

            // get the list of current orders
            var dbOrders = SQLiteDBContext.GetOrders(dbMerchant.MerchantId);

            if(dbOrders != null && dbOrders.Count > 0)
            {
                foreach(var dbOrder in dbOrders)
                {
                    // get the exploded order w/ the line items
                    var dbOrderExploded = SQLiteDBContext.GetOrderExploded(dbOrder.OrderGuid);

                    // convert to the MBE 
                    var orderHeader = (OrderHeaderMBE)dbOrderExploded;

                    // add the l/i subtotal
                    orderHeader.Total = (dbOrderExploded.OrderLineItems != null) 
                                            ? dbOrderExploded.OrderLineItems.Sum(oli => oli.ItemUnitPrice) 
                                            : 0.0M;

                    // add to the results collection
                    orderQueue.Orders.Add(orderHeader);
                }
            }

            return Ok(orderQueue);
        }

        /// <summary>
        /// Gets merchant order
        /// </summary>
        /// <param name="orderGuid">The unique identifier for the order</param>
        /// <returns>merchant Order</returns>
        [HttpGet("orders/{orderGuid:Guid}", Name = nameof(GetOrder))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<OrderMBE> GetOrder(Guid orderGuid)
        {
            //query the db
            var dbExplodedOrder = SQLiteDBContext.GetOrderExploded(orderGuid);

            // if we did not find a matching merchant order
            if (dbExplodedOrder == null)
            {
                return NotFound($"Merchant order: [{orderGuid}] not found");
            }

            // convert this to the public mbe
            var order = BuildExplodedOrder(dbExplodedOrder);

            //Return the response
            return Ok(order);

        }

        /// <summary>
        /// Creates a new merchant order
        /// </summary>
        /// <param name="newOrder">object containing information about the new merchant order</param>
        /// <returns>new Order</returns>
        [HttpPost("orders")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<OrderMBE> CreateOrder([FromBody] NewOrderMBE newOrder)
        {
            //trims customer name so that it doesn't have trailing characters
            newOrder.CustomerName = newOrder.CustomerName.Trim();

            // validate request data
            if (string.IsNullOrEmpty(newOrder.CustomerName))
            {
                return BadRequest(new ArgumentNullException(nameof(newOrder.CustomerName), @"You must supply a non blank value for the Order Name."));
            }
            else if (string.IsNullOrEmpty(newOrder.CustomerPhoneNo))
            {
                return BadRequest(new ArgumentNullException(nameof(newOrder.CustomerPhoneNo), @"You must supply a non blank value for the Order Phone No."));
            }
            foreach (var orderLineItem in newOrder.OrderLineItems)
            {
                var catalogItem = SQLiteDBContext.GetCatalogItem(orderLineItem.ItemGuid);
                if(catalogItem == null)
                {
                    return BadRequest(new ArgumentNullException(nameof(orderLineItem.ItemGuid), $"Error : [{orderLineItem.ItemGuid}] Is not a valid catalog item guid."));
                }
            }
            (bool isValidPhoneNo, string formatedPhoneNo, string normalizedPhoneNo) = Utilities.NormalizePhoneNo(newOrder.CustomerPhoneNo);
            if (!isValidPhoneNo)
            {
                return BadRequest(new ArgumentNullException(nameof(newOrder.CustomerPhoneNo), $"[{newOrder.CustomerPhoneNo}] is NOT a supported Phone No format."));
            }
            else
            {
                newOrder.CustomerPhoneNo = formatedPhoneNo;
            }

            //query the db for the active merchant
            var dbActiveMerchant = SQLiteDBContext.GetActiveMerchant();

            try
            {
                //Store the new merchant Order
                var dbOrder = SQLiteDBContext.InsertOrder(dbActiveMerchant.MerchantId, newOrder);

                //create the first event
                var dbOrderEvent = new OrderEventDBE()
                {
                    OrderId = dbOrder.OrderId,
                    EventDateTimeUTC = DateTime.UtcNow,
                    OrderStatus = Enums.ORDER_STATUS.New,
                    EventDescription = "A new order has been created."
                };

                //save order event
                SQLiteDBContext.InsertOrderEvent(dbOrderEvent);
                
                //iterate through orderLineItems collection to save it in the db
                foreach(var orderLineItem in newOrder.OrderLineItems)
                {
                    var catalogItem = SQLiteDBContext.GetCatalogItem(orderLineItem.ItemGuid);

                    var dbOrderLineItem = new OrderLineItemDBE()
                    {
                        ItemName = catalogItem.ItemName,
                        ItemUnitPrice = catalogItem.ItemUnitPrice,
                        OrderId = dbOrder.OrderId,
                        CatalogItemGuid = orderLineItem.ItemGuid
                    };
                    SQLiteDBContext.InsertOrderLineItem(dbOrderLineItem);
                }

                //convert dbOrder to public entity.
                var dbExplodedOrder = SQLiteDBContext.GetOrderExploded(dbOrder.OrderGuid);
                var explodedOrder = BuildExplodedOrder(dbExplodedOrder);

                // return the response
                return CreatedAtAction(nameof(GetOrder), new { orderGuid = explodedOrder.OrderGuid }, explodedOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] Failed trying to create merchant order."));
            }
        }

        /// <summary>
        /// Updates an order by order guid.
        /// </summary>
        /// <param name="orderGuid"></param>
        /// <param name="updatedOrder"></param>
        /// <returns>updated merchant order</returns>
        [HttpPut("orders/{orderGuid:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateOrder(Guid orderGuid, [FromBody] NewOrderMBE updatedOrder)
        {
            // get the existing order
            var dbOrder = SQLiteDBContext.GetOrder(orderGuid);

            // if we did not find a matching order
            if (dbOrder == null)
            {
                return BadRequest(new ArgumentException(nameof(orderGuid), $"OrderGuid: [{orderGuid}] not found"));
            }

            // Biz Logic: Cannot change the order if it has already been paid for.
            if (dbOrder.Status == Enums.ORDER_STATUS.Paid)
            {
                return BadRequest(new ArgumentException(nameof(orderGuid), $"Order status is Paid. Changes are not allowed."));
            }

            // validate the input params
            if (string.IsNullOrEmpty(updatedOrder.CustomerName))
            {
                return BadRequest(new ArgumentException(nameof(updatedOrder.CustomerName), @"The order name cannot be blank."));
            }
            else if (string.IsNullOrEmpty(updatedOrder.CustomerPhoneNo))
            {
                return BadRequest(new ArgumentException(nameof(updatedOrder.CustomerPhoneNo), @"The order phone number cannot be blank."));
            }
            (bool isValidPhoneNo, string formatedPhoneNo, string normalizedPhoneNo) = Utilities.NormalizePhoneNo(updatedOrder.CustomerPhoneNo);
            if (!isValidPhoneNo)
            {
                return BadRequest(new ArgumentNullException(nameof(updatedOrder.CustomerPhoneNo), $"[{updatedOrder.CustomerPhoneNo}] is NOT a supported Phone No format."));
            }
            else
            {
                updatedOrder.CustomerPhoneNo = formatedPhoneNo;
            }

            // validate the catalog guids
            foreach (var orderLineItem in updatedOrder.OrderLineItems)
            {
                // try to find the catalog item
                var catalogItem = SQLiteDBContext.GetCatalogItem(orderLineItem.ItemGuid);

                // if it did not exist (ie: a invalid guid)
                if (catalogItem == null)
                {
                    return BadRequest(new ArgumentNullException(nameof(orderLineItem.ItemGuid), $"Error : [{orderLineItem.ItemGuid}] Is not a valid catalog item guid."));
                }
            }

            try
            {
                // update the dbOrder with the values we just got
                dbOrder.CustomerName = updatedOrder.CustomerName;
                dbOrder.PhoneNumber = updatedOrder.CustomerPhoneNo;
                dbOrder.Status = Enums.ORDER_STATUS.Updated;
                SQLiteDBContext.UpdateOrder(dbOrder);

                // in this demo code we are just going to delete and readd the order line items
                SQLiteDBContext.DeleteOrderLineItems(dbOrder.OrderId);

                //iterate through orderLineItems collection to save it in the db
                foreach (var orderLineItem in updatedOrder.OrderLineItems)
                {
                    var catalogItem = SQLiteDBContext.GetCatalogItem(orderLineItem.ItemGuid);

                    var dbOrderLineItem = new OrderLineItemDBE()
                    {
                        ItemName = catalogItem.ItemName,
                        ItemUnitPrice = catalogItem.ItemUnitPrice,
                        OrderId = dbOrder.OrderId,
                        CatalogItemGuid = orderLineItem.ItemGuid
                    };

                    SQLiteDBContext.InsertOrderLineItem(dbOrderLineItem);
                }

                // create an event
                var dbOrderEvent = new OrderEventDBE()
                {
                    OrderId = dbOrder.OrderId,
                    EventDateTimeUTC = DateTime.UtcNow,
                    OrderStatus = Enums.ORDER_STATUS.Updated,
                    EventDescription = "Order was updated."
                };

                //save order event
                SQLiteDBContext.InsertOrderEvent(dbOrderEvent);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] Failed to update merchant order."));
            }
        }

        /// <summary>
        /// Sends a payment request to the customer.
        /// </summary>
        /// <param name="orderGuid">the unique id for the order</param>
        /// <returns></returns>
        [HttpPost("orders/{orderGuid:Guid}/sendPaymentLink")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult SendOrderPaymentRequest(Guid orderGuid)
        {
            
            // query the db
            var dbOrder = SQLiteDBContext.GetOrderExploded(orderGuid);

            // if we did not find a matching order
            if (dbOrder == null)
            {
                return BadRequest(new ArgumentException(nameof(orderGuid), $"OrderGuid: [{orderGuid}] not found"));
            }

            // Biz Logic: Cannot change the order if it has already been paid for.
            if (dbOrder.Status == Enums.ORDER_STATUS.Paid)
            {
                return BadRequest(new ArgumentException(nameof(orderGuid), $"Order status is Paid. Changes are not allowed."));
            }

            // get the exploded order w/ the line items
            var dbOrderExploded = SQLiteDBContext.GetOrderExploded(dbOrder.OrderGuid);

            // convert to the MBE 
            var orderHeader = (OrderHeaderMBE)dbOrderExploded;

            // Step 2: Build the SMS Msg
            string payAwayURL = $"{_webUrlConfig.HPPBaseUrl}/customerorder/{dbOrder.OrderGuid}";
            var messageBody = $"Hello {dbOrder.CustomerName}; {dbOrder.Merchant.MerchantName} is sending you this link to a secure payment page to enter your payment info for your Order Number: {dbOrder.OrderId} for: {orderHeader.Total}.";

            // Step 3: Send the SMS msg
            // convert the phone no to the "normalized format"  +15131234567 that the SMS api accepts
            // SendSMSMessage
            var phoneNumber = PhoneNumberResource.Fetch(
            countryCode: "US",
            pathPhoneNumber: new Twilio.Types.PhoneNumber(dbOrder.PhoneNumber));
            var formattedPhoneNumber = phoneNumber.ToString();

            string v = SMSController.SendSMSMessage(String.Empty, formattedPhoneNumber, messageBody);


            // Step 3.1 Write the SMS event

            // create an event
            var dbOrderEvent = new OrderEventDBE()
            {
                OrderId = dbOrder.OrderId,
                EventDateTimeUTC = DateTime.UtcNow,
                OrderStatus = Enums.ORDER_STATUS.SMS_Sent,
                EventDescription = "SMS sent."
            };

            //save order event
            SQLiteDBContext.InsertOrderEvent(dbOrderEvent);

            #region (addl work for next sprint)

            // Step 4:1 Get the merchant's demo customers

            // Step 4:2 Loop for each demo customer
            //  Clone the order setting the customer on the order to be the demo customer
            // normalize the phone no
            //  Send the SMS msg

            #endregion
        }

        #region === Helper Methods =============================================
        private OrderMBE BuildExplodedOrder(OrderDBE dbExplodedOrder)
        {
            // convert DB entity to the public entity type
            var order = (OrderMBE)dbExplodedOrder;

            order.MerchantGuid = dbExplodedOrder.Merchant.MerchantGuid;

            //create an empty working object
            var catalogItems = new List<CatalogItemMBE>();
            var orderEvents = new List<OrderEventMBE>();

            // optionally convert DB entities to the public entity type
            if (dbExplodedOrder.OrderLineItems != null)
            {
                // convert DB entities to the public entity types
                catalogItems = dbExplodedOrder.OrderLineItems.ConvertAll(oli => (CatalogItemMBE)oli);
            }
            if (dbExplodedOrder.OrderEvents != null)
            {
                // convert DB entities to the public entity types
                orderEvents = dbExplodedOrder.OrderEvents.ConvertAll(dbE => (OrderEventMBE)dbE);
            }

            // set the value of the property collection on the parent object
            order.OrderLineItems = catalogItems;
            order.OrderEvents = orderEvents;

            return order;
        }
        #endregion
    }
}