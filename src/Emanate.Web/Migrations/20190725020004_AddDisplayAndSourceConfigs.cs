using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emanate.Web.Migrations
{
    public partial class AddDisplayAndSourceConfigs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisplayConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayDeviceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisplayConfigurations_DisplayDevices_DisplayDeviceId",
                        column: x => x.DisplayDeviceId,
                        principalTable: "DisplayDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SourceDeviceId = table.Column<Guid>(nullable: false),
                    DisplayConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceGroup_DisplayConfigurations_DisplayConfigurationId",
                        column: x => x.DisplayConfigurationId,
                        principalTable: "DisplayConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SourceGroup_SourceDevices_SourceDeviceId",
                        column: x => x.SourceDeviceId,
                        principalTable: "SourceDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Builds = table.Column<string>(nullable: true),
                    SourceGroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceConfiguration_SourceGroup_SourceGroupId",
                        column: x => x.SourceGroupId,
                        principalTable: "SourceGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisplayConfigurations_DisplayDeviceId",
                table: "DisplayConfigurations",
                column: "DisplayDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceConfiguration_SourceGroupId",
                table: "SourceConfiguration",
                column: "SourceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceGroup_DisplayConfigurationId",
                table: "SourceGroup",
                column: "DisplayConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceGroup_SourceDeviceId",
                table: "SourceGroup",
                column: "SourceDeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SourceConfiguration");

            migrationBuilder.DropTable(
                name: "SourceGroup");

            migrationBuilder.DropTable(
                name: "DisplayConfigurations");
        }
    }
}
