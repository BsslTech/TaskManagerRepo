using Microsoft.EntityFrameworkCore.Migrations;

namespace BSSLTaskManagement.Migrations
{
    public partial class update203 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormNameHeader",
                table: "SubMenusetupTab",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormNameHeader",
                table: "SubMenusetupTab");
        }
    }
}
