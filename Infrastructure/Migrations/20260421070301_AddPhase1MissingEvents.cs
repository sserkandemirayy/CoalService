using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase1MissingEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnchorHealthEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Uptime = table.Column<long>(type: "bigint", nullable: false),
                    Temperature = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CpuUsage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MemoryUsage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TagCount = table.Column<int>(type: "integer", nullable: false),
                    PacketLossRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnchorHealthEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnchorHealthEvents_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnchorHealthEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagDataEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReportedTagType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagDataEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagDataEvents_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagDataEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagDataEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UwbRangingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Distance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UwbRangingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UwbRangingEvents_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UwbRangingEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UwbRangingEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UwbTagToTagRangingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeerTagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Distance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UwbTagToTagRangingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UwbTagToTagRangingEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UwbTagToTagRangingEvents_Tags_PeerTagId",
                        column: x => x.PeerTagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UwbTagToTagRangingEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnchorHealthEvents_AnchorId",
                table: "AnchorHealthEvents",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorHealthEvents_AnchorId_EventTimestamp",
                table: "AnchorHealthEvents",
                columns: new[] { "AnchorId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AnchorHealthEvents_EventTimestamp",
                table: "AnchorHealthEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorHealthEvents_RawEventId",
                table: "AnchorHealthEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TagDataEvents_AnchorId",
                table: "TagDataEvents",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_TagDataEvents_EventTimestamp",
                table: "TagDataEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_TagDataEvents_RawEventId",
                table: "TagDataEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TagDataEvents_TagId",
                table: "TagDataEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagDataEvents_TagId_EventTimestamp",
                table: "TagDataEvents",
                columns: new[] { "TagId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_UwbRangingEvents_AnchorId",
                table: "UwbRangingEvents",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_UwbRangingEvents_EventTimestamp",
                table: "UwbRangingEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_UwbRangingEvents_RawEventId",
                table: "UwbRangingEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_UwbRangingEvents_TagId",
                table: "UwbRangingEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_UwbRangingEvents_TagId_EventTimestamp",
                table: "UwbRangingEvents",
                columns: new[] { "TagId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_UwbTagToTagRangingEvents_EventTimestamp",
                table: "UwbTagToTagRangingEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_UwbTagToTagRangingEvents_PeerTagId",
                table: "UwbTagToTagRangingEvents",
                column: "PeerTagId");

            migrationBuilder.CreateIndex(
                name: "IX_UwbTagToTagRangingEvents_RawEventId",
                table: "UwbTagToTagRangingEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_UwbTagToTagRangingEvents_TagId",
                table: "UwbTagToTagRangingEvents",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnchorHealthEvents");

            migrationBuilder.DropTable(
                name: "TagDataEvents");

            migrationBuilder.DropTable(
                name: "UwbRangingEvents");

            migrationBuilder.DropTable(
                name: "UwbTagToTagRangingEvents");
        }
    }
}
