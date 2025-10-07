using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VneOcherediGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeChatIdType2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "ChatIds",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeChat",
                table: "ChatIds",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "ChatIds");

            migrationBuilder.DropColumn(
                name: "TypeChat",
                table: "ChatIds");
        }
    }
}
