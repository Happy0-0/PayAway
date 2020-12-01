using Microsoft.EntityFrameworkCore.Migrations;

namespace PayAway.WebAPI.Migrations
{
    public partial class NewPaymentColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpMonth",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpYear",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpMonth",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExpYear",
                table: "Orders");
        }
    }
}
