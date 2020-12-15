using Microsoft.EntityFrameworkCore.Migrations;

namespace PayAway.WebAPI.Migrations
{
    public partial class MerchantUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MerchantUrl",
                table: "Merchants",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchantUrl",
                table: "Merchants");
        }
    }
}
