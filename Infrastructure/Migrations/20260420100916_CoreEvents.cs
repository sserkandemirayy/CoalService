using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CoreEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anchors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastHeartbeatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastStatusChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anchors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anchors_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Anchors_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RawEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TagExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AnchorExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TagType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    BatteryLevel = table.Column<int>(type: "integer", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastEventAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnchorHeartbeatEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnchorHeartbeatEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnchorHeartbeatEvents_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnchorHeartbeatEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnchorStatusEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PreviousStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnchorStatusEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnchorStatusEvents_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnchorStatusEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alarms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    AlarmType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: true),
                    PeerTagId = table.Column<Guid>(type: "uuid", nullable: true),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AcknowledgedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DataJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alarms_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Alarms_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Alarms_Tags_PeerTagId",
                        column: x => x.PeerTagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Alarms_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Alarms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BatteryEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BatteryLevel = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatteryEvents_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatteryEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatteryEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrentLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    X = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Y = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Z = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Accuracy = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Confidence = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    LastEventAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastRawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastKnownAnchorCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrentLocations_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrentLocations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmergencyEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImuEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImuEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImuEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImuEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    X = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Y = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Z = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Accuracy = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Confidence = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    UsedAnchorsJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProximityEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeerTagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Distance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Threshold = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProximityEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProximityEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProximityEvents_Tags_PeerTagId",
                        column: x => x.PeerTagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProximityEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnassignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagAssignments_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_AlarmType",
                table: "Alarms",
                column: "AlarmType");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_AnchorId",
                table: "Alarms",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_PeerTagId",
                table: "Alarms",
                column: "PeerTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_RawEventId",
                table: "Alarms",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_Severity",
                table: "Alarms",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_StartedAt",
                table: "Alarms",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_Status",
                table: "Alarms",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_Status_StartedAt",
                table: "Alarms",
                columns: new[] { "Status", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_TagId",
                table: "Alarms",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_UserId",
                table: "Alarms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorHeartbeatEvents_AnchorId",
                table: "AnchorHeartbeatEvents",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorHeartbeatEvents_AnchorId_EventTimestamp",
                table: "AnchorHeartbeatEvents",
                columns: new[] { "AnchorId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AnchorHeartbeatEvents_EventTimestamp",
                table: "AnchorHeartbeatEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorHeartbeatEvents_RawEventId",
                table: "AnchorHeartbeatEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Anchors_BranchId",
                table: "Anchors",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Anchors_Code",
                table: "Anchors",
                column: "Code",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Anchors_CompanyId",
                table: "Anchors",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Anchors_ExternalId",
                table: "Anchors",
                column: "ExternalId",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Anchors_IpAddress",
                table: "Anchors",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Anchors_LastHeartbeatAt",
                table: "Anchors",
                column: "LastHeartbeatAt");

            migrationBuilder.CreateIndex(
                name: "IX_Anchors_LastStatusChangedAt",
                table: "Anchors",
                column: "LastStatusChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Anchors_Status",
                table: "Anchors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorStatusEvents_AnchorId",
                table: "AnchorStatusEvents",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorStatusEvents_EventTimestamp",
                table: "AnchorStatusEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorStatusEvents_RawEventId",
                table: "AnchorStatusEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorStatusEvents_Status",
                table: "AnchorStatusEvents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryEvents_AnchorId",
                table: "BatteryEvents",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryEvents_EventTimestamp",
                table: "BatteryEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryEvents_RawEventId",
                table: "BatteryEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryEvents_TagId",
                table: "BatteryEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryEvents_TagId_EventTimestamp",
                table: "BatteryEvents",
                columns: new[] { "TagId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_CurrentLocations_LastEventAt",
                table: "CurrentLocations",
                column: "LastEventAt");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentLocations_TagId",
                table: "CurrentLocations",
                column: "TagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrentLocations_UserId",
                table: "CurrentLocations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyEvents_EventTimestamp",
                table: "EmergencyEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyEvents_RawEventId",
                table: "EmergencyEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyEvents_TagId",
                table: "EmergencyEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyEvents_TagId_EventTimestamp",
                table: "EmergencyEvents",
                columns: new[] { "TagId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ImuEvents_EventTimestamp",
                table: "ImuEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ImuEvents_EventType",
                table: "ImuEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_ImuEvents_RawEventId",
                table: "ImuEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ImuEvents_TagId",
                table: "ImuEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationEvents_EventTimestamp",
                table: "LocationEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_LocationEvents_RawEventId",
                table: "LocationEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationEvents_TagId",
                table: "LocationEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationEvents_TagId_EventTimestamp",
                table: "LocationEvents",
                columns: new[] { "TagId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ProximityEvents_EventTimestamp",
                table: "ProximityEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ProximityEvents_PeerTagId",
                table: "ProximityEvents",
                column: "PeerTagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProximityEvents_RawEventId",
                table: "ProximityEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ProximityEvents_Severity",
                table: "ProximityEvents",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_ProximityEvents_TagId",
                table: "ProximityEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_RawEvents_AnchorExternalId",
                table: "RawEvents",
                column: "AnchorExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_RawEvents_EventTimestamp",
                table: "RawEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_RawEvents_EventType",
                table: "RawEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_RawEvents_ExternalEventId",
                table: "RawEvents",
                column: "ExternalEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RawEvents_ProcessingStatus",
                table: "RawEvents",
                column: "ProcessingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_RawEvents_ReceivedAt",
                table: "RawEvents",
                column: "ReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RawEvents_TagExternalId",
                table: "RawEvents",
                column: "TagExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_TagAssignments_TagId",
                table: "TagAssignments",
                column: "TagId",
                unique: true,
                filter: "\"UnassignedAt\" IS NULL AND \"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TagAssignments_TagId_UserId_AssignedAt",
                table: "TagAssignments",
                columns: new[] { "TagId", "UserId", "AssignedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TagAssignments_UserId",
                table: "TagAssignments",
                column: "UserId",
                filter: "\"UnassignedAt\" IS NULL AND \"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Code",
                table: "Tags",
                column: "Code",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ExternalId",
                table: "Tags",
                column: "ExternalId",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_LastEventAt",
                table: "Tags",
                column: "LastEventAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_LastSeenAt",
                table: "Tags",
                column: "LastSeenAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Status",
                table: "Tags",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alarms");

            migrationBuilder.DropTable(
                name: "AnchorHeartbeatEvents");

            migrationBuilder.DropTable(
                name: "AnchorStatusEvents");

            migrationBuilder.DropTable(
                name: "BatteryEvents");

            migrationBuilder.DropTable(
                name: "CurrentLocations");

            migrationBuilder.DropTable(
                name: "EmergencyEvents");

            migrationBuilder.DropTable(
                name: "ImuEvents");

            migrationBuilder.DropTable(
                name: "LocationEvents");

            migrationBuilder.DropTable(
                name: "ProximityEvents");

            migrationBuilder.DropTable(
                name: "TagAssignments");

            migrationBuilder.DropTable(
                name: "Anchors");

            migrationBuilder.DropTable(
                name: "RawEvents");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
