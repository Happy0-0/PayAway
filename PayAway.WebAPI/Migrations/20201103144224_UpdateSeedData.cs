using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PayAway.WebAPI.Migrations
{
    public partial class UpdateSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerID", "MerchantID", "CustomerName", "CustomerPhoneNo" },
                values: new object[] { new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"), new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"), "Test Customer", "(513) 498-6016" });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "MerchantID", "IsActive", "IsSupportsTips", "LogoUrl", "MerchantName" },
                values: new object[] { new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"), true, true, "https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/4670e0dc-0335-4370-a3b1-24d9fa1dfdbf.png", "Test Merchant #1" });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "MerchantID", "IsActive", "IsSupportsTips", "LogoUrl", "MerchantName" },
                values: new object[] { new Guid("5d590431-95d2-4f8a-b2d9-6eb4d8cabc89"), false, true, "https://innovatein48sa.blob.core.windows.net/innovatein48-bc/Merchants/062c5897-208a-486a-8c6a-76707b9c07eb.png", "Test Merchant #2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumns: new[] { "CustomerID", "MerchantID" },
                keyValues: new object[] { new Guid("5056ce22-50fb-4f1e-bb84-60fb45e21c21"), new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d") });

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "MerchantID",
                keyValue: new Guid("5d590431-95d2-4f8a-b2d9-6eb4d8cabc89"));

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "MerchantID",
                keyValue: new Guid("f8c6f5b6-533e-455f-87a1-ced552898e1d"));
        }
    }
}
