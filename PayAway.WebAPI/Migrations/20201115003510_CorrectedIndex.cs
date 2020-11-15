using Microsoft.EntityFrameworkCore.Migrations;

namespace PayAway.WebAPI.Migrations
{
    public partial class CorrectedIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DemoCustomers_MerchantID_DemoCustomerGuid",
                table: "DemoCustomers");

            migrationBuilder.CreateIndex(
                name: "IX_DemoCustomers_DemoCustomerGuid",
                table: "DemoCustomers",
                column: "DemoCustomerGuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DemoCustomers_DemoCustomerGuid",
                table: "DemoCustomers");

            migrationBuilder.CreateIndex(
                name: "IX_DemoCustomers_MerchantID_DemoCustomerGuid",
                table: "DemoCustomers",
                columns: new[] { "MerchantID", "DemoCustomerGuid" },
                unique: true);
        }
    }
}
