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
        #region === Overall Demo Methods ================================

        /// <summary>
        /// Resets Database
        /// </summary>
        /// <param name="isPreloadEnabled">Optionally preloads sample data</param>
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
        /// Gets merchant information and associated customers using a GUID.
        /// </summary>
        /// <param name="merchantID">The unique identifier of the merchant to retrieve.</param>>
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
                return NotFound($"MerchantID: [{merchantID}] not found");
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
        /// <param name="newMerchant">The new merchant</param>
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
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] trying to add merchant: [{newMerchant.MerchantName}]"));
            }
        }

        /// <summary>
        /// Updates merchants using merchantID
        /// </summary>
        /// <param name="merchantID">The unique identifier of the merchant to update.</param>
        /// <param name="merchant">object containing information about merchants</param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateMerchant(Guid merchantID, NewMerchantMBE merchant)
        {
            // validate the input params
            if (string.IsNullOrEmpty(merchant.MerchantName))
            {
                return BadRequest(new ArgumentException(nameof(merchant.MerchantName), @"The merchant name cannot be blank."));
            }

            // query the DB
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantID);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return NotFound($"MerchantID: [{merchantID}] not found");
            }

            string exisitingDBMerchantName = dbMerchant.MerchantName;

            try 
            {
                // save the updated merchant
                dbMerchant.MerchantName = merchant.MerchantName;
                dbMerchant.IsSupportsTips = merchant.IsSupportsTips;

                SQLiteDBContext.UpdateMerchant(dbMerchant);
            }
            catch(Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] trying to update merchant: [{exisitingDBMerchantName}]"));
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes merchant by merchantID
        /// </summary>
        /// <param name="merchantID">The unique identifier of the merchant to delete.</param>
        /// <returns></returns>
        [HttpDelete("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NewMerchantMBE), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteMerchantByID(Guid merchantID)
        {
            bool isValidMerchant = false;

            try
            {
                //Delete merchant using the merchantID
                isValidMerchant = SQLiteDBContext.DeleteMerchantAndCustomers(merchantID);
            }
            catch (Exception ex)
            {
                // this could be from an invalid MerchantID
                return BadRequest(ex);
            }

            //If the merchant is valid return no content, return not found
            if (isValidMerchant)
            {
                return NoContent();
            }
            else
            {
                return NotFound($"MerchantID : [{merchantID}] is not valid");
            }
        }

        /// <summary>
        /// Makes selected merchant active and all other merchants inactive.
        /// </summary>
        /// <param name="merchantID">Unique identifier for merchant</param>
        /// <returns></returns>
        [HttpPost("merchants/{merchantID:guid}/setactive")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NewMerchantMBE), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantMBE> MakeMerchantActive(Guid merchantID)
        {
            // query the DB
            var merchantToMakeActive = SQLiteDBContext.GetMerchant(merchantID);

            // if we did not find a matching merchant
            if (merchantToMakeActive == null)
            {
                return NotFound($"MerchantID: [{merchantID}] not found");
            }

            //update merchant in the db
            SQLiteDBContext.SetActiveMerchant(merchantID);

            return NoContent();
        }

        #endregion

        #region === Customer Methods ================================

        /// <summary>
        /// Gets a specific customer by merchantID and customerID
        /// </summary>
        /// <param name="merchantID">The unique identifier of the merchant the customer belongs to.</param>
        /// <param name="customerID">The unique identifier for the customer.</param>
        /// <returns>a specified customer</returns>  
        [HttpGet("merchants/{merchantID:guid}/customers/{customerID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CustomerMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerMBE> GetCustomer(Guid merchantID, Guid customerID)
        {
            //query DB for a collection of customers from a specific merchant.
            var dbCustomer = SQLiteDBContext.GetCustomers(merchantID).Where(c => c.CustomerID == customerID).FirstOrDefault();

            if (dbCustomer == null)
            {
                return NotFound($"CustomerID: [{customerID}] on MerchantID: [{merchantID}] not found");
            }

            var customer = (CustomerMBE)dbCustomer;

            return Ok(customer);
        }

        /// <summary>
        /// Adds a new customer to a merchant
        /// </summary>
        /// <param name="merchantID">The unique identifier for the merchant</param>
        /// <param name="newCustomer">object containing information about customers</param>
        /// <returns></returns>
        [HttpPost("merchants/{merchantID:guid}/customers")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CustomerMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<CustomerMBE> AddNewCustomer(Guid merchantID, NewCustomerMBE newCustomer)
        {
            // validate request data
            if (string.IsNullOrEmpty(newCustomer.CustomerName))
            {
                return BadRequest(new ArgumentNullException(nameof(newCustomer.CustomerName), @"You must supply a non blank value for the Customer Name."));
            }
            else if (string.IsNullOrEmpty(newCustomer.CustomerPhoneNo))
            {
                return BadRequest(new ArgumentNullException(nameof(newCustomer.CustomerPhoneNo), @"You must supply a non blank value for the Customer Phone No."));
            }

            //query the db
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantID);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException(nameof(merchantID), $"MerchantID: [{merchantID}] not found"));
            }

            try
            {
                //Store the new customer
                var dbCustomer = SQLiteDBContext.InsertCustomer(merchantID, newCustomer);

                // convert DB entity to the public entity type
                var customer = (CustomerMBE)dbCustomer;

                // return the response
                return CreatedAtAction(nameof(GetCustomer), new { merchantID = merchantID, customerID = customer.CustomerID }, customer);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] trying to add Phone No: [{newCustomer.CustomerPhoneNo}] to merchant: [{merchantID}]"));
            }
        }

        /// <summary>
        /// Updates customer using merchantID and customerID
        /// </summary>
        /// <param name="merchantID">The unique indentifier for the merchant</param>
        /// <param name="customerID">The unique idnentifier for the customer</param>
        /// <param name="customer">object that contains information about the customer</param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantID:guid}/customers/{customerID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateCustomer(Guid merchantID, Guid customerID, NewCustomerMBE customer)
        {
            // validate the input params
            if (string.IsNullOrEmpty(customer.CustomerName))
            {
                return BadRequest(new ArgumentException(nameof(customer.CustomerName), @"The customer name cannot be blank."));
            }

            try
            {
                //Save the updated customer
                var updatedDBCustomer = (CustomerDBE)customer;
                SQLiteDBContext.UpdateCustomer(merchantID, updatedDBCustomer);
            }
            catch (Exception ex)
            {
                // this coudld be from an invalid Customer
                return BadRequest(ex);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a customer on a merchant
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="customerID"></param>
        [HttpDelete("merchants/{merchantID:guid}/customers/{customerID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCustomer(Guid merchantID, Guid customerID)
        {
            bool isValidCustomer = false;

            try
            {
                //Delete cutomser 
                isValidCustomer = SQLiteDBContext.DeleteCustomer(merchantID, customerID);
            }
            catch (Exception ex)
            {
                // this could be from an invalid Customer
                return BadRequest(ex);
            }

            //If the merchant is not valid return no content, return not found
            if (isValidCustomer)
            {
                return NoContent();
            }
            else
            {
                return NotFound($"CustomerID: [{customerID}] on Merchant : [{merchantID}] is not valid");
            }
        }
        #endregion
    }
}
