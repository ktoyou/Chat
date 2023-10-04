using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96a52d29-e129-4642-83f8-63e6215c77d2"));

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "WithPassword",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConnectionId", "Name", "RoomId" },
                values: new object[] { new Guid("0f7f994e-48cb-4798-a74e-23ed1822c99d"), "none", "System", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0f7f994e-48cb-4798-a74e-23ed1822c99d"));

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "WithPassword",
                table: "Rooms");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConnectionId", "Name", "RoomId" },
                values: new object[] { new Guid("96a52d29-e129-4642-83f8-63e6215c77d2"), "none", "System", null });
        }
    }
}
