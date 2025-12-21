using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Together.Migrations
{
    /// <inheritdoc />
    public partial class MinorFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumExperienceYears",
                table: "ProjectRequirements");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "CertificateTypes");

            migrationBuilder.RenameColumn(
                name: "IssuingAuthorityExample",
                table: "CertificateTypes",
                newName: "IssuingAuthority");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IssuingAuthority",
                table: "CertificateTypes",
                newName: "IssuingAuthorityExample");

            migrationBuilder.AddColumn<int>(
                name: "MinimumExperienceYears",
                table: "ProjectRequirements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Organizations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "CertificateTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
