using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalPipeline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                table: "properties",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_proposals_client_id",
                table: "proposals",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_properties_owner_id",
                table: "properties",
                column: "owner_id");

            migrationBuilder.AddForeignKey(
                name: "FK_properties_clients_owner_id",
                table: "properties",
                column: "owner_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_proposals_clients_client_id",
                table: "proposals",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_properties_clients_owner_id",
                table: "properties");

            migrationBuilder.DropForeignKey(
                name: "FK_proposals_clients_client_id",
                table: "proposals");

            migrationBuilder.DropIndex(
                name: "IX_proposals_client_id",
                table: "proposals");

            migrationBuilder.DropIndex(
                name: "IX_properties_owner_id",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "properties");
        }
    }
}
