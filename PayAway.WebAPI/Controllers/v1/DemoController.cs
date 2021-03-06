﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.v1;
using PayAway.WebAPI.Entities.Database;
using PayAway.WebAPI.Interfaces;
using PayAway.WebAPI.Utilities;

using static System.Net.Mime.MediaTypeNames;

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
    public class DemoController : ControllerBase, IDemoController
    {
        private readonly long _fileSizeLimit = 100000;  // .1 MB
        private readonly string[] _permittedExtensions = { ".bmp", ".png", ".jpeg", ".jpg" };

        private readonly SQLiteDBContext _dbContext;
        private static IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="environment">The environment.</param>
        public DemoController(SQLiteDBContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }
        #region === Overall Demo Methods ================================
        /// <summary>
        /// Resets Database
        /// </summary>
        /// <param name="isPreloadEnabled">Optionally preloads sample data</param>
        /// <remarks>You can choose to preload a set of default data</remarks>
        [HttpPost("reset", Name = nameof(ResetDatabase))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ResetDatabase([FromQuery] bool isPreloadEnabled)
        {
            await _dbContext.ResetDBAsync(isPreloadEnabled);

            // purge all uploaded logo files except the demo ones
            var logoFolderName = System.IO.Path.Combine(_environment.ContentRootPath, GeneralConstants.LOGO_IMAGES_FOLDER_NAME);
            var logoFileNames = Directory.GetFiles(logoFolderName).ToList();

            var exclusionList = new List<string> 
            { 
                // use Path.Combine to suppport both windows and rhel deployment enironments
                System.IO.Path.Combine(logoFolderName, GeneralConstants.MERCHANT_1_LOGO_FILENAME),
                System.IO.Path.Combine(logoFolderName, GeneralConstants.MERCHANT_2_LOGO_FILENAME)
            };

            foreach(var logoFileName in logoFileNames.Except(exclusionList))
            {
                System.IO.File.Delete(logoFileName);
            }

            return NoContent();
        }
        #endregion

        #region === Merchant Methods ================================

        /// <summary>
        /// Get all merchants
        /// </summary>
        /// <returns>all merchants</returns>
        [HttpGet("merchants", Name = nameof(GetAllMerchants))]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MerchantMBE>>> GetAllMerchants()
        {
            // query the DB
            var dbMerchants = await _dbContext.GetAllMerchantsAsync();
            
            // if no results from DB, return an empty list
            if (dbMerchants == null)
            {
                return Ok(new List<MerchantMBE>());
            }

            // convert DB entities to the public entity types
            var merchants = dbMerchants.ConvertAll(dbM => (MerchantMBE)dbM);

            foreach(var merchant in merchants)
            {
               merchant.LogoUrl = (!string.IsNullOrEmpty(merchant.LogoFileName)) ? HttpHelpers.BuildFullURL(this.Request, merchant.LogoFileName) : null;
               var dbDemoCustomers = await _dbContext.GetDemoCustomersAsync(merchant.MerchantId);
               merchant.DemoCustomers = dbDemoCustomers.ConvertAll(dbDc => (DemoCustomerMBE)dbDc);
            }
            
            // return the response
            return Ok(merchants);
        }

        /// <summary>
        /// Gets a specific merchant and it's associated demo customers
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <returns></returns>
        [HttpGet("merchants/{merchantGuid:guid}", Name = nameof(GetMerchant))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MerchantMBE>> GetMerchant([FromRoute] Guid merchantGuid)
        {
            // query the DB
            var dbMerchant = await _dbContext.GetMerchantAndDemoCustomersAsync(merchantGuid);

            // if we did not find a matching merchant
            if(dbMerchant == null)
            {
                return NotFound($"MerchantGuid: [{merchantGuid}] not found");
            }

            // convert DB entity to the public entity type
            var merchant = (MerchantMBE)dbMerchant;
            merchant.LogoUrl = (!string.IsNullOrEmpty(merchant.LogoFileName)) ? HttpHelpers.BuildFullURL(this.Request, merchant.LogoFileName) : null;

            // create an empty working object
            var demoCustomers = new List<DemoCustomerMBE>();

            // optionally convert DB entities to the public entity type
            if (dbMerchant.DemoCustomers != null)
            {
                // convert DB entities to the public entity types
                demoCustomers = dbMerchant.DemoCustomers.ConvertAll(dbC => (DemoCustomerMBE)dbC);
            }

            // set the value of the property collection on the parent object
            merchant.DemoCustomers = demoCustomers;

            // return the response
            return Ok(merchant);
        }

        /// <summary>
        /// Add a new merchant
        /// </summary>
        /// <param name="newMerchant">object containing information about the new merchant</param>
        /// <returns>newMerchant</returns>
        [HttpPost("merchants", Name = nameof(AddMerchant))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MerchantMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MerchantMBE>> AddMerchant([FromBody] NewMerchantMBE newMerchant)
        {
            //trims merchant name so that it doesn't have trailing characters
            newMerchant.MerchantName = newMerchant.MerchantName.Trim();

            // validate the input params
            //if (!Uri.IsWellFormedUriString(newMerchant.MerchantUrl.ToString(), UriKind.Absolute))
            //{
            //    return BadRequest(new ArgumentException(nameof(newMerchant.MerchantUrl), @"The merchant url is incorrect. Make sure the url has https://"));
            //}

            try
            {
                // store the new merchant
                var newDBMerchant = new MerchantDBE()
                {
                    MerchantName = newMerchant.MerchantName,
                    IsSupportsTips = newMerchant.IsSupportsTips,
                    MerchantUrl = (Uri.IsWellFormedUriString(newMerchant.MerchantUrl.ToString(), UriKind.Absolute)) 
                                    ? newMerchant.MerchantUrl 
                                    : new Uri("https://www.testmerchant.com")
                };

                await _dbContext.InsertMerchantAsync(newDBMerchant);

                // convert DB entity to the public entity type
                var merchant = (MerchantMBE)newDBMerchant;

                // return the response
                return CreatedAtAction(nameof(GetMerchant), new { merchantGuid = merchant.MerchantGuid }, merchant);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] Failed trying to add merchant: [{newMerchant.MerchantName}]"));
            }
        }

        /// <summary>
        /// Updates a merchant
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <param name="updatedMerchant">object containing updated merchant information</param>
        /// <returns></returns>
        [HttpPut("merchants/{merchantGuid:guid}", Name = nameof(UpdateMerchant))]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateMerchant([FromRoute] Guid merchantGuid, [FromBody] NewMerchantMBE updatedMerchant)
        {
            //trims merchant name so that it doesn't have trailing characters
            updatedMerchant.MerchantName = updatedMerchant.MerchantName.Trim();

            // validate the input params
            /*if (!Uri.IsWellFormedUriString(updatedMerchant.MerchantUrl.ToString(), UriKind.Absolute)) 
            {
                
                return BadRequest(new ArgumentException(nameof(updatedMerchant.MerchantUrl), @"The merchant url is incorrect. Make sure the url has https://"));
            }*/

            // query the DB
            var dbMerchant = await _dbContext.GetMerchantAsync(merchantGuid);

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
                dbMerchant.MerchantUrl = (Uri.IsWellFormedUriString(updatedMerchant.MerchantUrl.ToString(), UriKind.Absolute))
                                            ? updatedMerchant.MerchantUrl
                                            : new Uri("https://www.testmerchant.com");

                await _dbContext.UpdateMerchantAsync(dbMerchant);
            }
            catch(Exception ex)
            {
                return BadRequest(new ApplicationException($"Error: [{ex.Message}] failed trying to update merchant: [{exisitingDBMerchantName}]"));
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a merchant
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <returns></returns>
        [HttpDelete("merchants/{merchantGuid:guid}", Name = nameof(DeleteMerchant))]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteMerchant([FromRoute] Guid merchantGuid)
        {
            // query the DB
            var dbMerchant = await _dbContext.GetMerchantAsync(merchantGuid);

            if (dbMerchant == null)
            {
                return NotFound($"MerchantGuid: [{merchantGuid}] is not valid");
            }

            // optionally delete the logo image if it exists
            //  Note: All user uploaded image files have a -logo suffix added
            if(!string.IsNullOrEmpty(dbMerchant.LogoFileName) && (dbMerchant.LogoFileName.IndexOf(@"-logo") > -1))
            {;
                var logoFilePathName = System.IO.Path.Combine(_environment.ContentRootPath,
                                                    GeneralConstants.LOGO_IMAGES_FOLDER_NAME,
                                                    dbMerchant.LogoFileName);

                System.IO.File.Delete(logoFilePathName);
            }

            await _dbContext.DeleteMerchantAsync(dbMerchant.MerchantId);

            return NoContent();
        }

        /// <summary>
        /// Makes a specific merchant active for the next demo
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <returns></returns>
        [HttpPost("merchants/{merchantGuid:guid}/setactive", Name = nameof(SetActiveMerchantForDemo))]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SetActiveMerchantForDemo([FromRoute] Guid merchantGuid)
        {
            // query the DB
            var merchantToMakeActive = await _dbContext.GetMerchantAsync(merchantGuid);

            // if we did not find a matching merchant
            if (merchantToMakeActive == null)
            {
                return NotFound($"MerchantID: [{merchantGuid}] not found");
            }

            //update merchant in the db
            await _dbContext.SetActiveMerchantForDemoAsync(merchantToMakeActive);

            return NoContent();
        }

        /// <summary>Uploads the logo image for a merchant.</summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <param name="imageFile">The file containing the logo in one of the supported formats.</param>
        /// <returns>ActionResult&lt;System.String&gt;.</returns>
        /// <remarks>
        /// Supported image formats are: bmp, png, jpg, jpeg
        /// Max Image Size: 100KB
        /// </remarks>
        [HttpPost("merchants/{merchantGuid:guid}/uploadImage", Name = nameof(UploadLogoImage))]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> UploadLogoImage([FromRoute] Guid merchantGuid, IFormFile imageFile)
        {
            // Step 1: Get the merchant
            var dbMerchant = await _dbContext.GetMerchantAsync(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException($"MerchantID: [{merchantGuid}] not found", nameof(merchantGuid)));
            }

            // Step 2: Validate supported image type and that image format in the file matches the extension
            (byte[] fileContents, string errorMessage) = ImageFileHelpers.ProcessFormFile(imageFile, _permittedExtensions, _fileSizeLimit);

            if (fileContents.Length == 0)
            {
                return BadRequest(new ArgumentException(errorMessage, nameof(imageFile)));
            }

            // Step 3: Store in local folder
            string imageFileName = $"{merchantGuid}-logo{System.IO.Path.GetExtension(imageFile.FileName)}";
            // use Path.Combine to deal with O/S differences re Linux: "/" vs Windows: "\"
            string imageFilePathName = System.IO.Path.Combine(_environment.ContentRootPath,
                                                                GeneralConstants.LOGO_IMAGES_FOLDER_NAME,
                                                                imageFileName);
            using (var fileStream = System.IO.File.Create(imageFilePathName))
            {
                fileStream.Write(fileContents);
            }

            // Step 4: Update the merchant
            dbMerchant.LogoFileName = imageFileName;
            await _dbContext.UpdateMerchantAsync(dbMerchant);

            // Step 5: Return results        https://localhost:44318/LogoImages/f8c6f5b6-533e-455f-87a1-ced552898e1d.png
            var imageUri = HttpHelpers.BuildFullURL(this.Request, imageFileName);
            return Ok(imageUri);
        }

        #endregion

        #region === Demo Customer Methods ================================

        /// <summary>
        /// Gets list of all demo customers that belong to a specific merchant
        /// </summary>
        /// <param name="merchantGuid">the unique identifier for the merhant</param>
        /// <returns>list of customers</returns>
        /// <remarks>A pre-setup demo customer will be will have the same demo experience as the customer entered on the order during the demo</remarks>
        [HttpGet("merchants/{merchantGuid:guid}/customers", Name = nameof(GetDemoCustomers))]
        [ProducesResponseType(typeof(List<DemoCustomerMBE>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DemoCustomerMBE>>> GetDemoCustomers([FromRoute] Guid merchantGuid)
        {
            // query the DB
            var dbMerchant = await _dbContext.GetMerchantAsync(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException($"MerchantGuid: [{merchantGuid}] not found", nameof(merchantGuid)));
            }

            var dbDemoCustomers = await _dbContext.GetDemoCustomersAsync(dbMerchant.MerchantId);

            // if no results from DB, return an empty list
            if (dbDemoCustomers == null)
            {
                return Ok(new List<DemoCustomerMBE>());
            }

            // convert DB entities to the public entity types
            var demoCustomers = dbDemoCustomers.ConvertAll(dbDC => (DemoCustomerMBE)dbDC);

            // return the response
            return Ok(demoCustomers);
        }

        /// <summary>
        /// Gets a specific demo customer
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <param name="demoCustomerGuid">The unique identifier for the demo customer.</param>
        /// <returns>a specified customer</returns>
        /// <remarks>A pre-setup demo customer will be will have the same demo experience as the customer entered on the order during the demo</remarks>
        [HttpGet("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}", Name = nameof(GetDemoCustomer))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DemoCustomerMBE), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DemoCustomerMBE>> GetDemoCustomer([FromRoute] Guid merchantGuid, [FromRoute] Guid demoCustomerGuid)
        {
            // query the DB
            var dbMerchant = await _dbContext.GetMerchantAsync(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException($"MerchantGuid: [{merchantGuid}] not found", nameof(merchantGuid)));
            }

            //query DB for a collection of customers from a specific merchant.
            var dbDemoCustomers = await _dbContext.GetDemoCustomersAsync(dbMerchant.MerchantId);
            var dbDemoCustomer = dbDemoCustomers.Where(dc => dc.DemoCustomerGuid == demoCustomerGuid).FirstOrDefault();

            // if we did not find a matching demo customer
            if (dbDemoCustomer == null)
            {
                return NotFound($"CustomerGuid: [{demoCustomerGuid}] on MerchantGuid: [{merchantGuid}] not found");
            }

            var customer = (DemoCustomerMBE)dbDemoCustomer;

            return Ok(customer);
        }

        /// <summary>
        /// Adds a new demo customer to a merchant
        /// </summary>
        /// <param name="merchantGuid">The unique identifier for the merchant</param>
        /// <param name="newDemoCustomer">Object containing information about the new demo customer</param>
        /// <returns></returns>
        /// <remarks>A pre-setup demo customer will be will have the same demo experience as the customer entered on the order during the demo</remarks>
        [HttpPost("merchants/{merchantGuid:guid}/customers", Name = nameof(AddDemoCustomer))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DemoCustomerMBE), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DemoCustomerMBE>> AddDemoCustomer([FromRoute] Guid merchantGuid, [FromBody] NewDemoCustomerMBE newDemoCustomer)
        {
            //trims Customer name so that it doesn't have trailing characters
            newDemoCustomer.CustomerName = newDemoCustomer.CustomerName.Trim();

            // validate request data
            (bool isValidPhoneNo, string formatedPhoneNo, string normalizedPhoneNo) = Utilities.PhoneNoHelpers.NormalizePhoneNo(newDemoCustomer.CustomerPhoneNo);
            if (!isValidPhoneNo)
            {
                ModelState.AddModelError(nameof(newDemoCustomer.CustomerPhoneNo), $"[{newDemoCustomer.CustomerPhoneNo}] is NOT a supported Phone No format.");
                return BadRequest(ModelState);
            }

            //query the db for the merchant
            var dbMerchant = await _dbContext.GetMerchantAsync(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException($"MerchantGuid: [{merchantGuid}] not found", nameof(merchantGuid)));
            }
            else
            {
                newDemoCustomer.CustomerPhoneNo = formatedPhoneNo;
            }

            try
            {
                //Store the new customer
                var newDBDemoCustomer = new DemoCustomerDBE()
                {
                    MerchantId = dbMerchant.MerchantId,
                    CustomerName = newDemoCustomer.CustomerName,
                    CustomerPhoneNo = newDemoCustomer.CustomerPhoneNo
                };

                await _dbContext.InsertDemoCustomerAsync(newDBDemoCustomer);

                // convert DB entity to the public entity type
                var customer = (DemoCustomerMBE)newDBDemoCustomer;

                // return the response
                return CreatedAtAction(nameof(GetDemoCustomer), new { merchantGuid = merchantGuid, demoCustomerGuid = customer.CustomerGuid }, customer);
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
        [HttpPut("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}", Name = nameof(UpdateDemoCustomer))]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateDemoCustomer([FromRoute] Guid merchantGuid, [FromRoute] Guid demoCustomerGuid, [FromBody] NewDemoCustomerMBE updatedDemoCustomer)
        {
            // query the db for the merchant
            var dbMerchant = await _dbContext.GetMerchantAsync(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException($"MerchantGuid: [{merchantGuid}] not found", nameof(merchantGuid)));
            }

            // trims Customer name so that it doesn't have trailing characters
            updatedDemoCustomer.CustomerName = updatedDemoCustomer.CustomerName.Trim();

            // validate the input params
            (bool isValidPhoneNo, string formatedPhoneNo, string normalizedPhoneNo) = Utilities.PhoneNoHelpers.NormalizePhoneNo(updatedDemoCustomer.CustomerPhoneNo);
            if (!isValidPhoneNo)
            {
                return BadRequest(new ArgumentNullException($"[{updatedDemoCustomer.CustomerPhoneNo}] is NOT a supported Phone No format.", nameof(updatedDemoCustomer.CustomerPhoneNo)));
            }

            // get the existing demo customer
            var dbDemoCustomers = await _dbContext.GetDemoCustomersAsync(dbMerchant.MerchantId);
            var dbDemoCustomer = dbDemoCustomers.Where(c => c.DemoCustomerGuid == demoCustomerGuid).FirstOrDefault();

            if (dbDemoCustomer == null)
            {
                return NotFound($"CustomerID: [{demoCustomerGuid}] on MerchantID: [{merchantGuid}] not found");
            }

            // grab the current phone no
            string existingCustomerPhoneNo = dbDemoCustomer.CustomerPhoneNo;

            try
            {
                //Save the updated customer
                dbDemoCustomer.CustomerName = updatedDemoCustomer.CustomerName;
                dbDemoCustomer.CustomerPhoneNo = formatedPhoneNo;

                await _dbContext.UpdateDemoCustomerAsync(dbDemoCustomer);
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
        [HttpDelete("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}", Name = nameof(DeleteDemoCustomer))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDemoCustomer([FromRoute] Guid merchantGuid, [FromRoute] Guid demoCustomerGuid)
        {
            // query the db for the merchant
            var dbMerchant = await _dbContext.GetMerchantAsync(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException($"MerchantGuid: [{merchantGuid}] not found", nameof(merchantGuid)));
            }

            // get the existing demo customer
            var dbDemoCustomers = await _dbContext.GetDemoCustomersAsync(dbMerchant.MerchantId);
            var dbDemoCustomer = dbDemoCustomers.Where(c => c.DemoCustomerGuid == demoCustomerGuid).FirstOrDefault();

            if (dbDemoCustomer == null)
            {
                return NotFound($"CustomerID: [{demoCustomerGuid}] on MerchantID: [{merchantGuid}] not found");
            }

            await _dbContext.DeleteDemoCustomerAsync(dbDemoCustomer.DemoCustomerId);

            return NoContent();
        }
        #endregion
    }
}
