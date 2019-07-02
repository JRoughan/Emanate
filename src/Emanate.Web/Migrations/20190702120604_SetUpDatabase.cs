using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emanate.Web.Migrations
{
    public partial class SetUpDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisplayDeviceType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Icon = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayDeviceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisplayDeviceProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayDeviceProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisplayDeviceProfiles_DisplayDeviceType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DisplayDeviceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisplayDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: true),
                    ProfileId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisplayDevices_DisplayDeviceProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "DisplayDeviceProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisplayDevices_DisplayDeviceType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DisplayDeviceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisplayDeviceProfiles_TypeId",
                table: "DisplayDeviceProfiles",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayDevices_ProfileId",
                table: "DisplayDevices",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayDevices_TypeId",
                table: "DisplayDevices",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisplayDevices");

            migrationBuilder.DropTable(
                name: "DisplayDeviceProfiles");

            migrationBuilder.DropTable(
                name: "DisplayDeviceType");
        }
    }
}
