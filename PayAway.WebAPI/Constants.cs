using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI
{
    internal static class Constants
    {
        // demo ids
        internal static Guid MERCHANT_1_GUID = new Guid(@"f8c6f5b6-533e-455f-87a1-ced552898e1d");
        internal static Guid MERCHANT_1_LOGO_GUID = new Guid(@"4670e0dc-0335-4370-a3b1-24d9fa1dfdbf");
        internal static Guid MERCHANT_1_CUSTOMER_1_GUID = new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21");
        internal static Guid MERCHANT_1_CUSTOMER_2_GUID = new Guid("8b9b276a-cf81-47bf-97dc-3977cd464787");
        internal static Guid MERCHANT_2_GUID = new Guid(@"5d590431-95d2-4f8a-b2d9-6eb4d8cabc89");
        internal static Guid MERCHANT_2_LOGO_GUID = new Guid(@"062c5897-208a-486a-8c6a-76707b9c07eb");

        internal static Guid ORDER_1_GUID = new Guid(@"43e351fe-3cbc-4e36-b94a-9befe28637b3");
        internal static Guid ORDER_2_GUID = new Guid(@"fd07b84b-852e-47c9-b6d6-d248bd9e6bed");

        internal static Guid CATALOG_ITEM_1_GUID = new Guid(@"6b95573c-fbe5-4560-93c9-23c6a9baa500");
        internal static Guid CATALOG_ITEM_2_GUID = new Guid(@"f6a1e3a5-c563-48d0-9a15-f7a23c5ab688");
        internal static Guid CATALOG_ITEM_3_GUID = new Guid(@"51d06775-0be8-4dc9-9c25-2146f6a91fef");


        internal static readonly CultureInfo UnitedStates = CultureInfo.GetCultureInfo("en-US");
    }
}
