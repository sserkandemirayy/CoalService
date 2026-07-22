using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddCompanyBasedUserCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCompanies_CompanyId",
                table: "UserCompanies");

            migrationBuilder.AddColumn<string>(
                name: "UserCode",
                table: "UserCompanies",
                type: "character varying(8)",
                unicode: false,
                maxLength: 8,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyUserCounters",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastValue = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserCounters", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_CompanyUserCounters_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
WITH Numbered AS
(
    SELECT
        ""UserId"",
        ""CompanyId"",
        ROW_NUMBER() OVER
        (
            PARTITION BY ""CompanyId""
            ORDER BY ""CreatedAt"", ""UserId""
        ) AS Seq
    FROM ""UserCompanies""
)
UPDATE ""UserCompanies"" uc
SET ""UserCode"" = 'U' || LPAD(Numbered.Seq::text, 7, '0')
FROM Numbered
WHERE uc.""UserId"" = Numbered.""UserId""
  AND uc.""CompanyId"" = Numbered.""CompanyId"";
");

            migrationBuilder.Sql(@"
INSERT INTO ""CompanyUserCounters"" (""CompanyId"", ""LastValue"")
SELECT
    ""CompanyId"",
    COUNT(*)
FROM ""UserCompanies""
GROUP BY ""CompanyId"";
");

            migrationBuilder.AlterColumn<string>(
                name: "UserCode",
                table: "UserCompanies",
                type: "character varying(8)",
                unicode: false,
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldUnicode: false,
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_CompanyId_UserCode",
                table: "UserCompanies",
                columns: new[] { "CompanyId", "UserCode" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyUserCounters");

            migrationBuilder.DropIndex(
                name: "IX_UserCompanies_CompanyId_UserCode",
                table: "UserCompanies");

            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "UserCompanies");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_CompanyId",
                table: "UserCompanies",
                column: "CompanyId");
        }
    }
}