using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Together.Migrations
{
    /// <inheritdoc />
    public partial class FixBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Accounts_AuthorId",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Accounts_AuthorId",
                table: "BlogPosts",
                column: "AuthorId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Accounts_AuthorId",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "BlogPosts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Accounts_AuthorId",
                table: "BlogPosts",
                column: "AuthorId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
