using Microsoft.EntityFrameworkCore.Migrations;

namespace BSSLTaskManagement.Migrations
{
    public partial class updatetask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemMenuTab",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 10, nullable: true),
                    Desc = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMenuTab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSubMenuTab",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemMenuTabId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 10, nullable: true),
                    Desc = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSubMenuTab", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSubMenuTab_SystemMenuTab_SystemMenuTabId",
                        column: x => x.SystemMenuTabId,
                        principalTable: "SystemMenuTab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemDefTab",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemSubMenuTabId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 10, nullable: true),
                    Desc = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemDefTab", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemDefTab_SystemSubMenuTab_SystemSubMenuTabId",
                        column: x => x.SystemSubMenuTabId,
                        principalTable: "SystemSubMenuTab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemDefTab_SystemSubMenuTabId",
                table: "SystemDefTab",
                column: "SystemSubMenuTabId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSubMenuTab_SystemMenuTabId",
                table: "SystemSubMenuTab",
                column: "SystemMenuTabId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemDefTab");

            migrationBuilder.DropTable(
                name: "SystemSubMenuTab");

            migrationBuilder.DropTable(
                name: "SystemMenuTab");
        }
    }
}
