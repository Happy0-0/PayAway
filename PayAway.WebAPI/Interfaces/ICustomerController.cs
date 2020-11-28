using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v0;
using System;

namespace PayAway.WebAPI.Interfaces
{
    public interface ICustomerController
    {
        ActionResult<CustomerOrderMBE> GetCustomerOrder(Guid orderGuid);
        ActionResult<PaymentInfoMBE> SendOrderPayment(Guid orderGuid, PaymentInfoMBE paymentInfo);
    }
}