using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                    MerchantId = 1,
                    DemoCustomerId = 1,
                    DemoCustomerGuid = Constants.MERCHANT_1_CUSTOMER_1_GUID,
                    CustomerName = @"Test Customer 1",
                    CustomerPhoneNo = @"(513) 498-6016"
                },
                new DemoCustomerDBE()
                {
                    MerchantId = 1,
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
                    CatalogItemGuid = Constants.CATALOG_ITEM_1_GUID,
                    ItemName = "Product/Service 1",
                    ItemUnitPrice = 10.51M
                },
                new CatalogItemDBE
                {
                    MerchantId = 0,
                    CatalogItemId = 2,
                    CatalogItemGuid = Constants.CATALOG_ITEM_2_GUID,
                    ItemName = "Product/Service 2",
                    ItemUnitPrice = 20.52M
                },
                new CatalogItemDBE
                {
                    MerchantId = 0,
                    CatalogItemId = 3,
                    CatalogItemGuid = Constants.CATALOG_ITEM_3_GUID,
                    ItemName = "Product/Service 3",
                    ItemUnitPrice = 15.92M
                },
            };

            return seedCatalogueData;
        }

        public static List<OrderDBE> GetSeedOrders()
        {
            var seedOrders = new List<OrderDBE>()
            {
                new OrderDBE
                {
                    OrderId = 1,
                    OrderGuid = Constants.ORDER_1_GUID,
                    MerchantId = 1,
                    OrderDateTimeUTC = DateTime.Now.AddMinutes(-10),
                    Status = @"Paid",
                    CustomerName = @"Joe Smith",
                    PhoneNumber = @"(555) 555-5555",
                    CreditCardNumber = @"1234-5678-9101-1121",
                    AuthCode = @"506"
                },
                new OrderDBE
                {
                    OrderId = 2,
                    OrderGuid = Constants.ORDER_2_GUID,
                    MerchantId = 1,
                    OrderDateTimeUTC = DateTime.Now.AddMinutes(-5),
                    Status = @"Order Entered",
                    CustomerName = @"Jane Doe",
                    PhoneNumber = @"(444) 444-4444",
                    CreditCardNumber = @"9876-5432-1098-7654",
                    AuthCode = string.Empty
                }
            };

            return seedOrders;
        }

        public static List<OrderEventDBE> GetOrderEvents()
        {
            var seedOrderEvents = new List<OrderEventDBE>
            {
                new OrderEventDBE
                {
                    OrderId = 1,
                    EventDateTimeUTC = DateTime.Now.AddMinutes(-10),
                    OrderStatus = @"New Order",
                    EventDescription = @"Order entered."
                },
                new OrderEventDBE
                {
                    OrderId = 1,
                    EventDateTimeUTC = DateTime.Now.AddMinutes(-9),
                    OrderStatus = @"SMS Sent",
                    EventDescription = @"SMS Sent to (555) 555-5555"
                },
                new OrderEventDBE
                {
                    OrderId = 1,
                    EventDateTimeUTC = DateTime.Now.AddMinutes(-8),
                    OrderStatus = @"Paid",
                    EventDescription = @"Payment has been received for order."
                },

                new OrderEventDBE
                {
                    OrderId = 2,
                    EventDateTimeUTC = DateTime.Now.AddMinutes(-10),
                    OrderStatus = @"Order Entered",
                    EventDescription = @"Order entered."
                }
            };

            return seedOrderEvents;
        }

        public static List<OrderLineItemDBE> GetOrderLineItems()
        {
            var seedOrderLineItems = new List<OrderLineItemDBE>
            {
                new OrderLineItemDBE
                {
                    OrderId = 1,
                    CatalogItemGuid = Constants.CATALOG_ITEM_1_GUID,
                    ItemName = @"Product/Service 1",
                    ItemUnitPrice = 10.51M
                },
                new OrderLineItemDBE
                {
                    OrderId = 1,
                    CatalogItemGuid = Constants.CATALOG_ITEM_2_GUID,
                    ItemName = @"Product/Service 2",
                    ItemUnitPrice = 20.52M
                },
                new OrderLineItemDBE
                {
                    OrderId = 1,
                    CatalogItemGuid = Constants.CATALOG_ITEM_3_GUID,
                    ItemName = @"Product/Service 3",
                    ItemUnitPrice = 15.92M
                }
            };

            return seedOrderLineItems;
        }
    }
}
