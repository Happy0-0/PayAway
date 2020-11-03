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
            var merchants = dbMerchants.ConvertAll(oe => (MerchantMBE)oe);

            // return the response
            return Ok(merchants);
        }
    }
}
