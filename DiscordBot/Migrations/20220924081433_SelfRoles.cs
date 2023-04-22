using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBot.Migrations
{
    public partial class SelfRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleName",
                table: "AllowedRoles");

            migrationBuilder.AddColumn<decimal>(
                name: "RoleId",
                table: "AllowedRoles",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "SelfRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ServerConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelfRoles_ServerConfigs_ServerConfigId",
                        column: x => x.ServerConfigId,
                        principalTable: "ServerConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelfRoles_ServerConfigId",
                table: "SelfRoles",
                column: "ServerConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelfRoles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AllowedRoles");

            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                table: "AllowedRoles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
