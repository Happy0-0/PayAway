using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v0;

namespace PayAway.WebAPI.Controllers.v0
{
    /// <summary>
    /// This is v0 of the MerchantController.
    /// </summary>
    /// <remarks>
    /// This version is for the front-end team to have data to develop on until further development.
    /// </remarks>
    [Route("api/[controller]/v0")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        // demo ids
        static Guid merchant_1_id = new Guid(@"f8c6f5b6-533e-455f-87a1-ced552898e1d");
        static Guid order_1_id = new Guid(@"43e351fe-3cbc-4e36-b94a-9befe28637b3");
        static Guid merchant_1_logo_id = new Guid(@"4670e0dc-0335-4370-a3b1-24d9fa1dfdbf");

        /// <summary>
        /// Gets the active merchant
        /// </summary>
        /// <returns>active merchant and order queue</returns>
        [HttpGet()]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ActiveMerchantMBE), StatusCodes.Status200OK)]
        public ActionResult<ActiveMerchantMBE> GetActiveMerchant()
        {
            return Ok(new ActiveMerchantMBE
            {
                MerchantID = merchant_1_id,
                MerchantName = @"Domino's Pizza",
                LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_1_logo_id}.png",
                CatalogItems = new List<CatalogItemMBE>
                {
                    new CatalogItemMBE
                    {
                        ItemID = Guid.NewGuid(),
                        ItemName = "Product/Service 1",
                        ItemUnitPrice = 10.51M
                    },
                    new CatalogItemMBE
                    {
                        ItemID = Guid.NewGuid(),
                        ItemName = "Product/Service 2",
                        ItemUnitPrice = 20.52M
                    },
                    new CatalogItemMBE
                    {
                        ItemID = Guid.NewGuid(),
                        ItemName = "Product/Service 3",
                        ItemUnitPrice = 15.92M
                    }
                }
            });
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
            return Ok(new List<OrderHeaderMBE>()
                {
                    new OrderHeaderMBE
                    {
                        OrderID = order_1_id,
                        OrderNumber = "Order 2",
                        CustomerName = "Joe Smith",
                        PhoneNumber = "(555) 555-5555",
                        Status = "SMS Sent",
                        Total = 30.00M,
                        OrderDate = new DateTime(2020,11,10,15,00,00)
                    },
                    new OrderHeaderMBE
                    {
                        OrderID = order_1_id,
                        OrderNumber = "Order 1",
                        CustomerName = "Joanna Smith",
                        PhoneNumber = "(444) 444-4444",
                        Status = "Paid",
                        Total = 10.00M,
                        OrderDate = new DateTime(2020,11,10,12,00,00)
                    }
                });
        }

        /// <summary>
        /// Gets merchant order
        /// </summary>
        /// <param name="orderID">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <returns>merchant Order</returns>
        [HttpGet("orders/{orderID:Guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantOrderMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantOrderMBE> GetMerchantOrder(Guid orderID)
        {
            if (orderID != order_1_id)
            {
                return NotFound($"Merchant order: [{orderID}] not found");
            }

            return Ok(new MerchantOrderMBE 
            {
                OrderID = order_1_id,
                OrderNumber = "1234",
                MerchantID = merchant_1_id,
                Name = "Joe Smith",
                PhoneNumber = "(333) 333-3333",
                Status = "Paid",
                OrderItems = new List<CatalogItemMBE>
                {
                    new CatalogItemMBE
                    {
                        ItemID = Guid.NewGuid()
                        
                    },
                    new CatalogItemMBE
                    {
                        ItemID = Guid.NewGuid()
                        
                    }
                },
                OrderEvents = new List<OrderEventsMBE>
                {
                    new OrderEventsMBE
                    {
                        EventDate = new DateTime(20,11,11,08,00,00),
                        EventStatus = "Paid",
                        EventDescription = "Payment has been recieved for order."
                    },
                     new OrderEventsMBE
                    {
                        EventDate = new DateTime(20,11,11,07,59,00),
                        EventStatus = "SMS Sent",
                        EventDescription = "SMS Sent to Customer: (333) 333-3333"
                    },
                      new OrderEventsMBE
                    {
                        EventDate = new DateTime(20,11,11,07,58,00),
                        EventStatus = "New Order",
                        EventDescription = "A new order has been created."
                    }
                }
            });
        }

        /// <summary>
        /// Creates a new merchant order
        /// </summary>
        /// <param name="newMerchantOrder"></param>
        /// <returns></returns>
        [HttpPost("orders")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantOrderMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<MerchantOrderMBE> CreateMerchantOrder(NewMerchantOrderMBE newMerchantOrder)
        {
            var merchantOrder = new MerchantOrderMBE
            {
                OrderID = order_1_id,
                OrderNumber = "1234",
                MerchantID = merchant_1_id,
                Name = "Joe Smith",
                PhoneNumber = "(333) 333-3333",
                Status = "New",
                OrderItems = new List<CatalogItemMBE>
                {
                    new CatalogItemMBE
                    {
                        ItemID = Guid.NewGuid()
                    },
                    new CatalogItemMBE
                    {
                        ItemID = Guid.NewGuid()
                    }
                },
                OrderEvents = new List<OrderEventsMBE>
                {
                      new OrderEventsMBE
                    {
                        EventDate = new DateTime(20,11,11,07,58,00),
                        EventStatus = "New Order",
                        EventDescription = "A new order has been created."
                    }
                }
            };
            
            return CreatedAtAction(nameof(GetMerchantOrder), new { orderID = merchantOrder.OrderID }, merchantOrder);
        }

        /// <summary>
        /// Updates a merchant order by merchant ID.
        /// </summary>
        /// <param name="orderID">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <param name="newMerchantOrder"></param>
        /// <returns>updated merchant order</returns>
        [HttpPut("orders/{orderID:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateMerchantOrder(Guid orderID, NewMerchantOrderMBE newMerchantOrder)
        {
            if (orderID != order_1_id)
            {
                return NotFound($"Merchant order with ID: {orderID} not found");
            }

            return NoContent();
        }

        /// <summary>
        /// Sends merchant order
        /// </summary>
        /// <param name="orderID">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <returns></returns>
        [HttpPost("orders/{orderID:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult SendOrder(Guid orderID)
        {
            if (orderID != order_1_id)
            {
                return NotFound($"Merchant order with ID: {orderID} not found");
            }
            return NoContent();
        }

    }
}
