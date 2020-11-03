using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Entities.v1;

namespace PayAway.WebAPI.Controllers.v1
{
    /// <summary>
    /// This is v1 for the Demo Controller.
    /// </summary>
    /// <remarks>
    /// This is a fully functional implementation.
    /// </remarks>
    [Route("api/[controller]/v1")]
    [ApiController]
    public class DemoController : Controller
    {

        #region === Merchant Methods ================================

        /// <summary>
        /// Gets all merchants
        /// </summary>
        /// <returns>all merchants</returns>
        [HttpGet("merchants")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<MerchantMBE>> GetAllMerchants()
        {
            // query the DB
            var dbMerchants = SQLiteDBContext.GetAllMerchants();

            // if no results from DB, return an empty list
            if (dbMerchants == null)
            {
                return Ok(new List<MerchantMBE>());
            }

            // convert DB entities to the public entity types
            var merchants = dbMerchants.ConvertAll(dbM => (MerchantMBE)dbM);

            // return the response
            return Ok(merchants);
        }

        /// <summary>
        /// Gets merchant information and associated customers using a GUID.
        /// </summary>
        /// <param name="merchantID">the unique, system assigned identifier for this merchant</param>
        /// <returns></returns>
        /// <remarks>Requires a merchantID</remarks>
        [HttpGet("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantMBE> GetMerchant(Guid merchantID)
        {
            // query the DB
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantID);;

            // if we did not find a matching merchant
            if(dbMerchant == null)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }

            // convert DB entity to the public entity type
            var merchant = (MerchantMBE)dbMerchant;

            // query for the associated customers (child collection)
            var dbCustomers = SQLiteDBContext.GetCustomers(merchantID);

            // create an empty working object
            var customers = new List<CustomerMBE>();

            // optionally convert DB entities to the public entity type
            if (dbCustomers != null)
            {
                // convert DB entities to the public entity types
                customers = dbCustomers.ConvertAll(dbC => (CustomerMBE)dbC);
            }

            // set the value of the property collection on the parent object
            merchant.Customers = customers;

            // return the response
            return Ok(merchant);
        }

        /// <summary>
        /// Adds a new merchant
        /// </summary>
        /// <param name="newMerchant"></param>
        /// <returns>newMerchant</returns>
        [HttpPost("merchants")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<MerchantMBE> SetupNewMerchant(NewMerchantMBE newMerchant)
        {
            // validate request data
            if(string.IsNullOrEmpty(newMerchant.MerchantName))
            {
                return BadRequest(new ArgumentNullException(nameof(newMerchant.MerchantName), @"You must supply a non blank value for the Merchant Name."));
            }

            try
            {
                // store the new merchant
                var dbMerchant = SQLiteDBContext.InsertMerchant(newMerchant);

                // convert DB entity to the public entity type
                var merchant = (MerchantMBE)dbMerchant;

                // return the response
                return CreatedAtAction(nameof(GetMerchant), new { merchantID = merchant.MerchantID }, merchant);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        #endregion


    }
}
