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
    /// This is v0 of the CustomerController
    /// </summary>
    /// <remarks>
    /// This version is for the front-end team to have data to develop on until the working WebAPI is available
    /// </remarks>
    [Route("api/[controller]/v0")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult<CustomerOrderMBE> GetCustomerOrder()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult<PaymentInfoMBE> PostOrderPayment()
        {
            throw new NotImplementedException();
        }
    }
}
