using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PayAway.WebAPI.Migrations
{
    public partial class Milestone2Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CatalogItems",
                columns: table => new
                {
                    MerchantID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemUnitPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItems", x => new { x.MerchantID, x.ItemGuid });
                });

            migrationBuilder.CreateTable(
                name: "OrderEvents",
                columns: table => new
                {
                    OrderGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventDateTimeUTC = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OrderStatus = table.Column<string>(type: "TEXT", nullable: false),
                    EventDescription = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderEvents", x => new { x.OrderGuid, x.EventGuid });
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemUnitPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => new { x.OrderGuid, x.ItemGuid });
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CreditCardNumber = table.Column<string>(type: "TEXT", nullable: true),
                    AuthCode = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    OrderDateTimeUTC = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderGuid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_MerchantID_ItemName",
                table: "CatalogItems",
                columns: new[] { "MerchantID", "ItemName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItems");

            migrationBuilder.DropTable(
                name: "OrderEvents");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

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
    }
}
