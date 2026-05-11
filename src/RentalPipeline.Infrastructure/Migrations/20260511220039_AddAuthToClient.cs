using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalPipeline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "clients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "clients",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_clients_email_unique",
                table: "clients",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_clients_email_unique",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "role",
                table: "clients");
        }
    }
}
