using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyAndBranchToTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BranchId",
                table: "Tags",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BranchId_Status",
                table: "Tags",
                columns: new[] { "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CompanyId",
                table: "Tags",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CompanyId_Status",
                table: "Tags",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagType",
                table: "Tags",
                column: "TagType");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Branches_BranchId",
                table: "Tags",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Companies_CompanyId",
                table: "Tags",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Branches_BranchId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Companies_CompanyId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_BranchId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_BranchId_Status",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CompanyId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CompanyId_Status",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagType",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Tags");
        }
    }
}
