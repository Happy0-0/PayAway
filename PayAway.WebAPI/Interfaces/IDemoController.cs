using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PayAway.WebAPI.Entities.v1;

namespace PayAway.WebAPI.Interfaces
{
    public interface IDemoController
    {
        ActionResult ResetDatabase(bool isPreloadEnabled);

        Task<ActionResult<IEnumerable<MerchantMBE>>> GetAllMerchants();

        ActionResult<MerchantMBE> GetMerchant(Guid merchantGuid);

        ActionResult<MerchantMBE> AddMerchant([FromBody] NewMerchantMBE newMerchant);

        ActionResult UpdateMerchant(Guid merchantGuid, [FromBody] NewMerchantMBE updatedMerchant);

        ActionResult DeleteMerchant(Guid merchantGuid);

        ActionResult SetActiveMerchantForDemo(Guid merchantGuid);

        ActionResult<string> UploadLogoImage(Guid merchantGuid, IFormFile formFile);


        ActionResult<IEnumerable<DemoCustomerMBE>> GetDemoCustomers(Guid merchantGuid);

        ActionResult<DemoCustomerMBE> GetDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid);

        ActionResult<DemoCustomerMBE> AddDemoCustomer(Guid merchantGuid, [FromBody] NewDemoCustomerMBE newDemoCustomer);

        ActionResult DeleteDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid);

        ActionResult UpdateDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid, [FromBody] NewDemoCustomerMBE updatedDemoCustomer);

    }
}