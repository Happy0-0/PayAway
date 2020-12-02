using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PayAway.WebAPI.Interfaces;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Utilities;

namespace PayAway.WebAPI.Controllers.v0
{
    /// <summary>
    /// This is v0 for the Demo Controller.
    /// </summary>
    /// <remarks>
    /// This version is for the front-end team to have data to develop on until the working WebAPI is available
    /// </remarks>
    [Route("api/[controller]/v0")]
    [ApiController]
    public class DemoController : ControllerBase, IDemoController
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
            return Ok(new List<MerchantMBE>
            {
                new MerchantMBE
                {
                    MerchantGuid = Constants.MERCHANT_1_GUID,
                    MerchantName = @"Domino's Pizza",
                    MerchantUrl = new Uri("https://www.dominos.com"),
                    LogoUrl = HttpHelpers.BuildFullURL(this.Request, Constants.MERCHANT_1_LOGO_FILENAME),
                    IsSupportsTips = true,
                    IsActive = true
                },
                new MerchantMBE
                {
                    MerchantGuid = Constants.MERCHANT_2_GUID,
                    MerchantName = @"Raising Cane's",
                    MerchantUrl = new Uri("https://www.raisingcanes.com"),
                    LogoUrl = HttpHelpers.BuildFullURL(this.Request, Constants.MERCHANT_2_LOGO_FILENAME),
                    IsSupportsTips = true,
                    IsActive = false
                }
            });
        }

        /// <summary>
        /// Gets merchant information and associated customers using a GUID.
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns></returns>
        /// <remarks>Requires a merchantID</remarks>
        [HttpGet("merchants/{merchantGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MerchantMBE> GetMerchant(Guid merchantGuid)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }

            return Ok(new MerchantMBE
            {
                MerchantGuid = Constants.MERCHANT_1_GUID,
                MerchantName = @"Domino's Pizza",
                MerchantUrl = new Uri("https://www.dominos.com"),
                LogoUrl = HttpHelpers.BuildFullURL(this.Request, Constants.MERCHANT_1_LOGO_FILENAME),
                IsSupportsTips = true,
                IsActive = true,
                DemoCustomers = new List<DemoCustomerMBE>()
                {
                    new DemoCustomerMBE
                    {
                        CustomerGuid = Constants.MERCHANT_1_CUSTOMER_1_GUID,
                        CustomerName = @"Joe Smith",
                        CustomerPhoneNo = @"(513) 456-7890"
                    },
                    new DemoCustomerMBE
                    {
                        CustomerGuid = Constants.MERCHANT_1_CUSTOMER_2_GUID,
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
        public ActionResult<MerchantMBE> AddMerchant([FromBody] NewMerchantMBE newMerchant)
        {
            // validate request data
            if (string.IsNullOrEmpty(newMerchant.MerchantName))
            {
                return BadRequest(new ArgumentNullException(nameof(newMerchant.MerchantName), @"You must supply a non blank value for the Merchant Name."));
            }
            // validate the input params
            /*if (!Uri.IsWellFormedUriString(newMerchant.MerchantUrl.ToString(), UriKind.Absolute))
            {

                return BadRequest(new ArgumentException(nameof(newMerchant.MerchantUrl), @"The merchant url is incorrect. Make sure the url has https:// or http://"));
            }*/

            var merchant = new MerchantMBE
            {
                MerchantGuid = Constants.MERCHANT_1_GUID,
                MerchantName = newMerchant.MerchantName,
                MerchantUrl = new Uri("https://www.testmerchant.com"),
                IsSupportsTips = newMerchant.IsSupportsTips
            };

            return CreatedAtAction(nameof(GetMerchant), new { merchantGuid = merchant.MerchantGuid }, merchant);
        }

        /// <summary>
        /// Updates merchants using merchantID
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="updatedMerchant"></param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateMerchant(Guid merchantGuid, [FromBody] NewMerchantMBE updatedMerchant)
        {
            // validate the input params
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }
            // validate the input params
            /*if (!Uri.IsWellFormedUriString(updatedMerchant.MerchantUrl.ToString(), UriKind.Absolute))
            {

                return BadRequest(new ArgumentException(nameof(updatedMerchant.MerchantUrl), @"The merchant url is incorrect. Make sure the url has https:// or http://"));
            }*/

            return NoContent();
        }

        /// <summary>
        /// Deletes merchant by merchantID
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns></returns>
        [HttpDelete("merchants/{merchantGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NewMerchantMBE), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteMerchant(Guid merchantGuid)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }

            return NoContent();
        }

        /// <summary>
        /// Makes selected merchant active and all other merchants inactive.
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns></returns>
        [HttpPost("merchants/{merchantGuid:guid}/setactive")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult SetActiveMerchantForDemo(Guid merchantGuid)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }

            return NoContent();

        }

        /// <summary>
        /// Uploads the logo image.
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="formFile">The form file.</param>
        /// <returns>ActionResult&lt;System.String&gt;.</returns>
        [HttpPost("merchants/{merchantGuid:guid}/uploadImage")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<string> UploadLogoImage(Guid merchantGuid, IFormFile formFile)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return BadRequest($"Merchant with ID: {merchantGuid} not found");
            }

            return Ok();
        }

        #endregion

        #region === Customer Methods ================================
        /// <summary>
        /// Gets list of all customers that belong to a specific merchant
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <returns>list of customers</returns>
        [HttpGet("merchants/{merchantGuid:guid}/customers")]
        [ProducesResponseType(typeof(List<DemoCustomerMBE>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<DemoCustomerMBE>> GetDemoCustomers(Guid merchantGuid)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }

            return new List<DemoCustomerMBE>
            {
                new DemoCustomerMBE
                {
                    CustomerGuid = Constants.MERCHANT_1_CUSTOMER_1_GUID,
                    CustomerName = "Joe Smith",
                    CustomerPhoneNo = "(513) 456-7890"
                },
                new DemoCustomerMBE
                {
                    CustomerGuid = Constants.MERCHANT_1_CUSTOMER_2_GUID,
                    CustomerName = @"Jane Doe",
                    CustomerPhoneNo = @"(513) 555-1212"
                }
            };
        }

        /// <summary>
        /// Gets a specific customer by merchantID and customerID
        /// </summary>
        /// <param name="merchantGuid">f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="demoCustomerGuid">5056ce22-50fb-4f1e-bb84-60fb45e21c21</param>
        /// <returns>customer</returns>
        [HttpGet("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}")]
        [ProducesResponseType(typeof(DemoCustomerMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DemoCustomerMBE> GetDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }
            else if (demoCustomerGuid != Constants.MERCHANT_1_CUSTOMER_1_GUID)
            {
                return NotFound($"Customer with ID: {demoCustomerGuid} on Merchant with ID: {merchantGuid} not found");
            }

            return new DemoCustomerMBE
            {
                CustomerGuid = Constants.MERCHANT_1_CUSTOMER_1_GUID,
                CustomerName = "Joe Smith",
                CustomerPhoneNo = "(513) 456-7890"
            };

        }

        /// <summary>
        /// Adds a new customer
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="newDemoCustomer"></param>
        /// <returns>new customer</returns>
        [HttpPost("merchants/{merchantGuid:guid}/customers")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DemoCustomerMBE> AddDemoCustomer(Guid merchantGuid, [FromBody] NewDemoCustomerMBE newDemoCustomer)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }

            var customer = new DemoCustomerMBE
            {
                CustomerGuid = Constants.MERCHANT_1_CUSTOMER_1_GUID,
                CustomerName = newDemoCustomer.CustomerName,
                CustomerPhoneNo = newDemoCustomer.CustomerPhoneNo
            };

            return CreatedAtAction(nameof(GetDemoCustomer), new { merchantGuid = merchantGuid, customerGuid = customer.CustomerGuid }, customer);
        }

        /// <summary>
        /// Updates a customer
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="demoCustomerGuid">for testing use: 5056ce22-50fb-4f1e-bb84-60fb45e21c21</param>
        /// <param name="updatedDemoCustomer"></param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantID:guid}/customers/{demoCustomerGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid, [FromBody] NewDemoCustomerMBE updatedDemoCustomer)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }
            else if (demoCustomerGuid != Constants.MERCHANT_1_CUSTOMER_1_GUID)
            {
                return NotFound($"Customer with ID: {demoCustomerGuid} on Merchant with ID: {merchantGuid} not found");
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a customer on a merchant
        /// </summary>
        /// <param name="merchantGuid">for testing use: f8c6f5b6-533e-455f-87a1-ced552898e1d</param>
        /// <param name="demoCustomerGuid">for testing use: 5056ce22-50fb-4f1e-bb84-60fb45e21c21</param>
        [HttpDelete("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid)
        {
            if (merchantGuid != Constants.MERCHANT_1_GUID)
            {
                return NotFound($"Merchant with ID: {merchantGuid} not found");
            }
            else if (demoCustomerGuid != Constants.MERCHANT_1_CUSTOMER_1_GUID)
            {
                return NotFound($"Customer with ID: {demoCustomerGuid} on Merchant with ID: {merchantGuid} not found");
            }

            return NoContent();
        }

        #endregion

    }
}
