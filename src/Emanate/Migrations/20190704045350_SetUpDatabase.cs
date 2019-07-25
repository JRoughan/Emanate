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
                name: "SourceDeviceTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Icon = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceDeviceTypes", x => x.Id);
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
                name: "SourceDeviceProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceDeviceProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceDeviceProfiles_SourceDeviceTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SourceDeviceTypes",
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

            migrationBuilder.CreateTable(
                name: "SourceDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: true),
                    ProfileId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceDevices_SourceDeviceProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "SourceDeviceProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SourceDevices_SourceDeviceTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SourceDeviceTypes",
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

            migrationBuilder.CreateIndex(
                name: "IX_SourceDeviceProfiles_TypeId",
                table: "SourceDeviceProfiles",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceDevices_ProfileId",
                table: "SourceDevices",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceDevices_TypeId",
                table: "SourceDevices",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisplayDevices");

            migrationBuilder.DropTable(
                name: "SourceDevices");

            migrationBuilder.DropTable(
                name: "DisplayDeviceProfiles");

            migrationBuilder.DropTable(
                name: "SourceDeviceProfiles");

            migrationBuilder.DropTable(
                name: "DisplayDeviceType");

            migrationBuilder.DropTable(
                name: "SourceDeviceTypes");
        }
    }
}
