using Microsoft.EntityFrameworkCore.Migrations;

namespace BSSLTaskManagement.Migrations
{
    public partial class ordernumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderNo",
                table: "SystemDefTab",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "SystemDefTab");
        }
    }
}
