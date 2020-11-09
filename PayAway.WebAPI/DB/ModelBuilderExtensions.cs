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
        static Guid merchant_1_id = new Guid(@"f8c6f5b6-533e-455f-87a1-ced552898e1d");
        static Guid merchant_1_logo_id = new Guid(@"4670e0dc-0335-4370-a3b1-24d9fa1dfdbf");
        static Guid merchant_1_customer_1_id = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21");
        static Guid merchant_1_customer_2_id = new Guid("8b9b276a-cf81-47bf-97dc-3977cd464787");
        static Guid merchant_2_id = new Guid(@"5d590431-95d2-4f8a-b2d9-6eb4d8cabc89");
        static Guid merchant_2_logo_id = new Guid(@"062c5897-208a-486a-8c6a-76707b9c07eb");

        /// <summary>
        /// Seeds the specified model builder.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var seedMerchants = GetSeedMerchants();
            modelBuilder.Entity<MerchantDBE>().HasData(seedMerchants);

            modelBuilder.Entity<CustomerDBE>().HasData(GetSeedCustomers(seedMerchants));
        }

        public static List<MerchantDBE> GetSeedMerchants()
        {
            var seedMerchants = new List<MerchantDBE>()
            {
                new MerchantDBE
                {
                    MerchantID = merchant_1_id,
                    MerchantName = @"Test Merchant #1",
                    LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_1_logo_id}.png",
                    IsSupportsTips = true,
                    IsActive = true
                },
                new MerchantDBE
                {
                    MerchantID = merchant_2_id,
                    MerchantName = @"Test Merchant #2",
                    LogoUrl = $"https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/{merchant_2_logo_id}.png",
                    IsSupportsTips = true,
                    IsActive = false
                }
            };

            return seedMerchants;
        }

        public static List<CustomerDBE> GetSeedCustomers(List<MerchantDBE> seedMerchants)
        {
            var merchant1 = seedMerchants.Where(m => m.MerchantID == merchant_1_id).FirstOrDefault();

            var seedCustomers = new List<CustomerDBE>()
            {
                new CustomerDBE()
                {
                    MerchantID = merchant_1_id,
                    CustomerID = merchant_1_customer_1_id,
                    CustomerName = @"Test Customer 1",
                    CustomerPhoneNo = @"(513) 498-6016"
                },
                new CustomerDBE()
                {
                    MerchantID = merchant_1_id,
                    CustomerID = merchant_1_customer_2_id,
                    CustomerName = @"Test Customer 2",
                    CustomerPhoneNo = @"(513) 791-9800"
                }
            };

            return seedCustomers;
        }
}
}
