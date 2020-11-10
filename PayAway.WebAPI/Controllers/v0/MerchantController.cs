using System;
using System.Collections.Generic;
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
                LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_1_logo_id}.png"
            }); ;
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
                        OrderID = Guid.NewGuid(),
                        OrderNumber = "Order 2",
                        CustomerName = "Joe Smith",
                        PhoneNumber = "(555) 555-5555",
                        Status = "SMS Sent",
                        Total = 30.00M,
                        OrderDate = new DateTime(2020,11,10,15,00,00)
                    },
                    new OrderHeaderMBE
                    {
                        OrderID = Guid.NewGuid(),
                        OrderNumber = "Order 1",
                        CustomerName = "Joanna Smith",
                        PhoneNumber = "(444) 444-4444",
                        Status = "Paid",
                        Total = 10.00M,
                        OrderDate = new DateTime(2020,11,10,12,00,00)
                    }
                });
        }




    }
}
