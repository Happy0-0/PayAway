using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Twilio.Rest.Api.V2010.Account;

using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.Config;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Entities.v1;
using PayAway.WebAPI.Interfaces;
using PayAway.WebAPI.BizTier;
using PayAway.WebAPI.Utilities;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Web;

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
        /// Gets the active demo merchant and the available catalog items
        /// </summary>
        /// <returns>active merchant and order queue</returns>
        [HttpGet()]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ActiveMerchantMBE), StatusCodes.Status200OK)]
        public ActionResult<ActiveMerchantMBE> GetActiveMerchant()
        {
            //Query the db for the active merchant
            var dbMerchant = SQLiteDBContext.GetActiveMerchant();

            if (dbMerchant == null)
            {
                return NotFound($"Active merchant not found");
            }

            //query the db for catalogue Items
            // 1st: look for unique catalog items for this merchant
            var dbCatalogueItems = SQLiteDBContext.GetCatalogItems(dbMerchant.MerchantId);

            // 2nd: if the merchant did not have unique catalog items, use the default ones on merchantId = 0
            if (dbCatalogueItems == null || dbCatalogueItems.Count == 0)
            {
                dbCatalogueItems = SQLiteDBContext.GetCatalogItems(0);
            }

            // query the db for demo customers
            var dbDemoCustomers = SQLiteDBContext.GetDemoCustomers(dbMerchant.MerchantId);

            // build the return object
            var activeMerchant = new ActiveMerchantMBE
            {
                MerchantGuid = dbMerchant.MerchantGuid,
                MerchantName = dbMerchant.MerchantName,
                LogoUrl = HttpHelpers.BuildFullURL(this.Request, dbMerchant.LogoFileName),
                CatalogItems = dbCatalogueItems.ConvertAll(dbCI => (CatalogItemMBE)dbCI),
                DemoCustomers = dbDemoCustomers.ConvertAll(dbDC => (DemoCustomerMBE)dbDC)
            };

            // return the response,
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
        /// Get a specific merchant order
        /// </summary>
        /// <param name="orderGuid">The unique identifier for the order</param>
        /// <returns>merchant Order</returns>
        [HttpGet("orders/{orderGuid:Guid}", Name = nameof(GetOrder))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<OrderMBE> GetOrder([FromRoute] Guid orderGuid)
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

            #region === Validation =================================================
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
            (bool isValidPhoneNo, string formatedPhoneNo, string normalizedPhoneNo) = Utilities.PhoneNoHelpers.NormalizePhoneNo(newOrder.CustomerPhoneNo);
            if (!isValidPhoneNo)
            {
                return BadRequest(new ArgumentNullException(nameof(newOrder.CustomerPhoneNo), $"[{newOrder.CustomerPhoneNo}] is NOT a supported Phone No format."));
            }
            else
            {
                newOrder.CustomerPhoneNo = formatedPhoneNo;
            }
            #endregion

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
        /// Updates a merchant order.
        /// </summary>
        /// <param name="orderGuid"></param>
        /// <param name="updatedOrder"></param>
        /// <returns>updated merchant order</returns>
        /// <remarks>This method is NOT avaialble if the order status is SMS_Sent or PAID</remarks>
        [HttpPut("orders/{orderGuid:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateOrder([FromRoute] Guid orderGuid, [FromBody] NewOrderMBE updatedOrder)
        {
            // get the existing order
            var dbOrder = SQLiteDBContext.GetOrder(orderGuid);

            // if we did not find a matching order
            if (dbOrder == null)
            {
                return BadRequest(new ArgumentException(nameof(orderGuid), $"OrderGuid: [{orderGuid}] not found"));
            }

            // Biz Logic: Cannot change the order if it has already been paid for.
            if (dbOrder.Status == Enums.ORDER_STATUS.SMS_Sent || dbOrder.Status == Enums.ORDER_STATUS.Paid)
            {
                return BadRequest(new ArgumentException($"Changes are not allowed after the SMS has been sent or the order has been paid", nameof(orderGuid)));
            }

            #region === Validation =================================================
            // validate the input params
            if (string.IsNullOrEmpty(updatedOrder.CustomerName))
            {
                return BadRequest(new ArgumentException(nameof(updatedOrder.CustomerName), @"The order name cannot be blank."));
            }
            else if (string.IsNullOrEmpty(updatedOrder.CustomerPhoneNo))
            {
                return BadRequest(new ArgumentException(nameof(updatedOrder.CustomerPhoneNo), @"The order phone number cannot be blank."));
            }
            (bool isValidPhoneNo, string formatedPhoneNo, _) = PhoneNoHelpers.NormalizePhoneNo(updatedOrder.CustomerPhoneNo);
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

            #endregion

            try
            {
                // update the dbOrder with the values we just got
                dbOrder.CustomerName = updatedOrder.CustomerName;
                dbOrder.PhoneNumber = updatedOrder.CustomerPhoneNo;
                dbOrder.Status = Enums.ORDER_STATUS.Updated;
                SQLiteDBContext.UpdateOrder(dbOrder);

                // in this demo code we are just going to delete and re-add the order line items
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
        /// Sends a payment request to the customer on the order (and all demo customers for this merchant).
        /// </summary>
        /// <param name="orderGuid">the unique id for the order</param>
        /// <returns></returns>
        /// <remarks>This method is NOT avaialble if the order status is PAID</remarks>
        [HttpPost("orders/{orderGuid:Guid}/sendPaymentLink")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult SendOrderPaymentRequest([FromRoute] Guid orderGuid)
        {
            // get the exploded order w/ the line items
            var dbOrderExploded = SQLiteDBContext.GetOrderExploded(orderGuid);

            // if we did not find a matching order
            if (dbOrderExploded == null)
            {
                return NotFound($"OrderGuid: [{orderGuid}] not found");
            }

            // Biz Logic: Cannot change the order if it has already been paid for.
            if (dbOrderExploded.Status == Enums.ORDER_STATUS.Paid)
            {
                return BadRequest(new ArgumentException(nameof(orderGuid), $"You cannot send the Payment link AFTER the order is paid."));
            }

            SendSMSMessageSaveEvent(dbOrderExploded, _webUrlConfig);

            // Step 4:1 Get the merchant's demo customers
            //query db
            var dbDemoCustomers = SQLiteDBContext.GetDemoCustomers(dbOrderExploded.MerchantId);

            // Step 4:2 Loop for each demo customer
            foreach (var demoCustomer in dbDemoCustomers)
            {
                var order = new NewOrderMBE()
                {
                    CustomerName = demoCustomer.CustomerName,
                    CustomerPhoneNo = demoCustomer.CustomerPhoneNo,
                    OrderLineItems = dbOrderExploded.OrderLineItems.ConvertAll(oli => (NewOrderLineItemMBE)oli)
                };
                //save the new order
                var newDbOrder = SQLiteDBContext.InsertOrder(dbOrderExploded.MerchantId, order);
                //get new order exploded for new order guid
                var dbNewOrderExploded = SQLiteDBContext.GetOrderExploded(newDbOrder.OrderGuid);
                //send sms message to demo customer and save new order event.
                SendSMSMessageSaveEvent(dbNewOrderExploded, _webUrlConfig);
            }

            return NoContent();
        }

        #region === Helper Methods =============================================
        private static OrderMBE BuildExplodedOrder(OrderDBE dbExplodedOrder)
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

        public static void SendSMSMessageSaveEvent(OrderDBE dbOrderExploded, WebUrlConfigurationBE webUrlConfig)
        {
            // calc the order total
            decimal orderTotal = (dbOrderExploded.OrderLineItems != null)
                                            ? dbOrderExploded.OrderLineItems.Sum(oli => oli.ItemUnitPrice)
                                            : 0.0M;

            // Step 2: Build the SMS Msg
            string payAwayURL = $"{webUrlConfig.HPPBaseUrl}/customerorder/{dbOrderExploded.OrderGuid}";

            StringBuilder messageBody = new StringBuilder();
            messageBody.AppendLine($"Hello {dbOrderExploded.CustomerName}");

            // we do not know what culture the server is set for so we are explicit, we want to make it formats currency with a US $
            var specificCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            FormattableString formattableString = $"{dbOrderExploded.Merchant.MerchantName} is sending you this link to a secure payment page to enter your payment info for your Order Number: {dbOrderExploded.OrderId:0000} for: {orderTotal:C}";
            messageBody.AppendLine(formattableString.ToString(specificCulture));
            messageBody.AppendLine($"{payAwayURL}");

            // Step 3: Send the SMS msg
            // convert the phone no to the "normalized format"  +15131234567 that the SMS api accepts
            // SendSMSMessage
            (bool isValidPhoneNo, string formattedPhoneNo, string normalizedPhoneNo) = Utilities.PhoneNoHelpers.NormalizePhoneNo(dbOrderExploded.PhoneNumber);
            SMSController.SendSMSMessage(String.Empty, formattedPhoneNo, messageBody.ToString());

            // Step 3.1 Write the SMS event
            var dbOrderEvent = new OrderEventDBE()
            {
                OrderId = dbOrderExploded.OrderId,
                EventDateTimeUTC = DateTime.UtcNow,
                OrderStatus = Enums.ORDER_STATUS.SMS_Sent,
                EventDescription = $"SMS sent to [{normalizedPhoneNo}]."
            };

            //save order event
            SQLiteDBContext.InsertOrderEvent(dbOrderEvent);

            dbOrderExploded.Status = Enums.ORDER_STATUS.SMS_Sent;
            SQLiteDBContext.UpdateOrder(dbOrderExploded);
        }

    #endregion
}
}