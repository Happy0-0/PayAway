using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Entities.v1;
using PayAway.WebAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// <param name="orderGuid">The unique identifier for the merchant</param>
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
            newOrder.Name = newOrder.Name.Trim();

            // validate request data
            if (string.IsNullOrEmpty(newOrder.Name))
            {
                return BadRequest(new ArgumentNullException(nameof(newOrder.Name), @"You must supply a non blank value for the Order Name."));
            }
            else if (string.IsNullOrEmpty(newOrder.PhoneNumber))
            {
                return BadRequest(new ArgumentNullException(nameof(newOrder.PhoneNumber), @"You must supply a non blank value for the Order Phone No."));
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
                    var dbOrderLineItem = new OrderLineItemDBE()
                    {
                        ItemName = orderLineItem.ItemName,
                        ItemUnitPrice = orderLineItem.ItemUnitPrice,
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
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] trying to add merchant order."));
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
        public ActionResult SendOrderPaymentRequest(Guid orderGuid)
        {
            throw new NotImplementedException();
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
            //query the db
            var dbOrder = SQLiteDBContext.GetOrder(orderGuid);

            // if we did not find a matching order
            if (dbOrder == null)
            {
                return BadRequest(new ArgumentException(nameof(orderGuid), $"OrderGuid: [{orderGuid}] not found"));
            }
                        
            try
            {
                
                dbOrder.CustomerName = updatedOrder.Name;
                dbOrder.PhoneNumber = updatedOrder.PhoneNumber;
                SQLiteDBContext.UpdateOrder(dbOrder);

                SQLiteDBContext.DeleteOrderLineItems(dbOrder.OrderId);

                //iterate through orderLineItems collection to save it in the db
                foreach (var orderLineItem in updatedOrder.OrderLineItems)
                {
                    var dbOrderLineItem = new OrderLineItemDBE()
                    {
                        ItemName = orderLineItem.ItemName,
                        ItemUnitPrice = orderLineItem.ItemUnitPrice,
                        OrderId = dbOrder.OrderId,
                        CatalogItemGuid = orderLineItem.ItemGuid
                    };
                    SQLiteDBContext.InsertOrderLineItem(dbOrderLineItem);
                }

                //create the first event
                var dbOrderEvent = new OrderEventDBE()
                {
                    OrderId = dbOrder.OrderId,
                    EventDateTimeUTC = DateTime.UtcNow,
                    OrderStatus = "Order Updated",
                    EventDescription = "The Order has changed."
                };

                //save order event
                SQLiteDBContext.InsertOrderEvent(dbOrderEvent);

                dbOrder.Status = "Order Updated";
                //Update Order again
                SQLiteDBContext.UpdateOrder(dbOrder);





            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] Failed to update merchant order."));
            }


            return NoContent();
            throw new NotImplementedException();
        }
                
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
    }
}