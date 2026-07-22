using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddCompanyBasedUserCode2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
UPDATE ""CompanyUserCounters"" c
SET ""LastValue"" = x.""LastValue""
FROM
(
    SELECT
        ""CompanyId"",
        COUNT(*)::bigint AS ""LastValue""
    FROM ""UserCompanies""
    GROUP BY ""CompanyId""
) x
WHERE c.""CompanyId"" = x.""CompanyId"";
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserCode",
                table: "UserCompanies",
                type: "character varying(8)",
                unicode: false,
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldUnicode: false,
                oldMaxLength: 8);
        }
    }
}