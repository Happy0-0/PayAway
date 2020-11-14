using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

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
    public class DemoController : ControllerBase
    {
        #region === Overall Demo Methods ================================
        /// <summary>
        /// Resets Database
        /// </summary>
        /// <param name="isPreloadEnabled">Optionally preloads sample data</param>
        /// <remarks>You can choose to preload a set of default data</remarks>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult ResetDatabase(bool isPreloadEnabled)
        {
           SQLiteDBContext.ResetDB(isPreloadEnabled);

            return NoContent();
        }
        #endregion

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
        /// Gets a merchant and associated demo customers
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <returns></returns>
        [HttpGet("merchants/{merchantGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantMBE> GetMerchant(Guid merchantGuid)
        {
            // query the DB
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);;

            // if we did not find a matching merchant
            if(dbMerchant == null)
            {
                return NotFound($"MerchantGuid: [{merchantGuid}] not found");
            }

            // convert DB entity to the public entity type
            var merchant = (MerchantMBE)dbMerchant;

            // query for the associated customers (child collection)
            var dbCustomers = SQLiteDBContext.GetDemoCustomers(dbMerchant.MerchantId);

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
        /// <param name="newMerchant">object containing information about the new merchant</param>
        /// <returns>newMerchant</returns>
        [HttpPost("merchants")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<MerchantMBE> AddMerchant([FromBody] NewMerchantMBE newMerchant)
        {
            //trims merchant name so that it doesn't have trailing characters
            newMerchant.MerchantName = newMerchant.MerchantName.Trim();

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
                return CreatedAtAction(nameof(GetMerchant), new { merchantGuid = merchant.MerchantGuid }, merchant);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] trying to add merchant: [{newMerchant.MerchantName}]"));
            }
        }

        /// <summary>
        /// Updates a merchant
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <param name="updatedMerchant">object containing updated merchant information</param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateMerchant(Guid merchantGuid, [FromBody] NewMerchantMBE updatedMerchant)
        {
            //trims merchant name so that it doesn't have trailing characters
            updatedMerchant.MerchantName = updatedMerchant.MerchantName.Trim();

            // validate the input params
            if (string.IsNullOrEmpty(updatedMerchant.MerchantName))
            {
                return BadRequest(new ArgumentException(nameof(updatedMerchant.MerchantName), @"The merchant name cannot be blank."));
            }

            // query the DB
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return NotFound($"MerchantGuid: [{merchantGuid}] not found");
            }

            string exisitingDBMerchantName = dbMerchant.MerchantName;

            try 
            {
                // save the updated merchant
                dbMerchant.MerchantName = updatedMerchant.MerchantName;
                dbMerchant.IsSupportsTips = updatedMerchant.IsSupportsTips;

                SQLiteDBContext.UpdateMerchant(dbMerchant);
            }
            catch(Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] trying to update merchant: [{exisitingDBMerchantName}]"));
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a merchant
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <returns></returns>
        [HttpDelete("merchants/{merchantGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NewMerchantMBE), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteMerchant(Guid merchantGuid)
        {
            // query the DB
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            if (dbMerchant == null)
            {
                return NotFound($"MerchantGuid: [{merchantGuid}] is not valid");
            }

            SQLiteDBContext.PurgeMerchant(dbMerchant.MerchantId);

            return NoContent();
        }

        /// <summary>
        /// Makes selected merchant active
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <returns></returns>
        [HttpPost("merchants/{merchantGuid:guid}/setactive")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NewMerchantMBE), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantMBE> SetActiveMerchantForDemo(Guid merchantGuid)
        {
            // query the DB
            var merchantToMakeActive = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (merchantToMakeActive == null)
            {
                return NotFound($"MerchantID: [{merchantGuid}] not found");
            }

            //update merchant in the db
            SQLiteDBContext.SetActiveMerchantForDemo(merchantToMakeActive.MerchantId);

            return NoContent();
        }

        #endregion

        #region === Demo Customer Methods ================================

        /// <summary>
        /// Gets a specific demo customer
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <param name="demoCustomerGuid">The unique identifier for the demo customer.</param>
        /// <returns>a specified customer</returns>
        /// <remarks>A pre-setup demo customer will be will have the same demo experience as the customer entered on the order during the demo</remarks>
        [HttpGet("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CustomerMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerMBE> GetDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid)
        {
            // query the DB
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException(nameof(merchantGuid), $"MerchantGuid: [{merchantGuid}] not found"));
            }

            //query DB for a collection of customers from a specific merchant.
            var dbCustomer = SQLiteDBContext.GetDemoCustomers(dbMerchant.MerchantId)
                                            .Where(dc => dc.DemoCustomerGuid == demoCustomerGuid).FirstOrDefault();

            // if we did not find a matching demo customer
            if (dbCustomer == null)
            {
                return NotFound($"CustomerGuid: [{demoCustomerGuid}] on MerchantGuid: [{merchantGuid}] not found");
            }

            var customer = (CustomerMBE)dbCustomer;

            return Ok(customer);
        }

        /// <summary>
        /// Adds a new demo customer to a merchant
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <param name="newDemoCustomer">Object containing information about the new demo customer</param>
        /// <returns></returns>
        /// <remarks>A pre-setup demo customer will be will have the same demo experience as the customer entered on the order during the demo</remarks>
        [HttpPost("merchants/{merchantGuid:guid}/customers")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CustomerMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<CustomerMBE> AddDemoCustomer(Guid merchantGuid, [FromBody] NewCustomerMBE newDemoCustomer)
        {
            //trims Customer name so that it doesn't have trailing characters
            newDemoCustomer.CustomerName = newDemoCustomer.CustomerName.Trim();

            // validate request data
            if (string.IsNullOrEmpty(newDemoCustomer.CustomerName))
            {
                return BadRequest(new ArgumentNullException(nameof(newDemoCustomer.CustomerName), @"You must supply a non blank value for the Customer Name."));
            }
            else if (string.IsNullOrEmpty(newDemoCustomer.CustomerPhoneNo))
            {
                return BadRequest(new ArgumentNullException(nameof(newDemoCustomer.CustomerPhoneNo), @"You must supply a non blank value for the Customer Phone No."));
            }

            //query the db for the merchant
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException(nameof(merchantGuid), $"MerchantGuid: [{merchantGuid}] not found"));
            }

            try
            {
                //Store the new customer
                var dbCustomer = SQLiteDBContext.InsertDemoCustomer(dbMerchant.MerchantId, newDemoCustomer);

                // convert DB entity to the public entity type
                var customer = (CustomerMBE)dbCustomer;

                // return the response
                return CreatedAtAction(nameof(GetDemoCustomer), new { merchantGuid = merchantGuid, customerGuid = customer.CustomerGuid }, customer);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] trying to add Phone No: [{newDemoCustomer.CustomerPhoneNo}] to merchant: [{merchantGuid}]"));
            }
        }

        /// <summary>
        /// Updates a demo customer on a merchant
        /// </summary>
        /// <param name="merchantGuid">The unique indentifier for the merchant</param>
        /// <param name="demoCustomerGuid">The unique identifier for the demo customer</param>
        /// <param name="updatedDemoCustomer">Object that contains updated information about the demo customer</param>
        /// <returns></returns>
        /// <remarks>A pre-setup demo customer will be will have the same demo experience as the customer entered on the order during the demo</remarks>
        [HttpPut("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid, [FromBody] NewCustomerMBE updatedDemoCustomer)
        {
            // query the db for the merchant
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException(nameof(merchantGuid), $"MerchantGuid: [{merchantGuid}] not found"));
            }

            // trims Customer name so that it doesn't have trailing characters
            updatedDemoCustomer.CustomerName = updatedDemoCustomer.CustomerName.Trim();

            // validate the input params
            if (string.IsNullOrEmpty(updatedDemoCustomer.CustomerName))
            {
                return BadRequest(new ArgumentException(nameof(updatedDemoCustomer.CustomerName), @"The customer name cannot be blank."));
            }

            // get the existing demo customer
            var dbDemoCustomer = SQLiteDBContext.GetDemoCustomers(dbMerchant.MerchantId).Where(c => c.DemoCustomerGuid == demoCustomerGuid).FirstOrDefault();

            if (dbDemoCustomer == null)
            {
                return NotFound($"CustomerID: [{demoCustomerGuid}] on MerchantID: [{merchantGuid}] not found");
            }

            // grab the current phoen no
            string existingCustomerPhoneNo = dbDemoCustomer.CustomerPhoneNo;

            try
            {
                //Save the updated customer
                dbDemoCustomer.CustomerName = updatedDemoCustomer.CustomerName;
                dbDemoCustomer.CustomerPhoneNo = updatedDemoCustomer.CustomerPhoneNo;

                SQLiteDBContext.UpdateDemoCustomer(dbDemoCustomer);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] trying to add Phone No: [{existingCustomerPhoneNo}] to merchant: [{merchantGuid}]"));
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a demo customer on a merchant
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <param name="demoCustomerGuid">The unique identifier for the demo customer</param>
        /// <remarks>A pre-setup demo customer will be will have the same demo experience as the customer entered on the order during the demo</remarks>
        [HttpDelete("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid)
        {
            // query the db for the merchant
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException(nameof(merchantGuid), $"MerchantGuid: [{merchantGuid}] not found"));
            }

            // get the existing demo customer
            var dbDemoCustomer = SQLiteDBContext.GetDemoCustomers(dbMerchant.MerchantId).Where(c => c.DemoCustomerGuid == demoCustomerGuid).FirstOrDefault();

            if (dbDemoCustomer == null)
            {
                return NotFound($"CustomerID: [{demoCustomerGuid}] on MerchantID: [{merchantGuid}] not found");
            }

            SQLiteDBContext.DeleteDemoCustomer(dbDemoCustomer.DemoCustomerId);

            return NoContent();
        }
        #endregion
    }
}
