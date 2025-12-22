using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Together.Migrations
{
    /// <inheritdoc />
    public partial class FixApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationCertificate",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    CertificateId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationCertificate", x => new { x.ApplicationId, x.CertificateId });
                    table.ForeignKey(
                        name: "FK_ApplicationCertificate_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationCertificate_VolunteerApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "VolunteerApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationCertificate_CertificateId",
                table: "ApplicationCertificate",
                column: "CertificateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationCertificate");
        }
    }
}
