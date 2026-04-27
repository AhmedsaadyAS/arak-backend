using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Messages",
                newName: "SentAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Messages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Messages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Messages",
                newName: "Timestamp");
        }
    }
}
