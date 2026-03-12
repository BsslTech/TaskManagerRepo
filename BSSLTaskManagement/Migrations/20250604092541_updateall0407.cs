using Microsoft.EntityFrameworkCore.Migrations;

namespace BSSLTaskManagement.Migrations
{
    public partial class updateall0407 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "SubMenusetupTab",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "ModuleSetup",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "SubMenusetupTab");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "ModuleSetup");
        }
    }
}
