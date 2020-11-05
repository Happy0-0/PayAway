using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

using PayAway.WebAPI.Entities.v0;

namespace PayAway.WebAPI.Controllers.v0
{
    /// <summary>
    /// This is v0 for the Demo Controller.
    /// </summary>
    /// <remarks>
    /// This version is for the front-end team to have data to develop on until further development.
    /// </remarks>
    [Route("api/[controller]/v0")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        // demo ids
        static Guid merchant_1_id = new Guid(@"f8c6f5b6-533e-455f-87a1-ced552898e1d");
        static Guid merchant_1_logo_id = new Guid(@"4670e0dc-0335-4370-a3b1-24d9fa1dfdbf");
        static Guid merchant_1_customer_1_id = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21");
        static Guid merchant_1_customer_2_id = new Guid("8b9b276a-cf81-47bf-97dc-3977cd464787");
        static Guid merchant_2_id = new Guid(@"5d590431-95d2-4f8a-b2d9-6eb4d8cabc89");
        static Guid merchant_2_logo_id = new Guid(@"062c5897-208a-486a-8c6a-76707b9c07eb");

        #region === Overall Demo Methods ================================

        /// <summary>
        /// Resets Database
        /// </summary>
        /// <param name="isPreloadEnabled">Optionally preloads sample data</param>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult ResetDatabase(bool isPreloadEnabled)
        {
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
        public ActionResult<List<MerchantMBE>> GetAllMerchants()
        {
            return Ok( new List<MerchantMBE>
            {
                new MerchantMBE
                {
                    MerchantID = merchant_1_id,
                    MerchantName = @"Domino's Pizza",
                    LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_1_logo_id}.png",
                    IsSupportsTips = true,
                    IsActive = true
                },
                new MerchantMBE
                {
                    MerchantID = merchant_2_id,
                    MerchantName = @"Raising Cane's",
                    LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_2_logo_id}.png",
                    IsSupportsTips = true,
                    IsActive = false
                }
            });
        }

        /// <summary>
        /// Gets merchant information and associated customers using a GUID.
        /// </summary>
        /// <param name="merchantID">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns></returns>
        /// <remarks>Requires a merchantID</remarks>
        [HttpGet("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantMBE> GetMerchant(Guid merchantID)       
        {
            if(merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }

            return Ok(new MerchantMBE
            {
                MerchantID = merchant_1_id,
                MerchantName = @"Domino's Pizza",
                LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_1_logo_id}.png",
                IsSupportsTips = true,
                IsActive = true,
                Customers = new List<CustomerMBE>()
                {
                    new CustomerMBE
                    {
                        CustomerID = merchant_1_customer_1_id,
                        CustomerName = @"Joe Smith",
                        CustomerPhoneNo = @"(513) 456-7890"
                    },
                    new CustomerMBE
                    {
                        CustomerID = merchant_1_customer_2_id,
                        CustomerName = @"Jane Doe",
                        CustomerPhoneNo = @"(513) 555-1212"
                    }
                }
            });
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
            var merchant = new MerchantMBE{ 
                MerchantID = merchant_1_id,
                MerchantName = newMerchant.MerchantName,
                IsSupportsTips = newMerchant.IsSupportsTips
            };

            return CreatedAtAction(nameof(GetMerchant), new { merchantID = merchant.MerchantID}, merchant);
        }

        /// <summary>
        /// Updates merchants using merchantID
        /// </summary>
        /// <param name="merchantID">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="newMerchant"></param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateMerchant(Guid merchantID, NewMerchantMBE merchant)
        {
            if (merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes merchant by merchantID
        /// </summary>
        /// <param name="merchantID">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns></returns>
        [HttpDelete("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NewMerchantMBE), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteMerchantByID(Guid merchantID)
        {
            if (merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }

            return NoContent();
        }
        /// <summary>
        /// Makes selected merchant active and all other merchants inactive.
        /// </summary>
        /// <param name="merchantID">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns></returns>
        [HttpPost("merchants/{merchantID:guid}/setactive")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NewMerchantMBE), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantMBE> MakeMerchantActive(Guid merchantID)
        {
            if (merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            var activeMerchant = new MerchantMBE
            {
                MerchantID = merchant_1_id,
                MerchantName = @"Domino's Pizza",
                LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_1_logo_id}.png",
                IsActive = true,
            };

            return NoContent();            

        }


        #endregion

        #region === Customer Methods ================================
        /// <summary>
        /// Gets list of all customers that belong to a specific merchant
        /// </summary>
        /// <param name="merchantID">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns>list of customers</returns>
        [HttpGet("merchants/{merchantID:guid}/customers")]
        [ProducesResponseType(typeof(List<CustomerMBE>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<CustomerMBE>> GetCustomers(Guid merchantID)
        {
            if (merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }

            return new List<CustomerMBE>
            {
                new CustomerMBE
                {
                    CustomerID = merchant_1_customer_1_id,
                    CustomerName = "Joe Smith",
                    CustomerPhoneNo = "(513) 456-7890"
                },
                new CustomerMBE
                {
                    CustomerID = merchant_1_customer_2_id,
                    CustomerName = @"Jane Doe",
                    CustomerPhoneNo = @"(513) 555-1212"
                }
            };
        }

        /// <summary>
        /// Gets a specific customer by merchantID and customerID
        /// </summary>
        /// <param name="merchantID">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="customerID">5056ce22-50fb-4f1e-bb84-60fb45e21c21</param>
        /// <returns>customer</returns>
        [HttpGet("merchants/{merchantID:guid}/customers/{customerID:guid}")]
        [ProducesResponseType(typeof(CustomerMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerMBE> GetCustomer(Guid merchantID, Guid customerID)
        {
            if (merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            else if (customerID != merchant_1_customer_1_id)
            {
                return NotFound($"Customer with ID: {customerID} on Merchant with ID: {merchantID} not found");
            }

            return new CustomerMBE
            {
                CustomerID = merchant_1_customer_1_id,
                CustomerName = "Joe Smith",
                CustomerPhoneNo = "(513) 456-7890"
            };
            
        }

        /// <summary>
        /// Adds a new customer
        /// </summary>
        /// <param name="merchantID">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="newCustomer"></param>
        /// <returns>new customer</returns>
        [HttpPost("merchants/{merchantID:guid}/customers")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<CustomerMBE> AddCustomer(Guid merchantID, NewCustomerMBE newCustomer)
        {
            if (merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }

            var customer = new CustomerMBE
            {
                CustomerID = merchant_1_customer_1_id,
                CustomerName = newCustomer.CustomerName,
                CustomerPhoneNo = newCustomer.CustomerPhoneNo
            };

            return CreatedAtAction(nameof(GetCustomer), new { merchantID = merchantID, customerID = customer.CustomerID}, customer);
        }

        /// <summary>
        /// Updates a customer
        /// </summary>
        /// <param name="merchantID">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="customerID">for testing use: 5056ce22-50fb-4f1e-bb84-60fb45e21c21</param>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantID:guid}/customers/{customerID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateCustomer(Guid merchantID, Guid customerID, CustomerMBE customer)
        {
            if (merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            else if (customerID != merchant_1_customer_1_id)
            {
                return NotFound($"Customer with ID: {customerID} on Merchant with ID: {merchantID} not found");
            }
            else if (customerID != customer.CustomerID)
            {
                return BadRequest(new ArgumentException(nameof(customer.CustomerID), @"The customerID in the request body did not match the one in the url"));
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a customer on a merchant
        /// </summary>
        /// <param name="merchantID">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="customerID">for testing use: 5056ce22-50fb-4f1e-bb84-60fb45e21c21</param>
        [HttpDelete("merchants/{merchantID:guid}/customers/{customerID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCustomerByID(Guid merchantID, Guid customerID)
        {
            if (merchantID != merchant_1_id)
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            else if (customerID != merchant_1_customer_1_id)
            {
                return NotFound($"Customer with ID: {customerID} on Merchant with ID: {merchantID} not found");
            }

            return NoContent();
        }

        #endregion

    }
}
