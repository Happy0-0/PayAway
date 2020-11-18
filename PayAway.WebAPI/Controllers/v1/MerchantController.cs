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
        /// Gets merchant order
        /// </summary>
        /// <param name="orderGuid">The unique identifier for the merchant</param>
        /// <returns>merchant Order</returns>
        [HttpGet("orders/{orderGuid:Guid}", Name = nameof(GetMerchantOrder))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<OrderMBE> GetMerchantOrder(Guid orderGuid)
        {
            //query the db
            var dbMerchantOrder = SQLiteDBContext.GetOrderExploded(orderGuid);

            // if we did not find a matching merchant order
            if (dbMerchantOrder == null)
            {
                return NotFound($"Merchant order: [{orderGuid}] not found");
            }

            // convert DB entity to the public entity type
            var merchantOrder = (OrderMBE)dbMerchantOrder;

            merchantOrder.MerchantGuid = dbMerchantOrder.Merchant.MerchantGuid;

            //create an empty working object
            var catalogItems = new List<CatalogItemMBE>();
            var orderEvents = new List<OrderEventMBE>();

            // optionally convert DB entities to the public entity type
            if (dbMerchantOrder.OrderLineItems != null)
            {
                // convert DB entities to the public entity types
                catalogItems = dbMerchantOrder.OrderLineItems.ConvertAll(oli => (CatalogItemMBE)oli);
            }
            if (dbMerchantOrder.OrderEvents != null)
            {
                // convert DB entities to the public entity types
                orderEvents = dbMerchantOrder.OrderEvents.ConvertAll(dbE => (OrderEventMBE)dbE);
            }

            // set the value of the property collection on the parent object
            merchantOrder.OrderLineItems = catalogItems;
            merchantOrder.OrderEvents = orderEvents;

            //Return the response
            return Ok(merchantOrder);

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
        /// Creates a new merchant order
        /// </summary>
        /// <param name="newMerchantOrder"></param>
        /// <returns></returns>
        [HttpPost("orders")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<OrderMBE> CreateMerchantOrder([FromBody] NewOrderMBE newMerchantOrder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a payment request to the customer.
        /// </summary>
        /// <param name="orderGuid">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <returns></returns>
        [HttpPost("orders/{orderGuid:Guid}/sendPaymentLink")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult SendOrderPaymentRequest(Guid orderGuid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates a merchant order by merchant ID.
        /// </summary>
        /// <param name="orderGuid">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <param name="updatedMerchantOrder"></param>
        /// <returns>updated merchant order</returns>
        [HttpPut("orders/{orderGuid:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateMerchantOrder(Guid orderGuid, [FromBody] NewOrderMBE updatedMerchantOrder)
        {
            throw new NotImplementedException();
        }
    }
}
