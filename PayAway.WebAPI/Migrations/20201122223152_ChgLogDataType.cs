using Microsoft.EntityFrameworkCore.Migrations;

namespace PayAway.WebAPI.Migrations
{
    public partial class ChgLogDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "Merchants",
                newName: "LogoFileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoFileName",
                table: "Merchants",
                newName: "LogoUrl");
        }
    }
}
