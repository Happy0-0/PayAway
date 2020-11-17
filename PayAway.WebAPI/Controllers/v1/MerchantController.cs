using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.v0;
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
            var merchant = SQLiteDBContext.GetActiveMerchant();

            if (merchant == null)
            {
                return NotFound($"Active merchant not found");
            }

            //query the db for catalogue Items
            var dbCatalogueItems = SQLiteDBContext.GetCatalogItems(0);

            var activeMerchant = new ActiveMerchantMBE
            {
                MerchantGuid = merchant.MerchantGuid,
                MerchantName = merchant.MerchantName,
                LogoUrl = merchant.LogoUrl,
                CatalogItems = dbCatalogueItems.ConvertAll(dbCI => (CatalogItemMBE)dbCI)
            };

            // return the response
            return Ok(activeMerchant);
        }

        /// <summary>
        /// Gets merchant order
        /// </summary>
        /// <param name="orderGuid">for testing use: 43e351fe-3cbc-4e36-b94a-9befe28637b3</param>
        /// <returns>merchant Order</returns>
        [HttpGet("orders/{orderGuid:Guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantOrderMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantOrderMBE> GetMerchantOrder(Guid orderGuid)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        public ActionResult<MerchantOrderMBE> CreateMerchantOrder([FromBody] NewMerchantOrderMBE newMerchantOrder)
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
        public ActionResult UpdateMerchantOrder(Guid orderGuid, [FromBody] NewMerchantOrderMBE updatedMerchantOrder)
        {
            throw new NotImplementedException();
        }
    }
}
