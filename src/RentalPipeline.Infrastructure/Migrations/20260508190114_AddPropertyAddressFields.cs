using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalPipeline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "properties");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "properties",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "complement",
                table: "properties",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "district",
                table: "properties",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "number",
                table: "properties",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "properties",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "properties",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "city",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "complement",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "district",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "number",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "state",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "street",
                table: "properties");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "properties",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
