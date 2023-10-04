using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0f7f994e-48cb-4798-a74e-23ed1822c99d"));

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConnectionId", "Name", "Password", "RoomId" },
                values: new object[] { new Guid("d142fa11-328d-4766-978b-5ddbfa16f0a6"), "none", "System", "77a48be5-6c86-4a1a-8919-fc33993bc504", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d142fa11-328d-4766-978b-5ddbfa16f0a6"));

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConnectionId", "Name", "RoomId" },
                values: new object[] { new Guid("0f7f994e-48cb-4798-a74e-23ed1822c99d"), "none", "System", null });
        }
    }
}
