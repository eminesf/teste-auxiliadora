using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalPipeline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImmutableField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_immutable",
                table: "proposal_histories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "_immutable",
                table: "proposal_histories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
