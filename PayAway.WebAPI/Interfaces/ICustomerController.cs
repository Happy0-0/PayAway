using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v1;
using System;

namespace PayAway.WebAPI.Interfaces
{
    public interface ICustomerController
    {
        ActionResult<CustomerOrderMBE> GetCustomerOrder(Guid orderGuid);
        ActionResult SubmitOrderPayment(Guid orderGuid, PaymentInfoMBE paymentInfo);
    }
}