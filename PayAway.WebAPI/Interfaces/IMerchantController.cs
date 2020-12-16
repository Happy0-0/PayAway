using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using PayAway.WebAPI.Shared.Entities.v1;

namespace PayAway.WebAPI.Interfaces
{
    public interface IMerchantController
    {
        Task<ActionResult<ActiveMerchantMBE>> GetActiveMerchant();

        Task<ActionResult<OrderQueueMBE>> GetOrderQueue();


        Task<ActionResult<MerchantOrderMBE>> CreateOrder([FromBody] NewOrderMBE newOrder);

        Task<ActionResult> UpdateOrder(Guid orderGuid, [FromBody] NewOrderMBE updatedOrder);

        Task<ActionResult<MerchantOrderMBE>> GetOrder(Guid orderGuid);

        Task<ActionResult> SendOrderPaymentRequest(Guid orderGuid);
    }
}