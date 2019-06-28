using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emanate.Web.Migrations
{
    public partial class RenameDeviceProfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceProfiles");

            migrationBuilder.CreateTable(
                name: "DisplayDeviceProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayDeviceProfiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisplayDeviceProfiles");

            migrationBuilder.CreateTable(
                name: "DeviceProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceProfiles", x => x.Id);
                });
        }
    }
}
