using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapStone.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailOtpLastSentAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailOtpLastSentAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailOtpLastSentAt",
                table: "Users");
        }
    }
}
