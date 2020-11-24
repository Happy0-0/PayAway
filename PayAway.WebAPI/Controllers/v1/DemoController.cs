using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PayAway.WebAPI;
using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.v0;
using PayAway.WebAPI.Entities.v1;
using PayAway.WebAPI.Interfaces;
using PayAway.WebAPI.Utilities;
using Twilio.Rest.Studio.V1.Flow;
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

        public static IWebHostEnvironment _environment;

        public DemoController(IWebHostEnvironment environment)
        {
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
        public ActionResult ResetDatabase(bool isPreloadEnabled)
        {
            SQLiteDBContext.ResetDB(isPreloadEnabled);

            // purge all uploaded logo files except the demo ones
            var logoFolderName = _environment.ContentRootPath + $"\\{Constants.LOGO_IMAGES_FOLDER_NAME}";
            var logoFileNames = Directory.GetFiles(logoFolderName).ToList();

            var exclusionList = new List<string> 
            { 
                $"{logoFolderName}\\{Constants.MERCHANT_1_LOGO_FILENAME}",
                $"{logoFolderName}\\{Constants.MERCHANT_2_LOGO_FILENAME}",
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

            foreach(var merchant in merchants)
            {
               merchant.LogoUrl = (!string.IsNullOrEmpty(merchant.LogoFileName)) ? HttpHelpers.BuildFullURL(this.Request, merchant.LogoFileName) : null;
               var dbDemoCustomers = SQLiteDBContext.GetDemoCustomers(merchant.MerchantId);
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
        public ActionResult<MerchantMBE> GetMerchant(Guid merchantGuid)
        {
            // query the DB
            var dbMerchant = SQLiteDBContext.GetMerchantAndDemoCustomers(merchantGuid);

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
        [HttpPut("merchants/{merchantGuid:guid}", Name = nameof(UpdateMerchant))]
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
        [HttpDelete("merchants/{merchantGuid:guid}", Name = nameof(DeleteMerchant))]
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

            // optionally delete the logo image if it exists
            //  Note: All user uploaded image files have a -logo suffix added
            if(!string.IsNullOrEmpty(dbMerchant.LogoFileName) && (dbMerchant.LogoFileName.IndexOf(@"-logo") > -1))
            {
                var logoFilePathName = _environment.ContentRootPath + $"\\{Constants.LOGO_IMAGES_FOLDER_NAME}\\{dbMerchant.LogoFileName}";
                System.IO.File.Delete(logoFilePathName);
            }

            SQLiteDBContext.DeleteMerchant(dbMerchant.MerchantId);

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
        public ActionResult SetActiveMerchantForDemo(Guid merchantGuid)
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
        public ActionResult<string> UploadLogoImage(Guid merchantGuid, IFormFile imageFile)
        {
            // Step 1: Get the merchant
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException(nameof(merchantGuid), $"MerchantID: [{merchantGuid}] not found"));
            }

            // Step 2: Validate supported image type and that image format in the file matches the extension
            (byte[] fileContents, string errorMessage) = ImageFileHelpers.ProcessFormFile(imageFile, _permittedExtensions, _fileSizeLimit);

            if (fileContents.Length == 0)
            {
                return BadRequest(new ArgumentException(nameof(imageFile), errorMessage));
            }

            // Step 3: Store in local folder
            string imageFileName = $"{merchantGuid}-logo{System.IO.Path.GetExtension(imageFile.FileName)}";
            using (var fileStream = System.IO.File.Create(_environment.ContentRootPath + $"\\{Constants.LOGO_IMAGES_FOLDER_NAME}\\" + imageFileName))
            {
                fileStream.Write(fileContents);
            }

            // Step 4: Update the merchant
            dbMerchant.LogoFileName = imageFileName;
            SQLiteDBContext.UpdateMerchant(dbMerchant);

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
        public ActionResult<IEnumerable<DemoCustomerMBE>> GetDemoCustomers(Guid merchantGuid)
        {
            // query the DB
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException(nameof(merchantGuid), $"MerchantGuid: [{merchantGuid}] not found"));
            }

            var dbDemoCustomers = SQLiteDBContext.GetDemoCustomers(dbMerchant.MerchantId);

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
        public ActionResult<DemoCustomerMBE> GetDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid)
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

            var customer = (DemoCustomerMBE)dbCustomer;

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
        public ActionResult<DemoCustomerMBE> AddDemoCustomer(Guid merchantGuid, [FromBody] NewDemoCustomerMBE newDemoCustomer)
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

            (bool isValidPhoneNo, string formatedPhoneNo, string normalizedPhoneNo) = Utilities.PhoneNoHelpers.NormalizePhoneNo(newDemoCustomer.CustomerPhoneNo);
            if (!isValidPhoneNo)
            {
                return BadRequest(new ArgumentNullException(nameof(newDemoCustomer.CustomerPhoneNo), $"[{newDemoCustomer.CustomerPhoneNo}] is NOT a supported Phone No format."));
            }

            //query the db for the merchant
            var dbMerchant = SQLiteDBContext.GetMerchant(merchantGuid);

            // if we did not find a matching merchant
            if (dbMerchant == null)
            {
                return BadRequest(new ArgumentException(nameof(merchantGuid), $"MerchantGuid: [{merchantGuid}] not found"));
            }
            else
            {
                newDemoCustomer.CustomerPhoneNo = formatedPhoneNo;
            }

            try
            {
                //Store the new customer
                var dbCustomer = SQLiteDBContext.InsertDemoCustomer(dbMerchant.MerchantId, newDemoCustomer);

                // convert DB entity to the public entity type
                var customer = (DemoCustomerMBE)dbCustomer;

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
        public ActionResult UpdateDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid, [FromBody] NewDemoCustomerMBE updatedDemoCustomer)
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

            (bool isValidPhoneNo, string formatedPhoneNo, string normalizedPhoneNo) = Utilities.PhoneNoHelpers.NormalizePhoneNo(updatedDemoCustomer.CustomerPhoneNo);
            if (!isValidPhoneNo)
            {
                return BadRequest(new ArgumentNullException(nameof(updatedDemoCustomer.CustomerPhoneNo), $"[{updatedDemoCustomer.CustomerPhoneNo}] is NOT a supported Phone No format."));
            }

            // get the existing demo customer
            var dbDemoCustomer = SQLiteDBContext.GetDemoCustomers(dbMerchant.MerchantId).Where(c => c.DemoCustomerGuid == demoCustomerGuid).FirstOrDefault();

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
        [HttpDelete("merchants/{merchantGuid:guid}/customers/{demoCustomerGuid:guid}", Name = nameof(DeleteDemoCustomer))]
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
