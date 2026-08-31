using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlientMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserRoleAddAdminTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 29, 8, 36, 54, 354, DateTimeKind.Utc).AddTicks(3781));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 29, 8, 36, 54, 354, DateTimeKind.Utc).AddTicks(3788));

            migrationBuilder.CreateIndex(
                name: "IX_Admins_Email",
                table: "Admins",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 11, 18, 15, 595, DateTimeKind.Utc).AddTicks(7500));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 11, 18, 15, 595, DateTimeKind.Utc).AddTicks(7509));
        }
    }
}
