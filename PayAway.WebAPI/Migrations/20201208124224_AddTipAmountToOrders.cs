using Microsoft.EntityFrameworkCore.Migrations;

namespace PayAway.WebAPI.Migrations
{
    public partial class AddTipAmountToOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TipAmount",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipAmount",
                table: "Orders");
        }
    }
}
