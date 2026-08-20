using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMovementEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovementEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TagCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TagType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserFullName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UserCode = table.Column<string>(type: "character varying(8)", unicode: false, maxLength: 8, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    FloorMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    FloorMapZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    X = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Y = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Z = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Accuracy = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Confidence = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    RecordReason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementEvents", x => new { x.Id, x.EventTimestamp });
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_BranchId_EventTimestamp",
                table: "MovementEvents",
                columns: new[] { "BranchId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_CompanyId_EventTimestamp",
                table: "MovementEvents",
                columns: new[] { "CompanyId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_EventTimestamp",
                table: "MovementEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_FloorMapId_EventTimestamp",
                table: "MovementEvents",
                columns: new[] { "FloorMapId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_FloorMapZoneId_EventTimestamp",
                table: "MovementEvents",
                columns: new[] { "FloorMapZoneId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_Id",
                table: "MovementEvents",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_RawEventId",
                table: "MovementEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_TagId_EventTimestamp",
                table: "MovementEvents",
                columns: new[] { "TagId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_MovementEvents_UserId_EventTimestamp",
                table: "MovementEvents",
                columns: new[] { "UserId", "EventTimestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovementEvents");
        }
    }
}
