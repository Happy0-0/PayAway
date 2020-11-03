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
            };

            return seedMerchants;
        }

        public static List<CustomerDBE> GetSeedCustomers(List<MerchantDBE> seedMerchants)
        {
            var seedCustomers = new List<CustomerDBE>()
            {
            };

            return seedCustomers;
        }
}
}
