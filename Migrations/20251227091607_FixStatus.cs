using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Together.Migrations
{
    /// <inheritdoc />
    public partial class FixStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Accounts_VerifiedByAdminId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_VerifiedByAdminId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "VerifiedByAdminId",
                table: "Certificates");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BlogPosts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Certificates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "Certificates",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerifiedByAdminId",
                table: "Certificates",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "BlogPosts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_VerifiedByAdminId",
                table: "Certificates",
                column: "VerifiedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Accounts_VerifiedByAdminId",
                table: "Certificates",
                column: "VerifiedByAdminId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
