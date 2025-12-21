using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Together.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCertiType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_CertificateTypes_CertificateTypeId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCategories_Categories_CategoryId",
                table: "ProjectCategories");

            migrationBuilder.DropTable(
                name: "ProjectRequirements");

            migrationBuilder.DropTable(
                name: "CertificateTypes");

            migrationBuilder.RenameColumn(
                name: "CertificateTypeId",
                table: "Certificates",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificates_CertificateTypeId",
                table: "Certificates",
                newName: "IX_Certificates_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Categories_CategoryId",
                table: "Certificates",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCategories_Categories_CategoryId",
                table: "ProjectCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Categories_CategoryId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCategories_Categories_CategoryId",
                table: "ProjectCategories");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Certificates",
                newName: "CertificateTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificates_CategoryId",
                table: "Certificates",
                newName: "IX_Certificates_CertificateTypeId");

            migrationBuilder.CreateTable(
                name: "CertificateTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IssuingAuthority = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateTypes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateTypeId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Level = table.Column<int>(type: "integer", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRequirements_CertificateTypes_CertificateTypeId",
                        column: x => x.CertificateTypeId,
                        principalTable: "CertificateTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectRequirements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTypes_CategoryId",
                table: "CertificateTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRequirements_CertificateTypeId",
                table: "ProjectRequirements",
                column: "CertificateTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRequirements_ProjectId",
                table: "ProjectRequirements",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_CertificateTypes_CertificateTypeId",
                table: "Certificates",
                column: "CertificateTypeId",
                principalTable: "CertificateTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCategories_Categories_CategoryId",
                table: "ProjectCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
