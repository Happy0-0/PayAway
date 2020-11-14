using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v0;
using System;

namespace PayAway.WebAPI.Interfaces
{
    public interface IMerchantController
    {
        ActionResult<ActiveMerchantMBE> GetActiveMerchant();

        ActionResult<OrderQueueMBE> GetOrderQueue();


        ActionResult<MerchantOrderMBE> CreateMerchantOrder([FromBody] NewMerchantOrderMBE newMerchantOrder);

        ActionResult UpdateMerchantOrder(Guid orderGuid, [FromBody] NewMerchantOrderMBE updatedMerchantOrder);

        ActionResult<MerchantOrderMBE> GetMerchantOrder(Guid orderGuid);

        ActionResult SendOrderPaymentRequest(Guid orderGuid);

    }
}