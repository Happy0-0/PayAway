using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using PayAway.WebAPI.Entities.v1;

namespace PayAway.WebAPI.Interfaces
{
    public interface ICustomerController
    {
        Task<ActionResult<CustomerOrderMBE>> GetCustomerOrder(Guid orderGuid);
        Task<ActionResult> SubmitOrderPayment(Guid orderGuid, PaymentInfoMBE paymentInfo);
    }
}