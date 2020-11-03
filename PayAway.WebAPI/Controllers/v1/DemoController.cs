using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

using PayAway.WebAPI.Entities.v0;

namespace PayAway.WebAPI.Controllers.v1
{
    /// <summary>
    /// This is v0 for the Demo Controller.
    /// </summary>
    /// <remarks>
    /// This version is for the front-end team to have data to develop on until further development.
    /// </remarks>
    [Route("api/[controller]/v1")]
    [ApiController]
    public class DemoController : ControllerBase
    {

        #region === Overall Demo Methods ================================
        /// <summary>
        /// Resets Database
        /// </summary>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult ResetDatabase()
        {
            return NoContent();
        }
        #endregion


    }
}
