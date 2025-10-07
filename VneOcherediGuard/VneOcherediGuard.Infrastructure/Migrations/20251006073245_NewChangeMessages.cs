using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VneOcherediGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewChangeMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeploymentEnvironment",
                table: "MessagesTime",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeploymentEnvironment",
                table: "MessagesTime");
        }
    }
}
