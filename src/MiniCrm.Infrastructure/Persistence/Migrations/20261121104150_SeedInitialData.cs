using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstContactedAt", "IsActive", "Name", "UpdatedAt" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTimeOffset(new DateTime(2026, 11, 21, 10, 41, 49, 905, DateTimeKind.Unspecified).AddTicks(9413), new TimeSpan(0, 0, 0, 0, 0)), "contact@acme.test", null, true, "Acme Corp", null });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "CustomerId", "Type", "Value" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111"), "Email", "contact@acme.test" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
