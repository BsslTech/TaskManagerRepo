using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BSSLTaskManagement.Migrations
{
    public partial class update_02_07_2024 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
              name: "LicenseTab",
              columns: table => new
              {
                  Id = table.Column<int>(nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  CompCode = table.Column<string>(maxLength: 10, nullable: true),
                  ClientName = table.Column<string>(nullable: true),
                  LicenseType = table.Column<string>(nullable: true),
                  LicenseNo = table.Column<int>(nullable: true),
                  GraceDays = table.Column<int>(nullable: true),
                  DateFrm = table.Column<DateTime>(nullable: true),
                  DateTo = table.Column<DateTime>(nullable: true),
                  SystemType = table.Column<string>(nullable: true),
                  SystemTypeDescr = table.Column<string>(nullable: true),
                  Deactivate = table.Column<bool>(nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_LicenseTab", x => x.Id);
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
         

            migrationBuilder.DropTable(
                name: "LicenseTab");

        }
    }
}
