using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rased.Infrustracture.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoriesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Categories_CategoryId",
                table: "Complaints");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_CategoryId",
                table: "Complaints");
            */

            /*
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Complaints");
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Complaints",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "General Complaint" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Infrastructure" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Safety" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CategoryId",
                table: "Complaints",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Categories_CategoryId",
                table: "Complaints",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
