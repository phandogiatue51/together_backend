using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Together.Migrations
{
    /// <inheritdoc />
    public partial class AddProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Activities",
                table: "Projects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "Projects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Challenges",
                table: "Projects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Goals",
                table: "Projects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Impacts",
                table: "Projects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "Projects",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activities",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Challenges",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Goals",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Impacts",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "Projects");
        }
    }
}
