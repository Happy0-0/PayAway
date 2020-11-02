using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
        /// <summary>
        /// Resets Database
        /// </summary>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult ResetDatabase()
        {
            return NoContent();
        }

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
                    MerchantID = new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"),
                    MerchantName = "Dominoes Pizza",
                    LogoUrl = "https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/4670e0dc-0335-4370-a3b1-24d9fa1dfdbf.png",
                    IsSupportsTips = true,
                    IsActive = true
                }
            });


        }

        /// <summary>
        /// Gets merchant information and associated customers using a GUID.
        /// </summary>
        /// <param name="merchantID">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns></returns>
        /// <remarks>Requires a merchantID</remarks>
        [HttpGet("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantMBE> GetMerchant(Guid merchantID)       
        {
            if(merchantID != new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"))
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            return Ok(new MerchantMBE
            {
                MerchantID = new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"),
                MerchantName = "Dominoes Pizza",
                LogoUrl = "https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/4670e0dc-0335-4370-a3b1-24d9fa1dfdbf.png",
                IsSupportsTips = true,
                IsActive = true,
                Customers = new List<CustomerMBE>()
                {
                    new CustomerMBE
                    {
                        CustomerID = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"),
                        CustomerName = "Joe Smith",
                        CustomerPhoneNo = "(513) 456-7890"
                    }
                }
            });

        }

        /// <summary>
        /// Adds a new merchant
        /// </summary>
        /// <param name="newMerchant"></param>
        /// <returns>newMerchant</returns>
        [HttpPost("merchant")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<MerchantMBE> SetupNewMerchant(NewMerchantMBE newMerchant)
        {
            var merchant = new MerchantMBE{ 
                MerchantID = new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"),
                MerchantName = newMerchant.MerchantName,
                LogoUrl = newMerchant.LogoUrl,
                IsSupportsTips = newMerchant.IsSupportsTips,
                IsActive = newMerchant.IsActive
            };
            return CreatedAtAction(nameof(GetMerchant), new { merchantID = merchant.MerchantID}, merchant);
        }
        /// <summary>
        /// Updates merchants using merchantID
        /// </summary>
        /// <param name="merchantID">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="merchant"></param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult UpdateMerchant(Guid merchantID, MerchantMBE merchant)
        {
            if (merchantID != new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"))
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            
            if (merchantID != merchant.MerchantID)
            {
                return BadRequest(new ArgumentException(nameof(merchant.MerchantID), @"The merchantID in the request body did not match the url."));
            }
            return NoContent();

        }

        /// <summary>
        /// Deletes merchant by merchantID
        /// </summary>
        /// <param name="merchantID">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns></returns>
        [HttpDelete("merchants/{merchantID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NewMerchantMBE), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteMerchantByID(Guid merchantID)
        {

            if (merchantID != new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"))
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }

            return NoContent();
        }

        /// <summary>
        /// Gets list of all customers that belong to a specific merchant
        /// </summary>
        /// <param name="merchantID">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns>list of customers</returns>
        [HttpGet("merchants/{merchantID:guid}/customers")]
        [ProducesResponseType(typeof(List<CustomerMBE>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<CustomerMBE>> GetCustomers(Guid merchantID)
        {
            if (merchantID != new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"))
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            return new List<CustomerMBE>
            {
                new CustomerMBE
                {
                    CustomerID = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"),
                    CustomerName = "Joe Smith",
                    CustomerPhoneNo = "(513) 456-7890"
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
            if (merchantID != new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"))
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            if (customerID != new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"))
            {
                return NotFound($"Customer with ID: {customerID} on Merchant with ID: {merchantID} not found");
            }
            return new CustomerMBE
            {
                CustomerID = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"),
                CustomerName = "Joe Smith",
                CustomerPhoneNo = "(513) 456-7890"
            };
            
        }

        /// <summary>
        /// Adds a new customer
        /// </summary>
        /// <param name="merchantID">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="newCustomer"></param>
        /// <returns>new customer</returns>
        [HttpPost("merchants/{merchantID:guid}/customers")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<CustomerMBE> AddCustomer(Guid merchantID, NewCustomerMBE newCustomer)
        {
            if (merchantID != new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"))
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            var customer = new CustomerMBE
            {
                CustomerID = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"),
                CustomerName = newCustomer.CustomerName,
                CustomerPhoneNo = newCustomer.CustomerPhoneNo
            };

            return CreatedAtAction(nameof(GetCustomer), new { merchantID = merchantID, 
                customerID = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21")}, customer);
        }

        /// <summary>
        /// Updates a customer
        /// </summary>
        /// <param name="merchantID">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="customerID">5056ce22-50fb-4f1e-bb84-60fb45e21c21</param>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantID:guid}/customers/{customerID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult UpdateCustomer(Guid merchantID, Guid customerID, CustomerMBE customer)
        {
            if (merchantID != new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"))
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            if (customerID != new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"))
            {
                return NotFound($"Customer with ID: {customerID} on Merchant with ID: {merchantID} not found");
            }
            if (customerID != customer.CustomerID)
            {
                return BadRequest(new ArgumentException(nameof(customer.CustomerID), @"The customerID in the request body did not match the one in the url"));
            }
            return NoContent();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerID">5056ce22-50fb-4f1e-bb84-60fb45e21c21</param>
        /// <param name="merchantID">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        [HttpDelete("merchants/{merchantID:guid}/customers/{customerID:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCustomerByID(Guid merchantID, Guid customerID)
        {
            if (merchantID != new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"))
            {
                return NotFound($"Merchant with ID: {merchantID} not found");
            }
            if (customerID != new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"))
            {
                return NotFound($"Customer with ID: {customerID} not found");
            }

            return NoContent();
        }
    }
}
