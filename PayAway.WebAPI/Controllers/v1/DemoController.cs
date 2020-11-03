using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.DB;
using PayAway.WebAPI.Entities.v0;

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
        // demo ids
        static Guid merchant_1_id = new Guid(@"f8c6f5b6-533e-455f-87a1-ced552898e1d");
        static Guid merchant_1_logo_id = new Guid(@"4670e0dc-0335-4370-a3b1-24d9fa1dfdbf");
        static Guid merchant_1_customer_1_id = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21");
        static Guid merchant_1_customer_2_id = new Guid("8b9b276a-cf81-47bf-97dc-3977cd464787");
        static Guid merchant_2_id = new Guid(@"5d590431-95d2-4f8a-b2d9-6eb4d8cabc89");
        static Guid merchant_2_logo_id = new Guid(@"062c5897-208a-486a-8c6a-76707b9c07eb");

        /// <summary>
        /// Gets all merchants
        /// </summary>
        /// <returns>all merchants</returns>
        [HttpGet("merchants")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<MerchantMBE>> GetAllMerchants()
        {
            // query the DB
            var dbMerchants = SQLiteDBContext.GetAllMerchants();

            // convert to the public entity types
            var merchants = dbMerchants.ConvertAll(oe => (MerchantMBE)oe);

            // return the data
            return Ok(merchants);
        }
    }
}
