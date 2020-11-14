using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PayAway.WebAPI.Controllers.v0;
using PayAway.WebAPI.Entities.v1;

namespace PayAway.WebAPI.DB
{
    public static class SeedData
    {
        public static List<MerchantDBE> GetSeedMerchants()
        {
            var seedMerchants = new List<MerchantDBE>()
            {
                new MerchantDBE
                {
                    MerchantId = 1,
                    MerchantGuid = Constants.MERCHANT_1_GUID,
                    MerchantName = @"Test Merchant #1",
                    LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{Constants.MERCHANT_1_LOGO_GUID}.png",
                    IsSupportsTips = true,
                    IsActive = true
                },
                new MerchantDBE
                {
                    MerchantId = 2,
                    MerchantGuid = Constants.MERCHANT_2_GUID,
                    MerchantName = @"Test Merchant #2",
                    LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{Constants.MERCHANT_2_LOGO_GUID}.png",
                    IsSupportsTips = true,
                    IsActive = false
                }
            };

            return seedMerchants;
        }

        public static List<DemoCustomerDBE> GetSeedDemoCustomers()
        {
            var seedDemoCustomers = new List<DemoCustomerDBE>()
            {
                new DemoCustomerDBE()
                {
                    MerchantID = 1,
                    DemoCustomerId = 1,
                    DemoCustomerGuid = Constants.MERCHANT_1_CUSTOMER_1_GUID,
                    CustomerName = @"Test Customer 1",
                    CustomerPhoneNo = @"(513) 498-6016"
                },
                new DemoCustomerDBE()
                {
                    MerchantID = 1,
                    DemoCustomerId = 2,
                    DemoCustomerGuid = Constants.MERCHANT_1_CUSTOMER_2_GUID,
                    CustomerName = @"Test Customer 2",
                    CustomerPhoneNo = @"(513) 791-9800"
                }
            };

            return seedDemoCustomers;
        }

        public static List<CatalogItemDBE> GetSeedCatalogueItems()
        {
            var seedCatalogueData = new List<CatalogItemDBE>()
            {
                new CatalogItemDBE
                {
                    MerchantId = 0,
                    CatalogItemId = 1,
                    ItemName = "Product/Service 1",
                    ItemUnitPrice = 10.51M
                },
                new CatalogItemDBE
                {
                    MerchantId = 0,
                    CatalogItemId = 2,
                    ItemName = "Product/Service 2",
                    ItemUnitPrice = 20.52M
                },
                new CatalogItemDBE
                {
                    MerchantId = 0,
                    CatalogItemId = 3,
                    ItemName = "Product/Service 3",
                    ItemUnitPrice = 15.92M
                },
            };

            return seedCatalogueData;
        }

    }
}
