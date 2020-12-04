using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v1;
using System;

namespace PayAway.WebAPI.Interfaces
{
    public interface IMerchantController
    {
        ActionResult<ActiveMerchantMBE> GetActiveMerchant();

        ActionResult<OrderQueueMBE> GetOrderQueue();


        ActionResult<MerchantOrderMBE> CreateOrder([FromBody] NewOrderMBE newOrder);

        ActionResult UpdateOrder(Guid orderGuid, [FromBody] NewOrderMBE updatedOrder);

        ActionResult<MerchantOrderMBE> GetOrder(Guid orderGuid);

        ActionResult SendOrderPaymentRequest(Guid orderGuid);

    }
}