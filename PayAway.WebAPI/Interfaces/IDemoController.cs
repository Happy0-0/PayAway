using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PayAway.WebAPI.Shared.Entities.v1;

namespace PayAway.WebAPI.Interfaces
{
    public interface IDemoController
    {
        Task<ActionResult> ResetDatabase(bool isPreloadEnabled);

        Task<ActionResult<IEnumerable<MerchantMBE>>> GetAllMerchants();

        Task<ActionResult<MerchantMBE>> GetMerchant(Guid merchantGuid);

        Task<ActionResult<MerchantMBE>> AddMerchant([FromBody] NewMerchantMBE newMerchant);

        Task<ActionResult> UpdateMerchant(Guid merchantGuid, [FromBody] NewMerchantMBE updatedMerchant);

        Task<ActionResult> DeleteMerchant(Guid merchantGuid);

        Task<ActionResult> SetActiveMerchantForDemo(Guid merchantGuid);

        Task<ActionResult<string>> UploadLogoImage(Guid merchantGuid, IFormFile formFile);


        Task<ActionResult<IEnumerable<DemoCustomerMBE>>> GetDemoCustomers(Guid merchantGuid);

        Task<ActionResult<DemoCustomerMBE>> GetDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid);

        Task<ActionResult<DemoCustomerMBE>> AddDemoCustomer(Guid merchantGuid, [FromBody] NewDemoCustomerMBE newDemoCustomer);

        Task<ActionResult> DeleteDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid);

        Task<ActionResult> UpdateDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid, [FromBody] NewDemoCustomerMBE updatedDemoCustomer);

    }
}