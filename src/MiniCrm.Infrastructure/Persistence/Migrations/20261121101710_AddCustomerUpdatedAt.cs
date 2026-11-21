using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCrm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Customers",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Customers");
        }
    }
}
