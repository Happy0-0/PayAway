using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v0;
using System;

namespace PayAway.WebAPI.Interfaces
{
    public interface IMerchantController
    {
        ActionResult<ActiveMerchantMBE> GetActiveMerchant();

        ActionResult<OrderQueueMBE> GetOrderQueue();


        ActionResult<OrderMBE> CreateMerchantOrder([FromBody] NewOrderMBE newMerchantOrder);

        ActionResult UpdateMerchantOrder(Guid orderGuid, [FromBody] NewOrderMBE updatedMerchantOrder);

        ActionResult<OrderMBE> GetOrder(Guid orderGuid);

        ActionResult SendOrderPaymentRequest(Guid orderGuid);

    }
}