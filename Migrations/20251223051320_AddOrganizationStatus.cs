using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Together.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Organizations");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Organizations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Organizations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Organizations");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Organizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
