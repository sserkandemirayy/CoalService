using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Icon = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShowOnMap = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentCategories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Model = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FloorMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    X = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    Y = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    Z = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    InstalledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastInspectionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextInspectionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Equipments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "EquipmentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_FloorMaps_FloorMapId",
                        column: x => x.FloorMapId,
                        principalTable: "FloorMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentInspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Result = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    NextInspectionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_EquipmentInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentInspections_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentInspections_Users_InspectedByUserId",
                        column: x => x.InspectedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategories_CompanyId",
                table: "EquipmentCategories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategories_CompanyId_Code",
                table: "EquipmentCategories",
                columns: new[] { "CompanyId", "Code" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategories_CompanyId_IsActive",
                table: "EquipmentCategories",
                columns: new[] { "CompanyId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCategories_CompanyId_ShowOnMap",
                table: "EquipmentCategories",
                columns: new[] { "CompanyId", "ShowOnMap" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInspections_EquipmentId",
                table: "EquipmentInspections",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInspections_EquipmentId_InspectedAt",
                table: "EquipmentInspections",
                columns: new[] { "EquipmentId", "InspectedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInspections_InspectedAt",
                table: "EquipmentInspections",
                column: "InspectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInspections_InspectedByUserId",
                table: "EquipmentInspections",
                column: "InspectedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInspections_Result",
                table: "EquipmentInspections",
                column: "Result");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_BranchId",
                table: "Equipments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_CategoryId",
                table: "Equipments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_CompanyId",
                table: "Equipments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_CompanyId_Code",
                table: "Equipments",
                columns: new[] { "CompanyId", "Code" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_ExpirationDate",
                table: "Equipments",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_FloorMapId",
                table: "Equipments",
                column: "FloorMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_FloorMapId_IsActive",
                table: "Equipments",
                columns: new[] { "FloorMapId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_IsActive",
                table: "Equipments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_NextInspectionAt",
                table: "Equipments",
                column: "NextInspectionAt");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_Status",
                table: "Equipments",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentInspections");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "EquipmentCategories");
        }
    }
}
