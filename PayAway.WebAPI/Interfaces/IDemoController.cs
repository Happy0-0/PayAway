using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayAway.WebAPI.Entities.v0;
using System;
using System.Collections.Generic;

namespace PayAway.WebAPI.Interfaces
{
    public interface IDemoController
    {
        ActionResult ResetDatabase(bool isPreloadEnabled);

        ActionResult<IEnumerable<MerchantMBE>> GetAllMerchants();

        ActionResult<MerchantMBE> GetMerchant(Guid merchantGuid);

        ActionResult<MerchantMBE> AddMerchant([FromBody] NewMerchantMBE newMerchant);

        ActionResult UpdateMerchant(Guid merchantGuid, [FromBody] NewMerchantMBE updatedMerchant);

        ActionResult DeleteMerchant(Guid merchantGuid);

        ActionResult SetActiveMerchantForDemo(Guid merchantGuid);

        ActionResult<string> UploadLogoImage(Guid merchantGuid, IFormFile formFile);


        ActionResult<IEnumerable<CustomerMBE>> GetDemoCustomers(Guid merchantGuid);

        ActionResult<CustomerMBE> GetDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid);

        ActionResult<CustomerMBE> AddDemoCustomer(Guid merchantGuid, [FromBody] NewCustomerMBE newDemoCustomer);

        ActionResult DeleteDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid);

        ActionResult UpdateDemoCustomer(Guid merchantGuid, Guid demoCustomerGuid, [FromBody] NewCustomerMBE updatedDemoCustomer);

    }
}