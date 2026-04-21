using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigReportedEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnchorConfigEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PositionJson = table.Column<string>(type: "jsonb", nullable: false),
                    NetworkJson = table.Column<string>(type: "jsonb", nullable: false),
                    UwbJson = table.Column<string>(type: "jsonb", nullable: false),
                    BleJson = table.Column<string>(type: "jsonb", nullable: false),
                    HeartbeatInterval = table.Column<long>(type: "bigint", nullable: false),
                    ReportInterval = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnchorConfigEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnchorConfigEvents_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnchorConfigEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnchorConfigSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastRawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PositionJson = table.Column<string>(type: "jsonb", nullable: false),
                    NetworkJson = table.Column<string>(type: "jsonb", nullable: false),
                    UwbJson = table.Column<string>(type: "jsonb", nullable: false),
                    BleJson = table.Column<string>(type: "jsonb", nullable: false),
                    HeartbeatInterval = table.Column<long>(type: "bigint", nullable: false),
                    ReportInterval = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnchorConfigSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnchorConfigSnapshots_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BleConfigEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    TxPower = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    AdvertisementInterval = table.Column<long>(type: "bigint", nullable: false),
                    MeshEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BleConfigEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BleConfigEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BleConfigEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DioConfigEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Pin = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PeriodicReportEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodicReportInterval = table.Column<long>(type: "bigint", nullable: false),
                    ReportOnChange = table.Column<bool>(type: "boolean", nullable: false),
                    LowPassFilterJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DioConfigEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DioConfigEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DioConfigEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "I2cConfigEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    ClockSpeed = table.Column<int>(type: "integer", nullable: false),
                    DevicesJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_I2cConfigEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_I2cConfigEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_I2cConfigEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagBleConfigSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastRawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    TxPower = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    AdvertisementInterval = table.Column<long>(type: "bigint", nullable: false),
                    MeshEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagBleConfigSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagBleConfigSnapshots_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagDioConfigSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    Pin = table.Column<int>(type: "integer", nullable: false),
                    LastRawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Direction = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PeriodicReportEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PeriodicReportInterval = table.Column<long>(type: "bigint", nullable: false),
                    ReportOnChange = table.Column<bool>(type: "boolean", nullable: false),
                    LowPassFilterJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagDioConfigSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagDioConfigSnapshots_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagI2cConfigSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastRawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    ClockSpeed = table.Column<int>(type: "integer", nullable: false),
                    DevicesJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagI2cConfigSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagI2cConfigSnapshots_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagUwbConfigSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastRawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    TxPower = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    RangingInterval = table.Column<long>(type: "bigint", nullable: false),
                    TagToTagEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TagToTagInterval = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagUwbConfigSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagUwbConfigSnapshots_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UwbConfigEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    TxPower = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    RangingInterval = table.Column<long>(type: "bigint", nullable: false),
                    TagToTagEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TagToTagInterval = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UwbConfigEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UwbConfigEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UwbConfigEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnchorConfigEvents_AnchorId",
                table: "AnchorConfigEvents",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorConfigEvents_AnchorId_EventTimestamp",
                table: "AnchorConfigEvents",
                columns: new[] { "AnchorId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AnchorConfigEvents_EventTimestamp",
                table: "AnchorConfigEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorConfigEvents_RawEventId",
                table: "AnchorConfigEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_AnchorConfigSnapshots_AnchorId",
                table: "AnchorConfigSnapshots",
                column: "AnchorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnchorConfigSnapshots_LastReportedAt",
                table: "AnchorConfigSnapshots",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BleConfigEvents_EventTimestamp",
                table: "BleConfigEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_BleConfigEvents_RawEventId",
                table: "BleConfigEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_BleConfigEvents_TagId",
                table: "BleConfigEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_BleConfigEvents_TagId_EventTimestamp",
                table: "BleConfigEvents",
                columns: new[] { "TagId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_DioConfigEvents_EventTimestamp",
                table: "DioConfigEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_DioConfigEvents_Pin",
                table: "DioConfigEvents",
                column: "Pin");

            migrationBuilder.CreateIndex(
                name: "IX_DioConfigEvents_RawEventId",
                table: "DioConfigEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_DioConfigEvents_TagId",
                table: "DioConfigEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_DioConfigEvents_TagId_Pin_EventTimestamp",
                table: "DioConfigEvents",
                columns: new[] { "TagId", "Pin", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_I2cConfigEvents_EventTimestamp",
                table: "I2cConfigEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_I2cConfigEvents_RawEventId",
                table: "I2cConfigEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_I2cConfigEvents_TagId",
                table: "I2cConfigEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_I2cConfigEvents_TagId_EventTimestamp",
                table: "I2cConfigEvents",
                columns: new[] { "TagId", "EventTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_TagBleConfigSnapshots_LastReportedAt",
                table: "TagBleConfigSnapshots",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TagBleConfigSnapshots_TagId",
                table: "TagBleConfigSnapshots",
                column: "TagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagDioConfigSnapshots_LastReportedAt",
                table: "TagDioConfigSnapshots",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TagDioConfigSnapshots_TagId_Pin",
                table: "TagDioConfigSnapshots",
                columns: new[] { "TagId", "Pin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagI2cConfigSnapshots_LastReportedAt",
                table: "TagI2cConfigSnapshots",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TagI2cConfigSnapshots_TagId",
                table: "TagI2cConfigSnapshots",
                column: "TagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagUwbConfigSnapshots_LastReportedAt",
                table: "TagUwbConfigSnapshots",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TagUwbConfigSnapshots_TagId",
                table: "TagUwbConfigSnapshots",
                column: "TagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UwbConfigEvents_EventTimestamp",
                table: "UwbConfigEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_UwbConfigEvents_RawEventId",
                table: "UwbConfigEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_UwbConfigEvents_TagId",
                table: "UwbConfigEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_UwbConfigEvents_TagId_EventTimestamp",
                table: "UwbConfigEvents",
                columns: new[] { "TagId", "EventTimestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnchorConfigEvents");

            migrationBuilder.DropTable(
                name: "AnchorConfigSnapshots");

            migrationBuilder.DropTable(
                name: "BleConfigEvents");

            migrationBuilder.DropTable(
                name: "DioConfigEvents");

            migrationBuilder.DropTable(
                name: "I2cConfigEvents");

            migrationBuilder.DropTable(
                name: "TagBleConfigSnapshots");

            migrationBuilder.DropTable(
                name: "TagDioConfigSnapshots");

            migrationBuilder.DropTable(
                name: "TagI2cConfigSnapshots");

            migrationBuilder.DropTable(
                name: "TagUwbConfigSnapshots");

            migrationBuilder.DropTable(
                name: "UwbConfigEvents");
        }
    }
}
