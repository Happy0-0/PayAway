using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Interfaces;

namespace PayAway.WebAPI.Controllers.v0
{
    /// <summary>
    /// This is v0 of the Merchant Controller.
    /// </summary>
    /// <remarks>
    /// This version is for the front-end team to have data to develop on until the working WebAPI is available
    /// </remarks>
    [Route("api/[controller]/v0")]
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
            return Ok(new ActiveMerchantMBE
            {
                MerchantGuid = Constants.MERCHANT_1_GUID,
                MerchantName = @"Domino's Pizza",
                LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{Constants.MERCHANT_1_LOGO_GUID}.png",
                CatalogItems = new List<CatalogItemMBE>
                {
                    new CatalogItemMBE
                    {
                        ItemGuid = Guid.NewGuid(),
                        ItemName = "Product/Service 1",
                        ItemUnitPrice = 10.51M
                    },
                    new CatalogItemMBE
                    {
                        ItemGuid = Guid.NewGuid(),
                        ItemName = "Product/Service 2",
                        ItemUnitPrice = 20.52M
                    },
                    new CatalogItemMBE
                    {
                        ItemGuid = Guid.NewGuid(),
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
                        OrderGuid = Constants.ORDER_1_GUID,
                        OrderNumber = "1",
                        CustomerName = "Joe Smith",
                        PhoneNumber = "(555) 555-5555",
                        Status = "SMS Sent",
                        Total = 30.00M,
                        OrderDate = new DateTime(2020,11,10,15,00,00)
                    },
                    new OrderHeaderMBE
                    {
                        OrderGuid = Constants.ORDER_1_GUID,
                        OrderNumber = "2",
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
        /// <param name="orderGuid">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <returns>merchant Order</returns>
        [HttpGet("orders/{orderGuid:Guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<OrderMBE> GetOrder(Guid orderGuid)
        {
            if (orderGuid != Constants.ORDER_1_GUID)
            {
                return NotFound($"Merchant order: [{orderGuid}] not found");
            }

            return Ok(new OrderMBE
            {
                OrderGuid = orderGuid,
                OrderNumber = "1234",
                MerchantGuid = Constants.MERCHANT_1_GUID,
                Name = "Joe Smith",
                PhoneNumber = "(333) 333-3333",
                Status = "Paid",
                OrderLineItems = new List<CatalogItemMBE>
                {
                    new CatalogItemMBE
                    {
                        ItemGuid = Guid.NewGuid()
                    },
                    new CatalogItemMBE
                    {
                        ItemGuid = Guid.NewGuid()
                    }
                },
                OrderEvents = new List<OrderEventMBE>
                {
                    new OrderEventMBE
                    {
                        EventDate = new DateTime(20,11,11,08,00,00),
                        EventStatus = "Paid",
                        EventDescription = "Payment has been recieved for order."
                    },
                     new OrderEventMBE
                    {
                        EventDate = new DateTime(20,11,11,07,59,00),
                        EventStatus = "SMS Sent",
                        EventDescription = "SMS Sent to Customer: (333) 333-3333"
                    },
                      new OrderEventMBE
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
        [ProducesResponseType(typeof(OrderMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<OrderMBE> CreateMerchantOrder([FromBody] NewOrderMBE newMerchantOrder)
        {
            var merchantOrder = new OrderMBE
            {
                OrderGuid = Constants.ORDER_1_GUID,
                OrderNumber = "1234",
                MerchantGuid = Constants.MERCHANT_1_GUID,
                Name = "Joe Smith",
                PhoneNumber = "(333) 333-3333",
                Status = "New",
                OrderLineItems = new List<CatalogItemMBE>
                {
                    new CatalogItemMBE
                    {
                        ItemGuid = Guid.NewGuid()
                    },
                    new CatalogItemMBE
                    {
                        ItemGuid = Guid.NewGuid()
                    }
                },
                OrderEvents = new List<OrderEventMBE>
                {
                      new OrderEventMBE
                    {
                        EventDate = new DateTime(20,11,11,07,58,00),
                        EventStatus = "New Order",
                        EventDescription = "A new order has been created."
                    }
                }
            };

            return CreatedAtAction(nameof(GetOrder), new { orderID = merchantOrder.OrderGuid }, merchantOrder);
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
            if (orderGuid != Constants.ORDER_1_GUID)
            {
                return NotFound($"Merchant order with ID: {orderGuid} not found");
            }

            return NoContent();
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
            if (orderGuid != Constants.ORDER_1_GUID)
            {
                return NotFound($"Merchant order with ID: {orderGuid} not found");
            }
            return NoContent();
        }

    }
}
