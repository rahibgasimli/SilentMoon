using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlientMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTrackImageAndNumber_RequireNarrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Narrators_NarratorId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "TrackNumber",
                table: "Tracks");

            migrationBuilder.AlterColumn<string>(
                name: "NarratorId",
                table: "Tracks",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 31, 8, 14, 39, 324, DateTimeKind.Utc).AddTicks(7873));

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                column: "CreatedAt",
                value: new DateTime(2026, 8, 31, 8, 14, 39, 324, DateTimeKind.Utc).AddTicks(7881));

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Narrators_NarratorId",
                table: "Tracks",
                column: "NarratorId",
                principalTable: "Narrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Narrators_NarratorId",
                table: "Tracks");

            migrationBuilder.AlterColumn<string>(
                name: "NarratorId",
                table: "Tracks",
                type: "NVARCHAR2(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Tracks",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrackNumber",
                table: "Tracks",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Narrators_NarratorId",
                table: "Tracks",
                column: "NarratorId",
                principalTable: "Narrators",
                principalColumn: "Id");
        }
    }
}
