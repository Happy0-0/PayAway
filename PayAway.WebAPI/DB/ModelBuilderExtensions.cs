using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using PayAway.WebAPI.Entities.v1;

namespace PayAway.WebAPI.DB
{
    public static class ModelBuilderExtensions
    {

        // demo ids
        static Guid merchant_1_guid = new Guid(@"f8c6f5b6-533e-455f-87a1-ced552898e1d");
        static Guid merchant_1_logo_guid = new Guid(@"4670e0dc-0335-4370-a3b1-24d9fa1dfdbf");
        static Guid merchant_1_customer_1_guid = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21");
        static Guid merchant_1_customer_2_guid = new Guid("8b9b276a-cf81-47bf-97dc-3977cd464787");
        static Guid merchant_2_guid = new Guid(@"5d590431-95d2-4f8a-b2d9-6eb4d8cabc89");
        static Guid merchant_2_logo_guid = new Guid(@"062c5897-208a-486a-8c6a-76707b9c07eb");

        public static List<MerchantDBE> GetSeedMerchants()
        {
            var seedMerchants = new List<MerchantDBE>()
            {
                new MerchantDBE
                {
                    MerchantId = 1,
                    MerchantGuid = merchant_1_guid,
                    MerchantName = @"Test Merchant #1",
                    LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_1_logo_guid}.png",
                    IsSupportsTips = true,
                    IsActive = true
                },
                new MerchantDBE
                {
                    MerchantId = 2,
                    MerchantGuid = merchant_2_guid,
                    MerchantName = @"Test Merchant #2",
                    LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_2_logo_guid}.png",
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
                    DemoCustomerGuid = merchant_1_customer_1_guid,
                    CustomerName = @"Test Customer 1",
                    CustomerPhoneNo = @"(513) 498-6016"
                },
                new DemoCustomerDBE()
                {
                    MerchantID = 1,
                    DemoCustomerId = 2,
                    DemoCustomerGuid = merchant_1_customer_2_guid,
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
