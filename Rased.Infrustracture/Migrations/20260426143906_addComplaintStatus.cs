using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rased.Infrustracture.Migrations
{
    /// <inheritdoc />
    public partial class addComplaintStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Complaints",
                newName: "complaintStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "complaintStatus",
                table: "Complaints",
                newName: "Status");
        }
    }
}
