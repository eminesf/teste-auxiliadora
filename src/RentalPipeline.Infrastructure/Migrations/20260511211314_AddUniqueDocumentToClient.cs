using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalPipeline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueDocumentToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_clients_document_unique",
                table: "clients",
                column: "document",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_clients_document_unique",
                table: "clients");
        }
    }
}
