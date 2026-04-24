using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    Detail = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TaxNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    TaxOffice = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    WorkHours = table.Column<string>(type: "text", nullable: true),
                    Holidays = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DispatchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
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
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
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
                name: "UserTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    ManagerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "DioValueEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Pin = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DioValueEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DioValueEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DioValueEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "I2cDataEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Address = table.Column<int>(type: "integer", nullable: false),
                    Register = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Ack = table.Column<bool>(type: "boolean", nullable: false),
                    DataJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_I2cDataEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_I2cDataEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_I2cDataEvents_Tags_TagId",
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
                name: "TagDioValueSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    Pin = table.Column<int>(type: "integer", nullable: false),
                    LastRawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagDioValueSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagDioValueSnapshots_Tags_TagId",
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

            migrationBuilder.CreateTable(
                name: "UserSpecializations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSpecializations_UserTypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalTable: "UserTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    NationalIdEncrypted = table.Column<string>(type: "text", nullable: true),
                    PhoneEncrypted = table.Column<string>(type: "text", nullable: true),
                    AddressEncrypted = table.Column<string>(type: "text", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmergencyContactNameEncrypted = table.Column<string>(type: "text", nullable: true),
                    EmergencyContactPhoneEncrypted = table.Column<string>(type: "text", nullable: true),
                    KvkConsentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KvkConsentVersion = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserSpecializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "UserSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalTable: "UserTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "BleAdvertisementEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RawEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rssi = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BleAdvertisementEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BleAdvertisementEvents_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BleAdvertisementEvents_RawEvents_RawEventId",
                        column: x => x.RawEventId,
                        principalTable: "RawEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BleAdvertisementEvents_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
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
                name: "CommandRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommandType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: true),
                    AnchorId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QueuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExternalCorrelationId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CancelReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ResponseJson = table.Column<string>(type: "jsonb", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastRetriedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommandRequests_Anchors_AnchorId",
                        column: x => x.AnchorId,
                        principalTable: "Anchors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CommandRequests_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CommandRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
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
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Device = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
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

            migrationBuilder.CreateTable(
                name: "UserBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBranches_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBranches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCompanies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommandStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommandRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_CommandStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommandStatusHistories_CommandRequests_CommandRequestId",
                        column: x => x.CommandRequestId,
                        principalTable: "CommandRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommandStatusHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                name: "IX_BleAdvertisementEvents_AnchorId",
                table: "BleAdvertisementEvents",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_BleAdvertisementEvents_EventTimestamp",
                table: "BleAdvertisementEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_BleAdvertisementEvents_RawEventId",
                table: "BleAdvertisementEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_BleAdvertisementEvents_TagId",
                table: "BleAdvertisementEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_BleAdvertisementEvents_TagId_EventTimestamp",
                table: "BleAdvertisementEvents",
                columns: new[] { "TagId", "EventTimestamp" });

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
                name: "IX_Branches_CompanyId",
                table: "Branches",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRequests_AnchorId",
                table: "CommandRequests",
                column: "AnchorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRequests_CommandType",
                table: "CommandRequests",
                column: "CommandType");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRequests_RequestedAt",
                table: "CommandRequests",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRequests_RequestedByUserId",
                table: "CommandRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRequests_Status",
                table: "CommandRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRequests_Status_RequestedAt",
                table: "CommandRequests",
                columns: new[] { "Status", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CommandRequests_TagId",
                table: "CommandRequests",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRequests_TargetType",
                table: "CommandRequests",
                column: "TargetType");

            migrationBuilder.CreateIndex(
                name: "IX_CommandStatusHistories_ChangedAt",
                table: "CommandStatusHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CommandStatusHistories_ChangedByUserId",
                table: "CommandStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommandStatusHistories_CommandRequestId",
                table: "CommandStatusHistories",
                column: "CommandRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CommandStatusHistories_CommandRequestId_ChangedAt",
                table: "CommandStatusHistories",
                columns: new[] { "CommandRequestId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TaxNumber",
                table: "Companies",
                column: "TaxNumber",
                unique: true);

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
                name: "IX_DioValueEvents_EventTimestamp",
                table: "DioValueEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_DioValueEvents_Pin",
                table: "DioValueEvents",
                column: "Pin");

            migrationBuilder.CreateIndex(
                name: "IX_DioValueEvents_RawEventId",
                table: "DioValueEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_DioValueEvents_TagId",
                table: "DioValueEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_DioValueEvents_TagId_Pin_EventTimestamp",
                table: "DioValueEvents",
                columns: new[] { "TagId", "Pin", "EventTimestamp" });

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
                name: "IX_I2cDataEvents_EventTimestamp",
                table: "I2cDataEvents",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_I2cDataEvents_RawEventId",
                table: "I2cDataEvents",
                column: "RawEventId");

            migrationBuilder.CreateIndex(
                name: "IX_I2cDataEvents_TagId",
                table: "I2cDataEvents",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_I2cDataEvents_TagId_EventTimestamp",
                table: "I2cDataEvents",
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
                name: "IX_OutboxMessages_AggregateId",
                table: "OutboxMessages",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_AggregateType",
                table: "OutboxMessages",
                column: "AggregateType");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_MessageType",
                table: "OutboxMessages",
                column: "MessageType");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_OccurredAt",
                table: "OutboxMessages",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status_OccurredAt",
                table: "OutboxMessages",
                columns: new[] { "Status", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

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
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId");

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
                name: "IX_TagBleConfigSnapshots_LastReportedAt",
                table: "TagBleConfigSnapshots",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TagBleConfigSnapshots_TagId",
                table: "TagBleConfigSnapshots",
                column: "TagId",
                unique: true);

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
                name: "IX_TagDioConfigSnapshots_LastReportedAt",
                table: "TagDioConfigSnapshots",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TagDioConfigSnapshots_TagId_Pin",
                table: "TagDioConfigSnapshots",
                columns: new[] { "TagId", "Pin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagDioValueSnapshots_LastReportedAt",
                table: "TagDioValueSnapshots",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TagDioValueSnapshots_TagId_Pin",
                table: "TagDioValueSnapshots",
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
                name: "IX_UserBranches_BranchId",
                table: "UserBranches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_UserId_BranchId",
                table: "UserBranches",
                columns: new[] { "UserId", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_CompanyId",
                table: "UserCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_UserId_CompanyId",
                table: "UserCompanies",
                columns: new[] { "UserId", "CompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SpecializationId",
                table: "Users",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeId",
                table: "Users",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializations_Code",
                table: "UserSpecializations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecializations_UserTypeId",
                table: "UserSpecializations",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTypes_Code",
                table: "UserTypes",
                column: "Code",
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
                name: "Alarms");

            migrationBuilder.DropTable(
                name: "AnchorConfigEvents");

            migrationBuilder.DropTable(
                name: "AnchorConfigSnapshots");

            migrationBuilder.DropTable(
                name: "AnchorHealthEvents");

            migrationBuilder.DropTable(
                name: "AnchorHeartbeatEvents");

            migrationBuilder.DropTable(
                name: "AnchorStatusEvents");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BatteryEvents");

            migrationBuilder.DropTable(
                name: "BleAdvertisementEvents");

            migrationBuilder.DropTable(
                name: "BleConfigEvents");

            migrationBuilder.DropTable(
                name: "CommandStatusHistories");

            migrationBuilder.DropTable(
                name: "CurrentLocations");

            migrationBuilder.DropTable(
                name: "DioConfigEvents");

            migrationBuilder.DropTable(
                name: "DioValueEvents");

            migrationBuilder.DropTable(
                name: "EmergencyEvents");

            migrationBuilder.DropTable(
                name: "I2cConfigEvents");

            migrationBuilder.DropTable(
                name: "I2cDataEvents");

            migrationBuilder.DropTable(
                name: "ImuEvents");

            migrationBuilder.DropTable(
                name: "LocationEvents");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "ProximityEvents");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "TagAssignments");

            migrationBuilder.DropTable(
                name: "TagBleConfigSnapshots");

            migrationBuilder.DropTable(
                name: "TagDataEvents");

            migrationBuilder.DropTable(
                name: "TagDioConfigSnapshots");

            migrationBuilder.DropTable(
                name: "TagDioValueSnapshots");

            migrationBuilder.DropTable(
                name: "TagI2cConfigSnapshots");

            migrationBuilder.DropTable(
                name: "TagUwbConfigSnapshots");

            migrationBuilder.DropTable(
                name: "UserBranches");

            migrationBuilder.DropTable(
                name: "UserCompanies");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UwbConfigEvents");

            migrationBuilder.DropTable(
                name: "UwbRangingEvents");

            migrationBuilder.DropTable(
                name: "UwbTagToTagRangingEvents");

            migrationBuilder.DropTable(
                name: "CommandRequests");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "RawEvents");

            migrationBuilder.DropTable(
                name: "Anchors");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "UserSpecializations");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "UserTypes");
        }
    }
}
