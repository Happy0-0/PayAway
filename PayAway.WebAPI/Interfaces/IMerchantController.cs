using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v0;
using System;

namespace PayAway.WebAPI.Interfaces
{
    public interface IMerchantController
    {
        ActionResult<ActiveMerchantMBE> GetActiveMerchant();

        ActionResult<OrderQueueMBE> GetOrderQueue();


        ActionResult<OrderMBE> CreateOrder([FromBody] NewOrderMBE newOrder);

        ActionResult UpdateOrder(Guid orderGuid, [FromBody] NewOrderMBE updatedOrder);

        ActionResult<OrderMBE> GetOrder(Guid orderGuid);

        ActionResult SendOrderPaymentRequest(Guid orderGuid);

    }
}