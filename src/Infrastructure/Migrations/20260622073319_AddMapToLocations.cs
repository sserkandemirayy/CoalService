using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMapToLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FloorMapId",
                table: "LocationEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FloorMapZoneId",
                table: "LocationEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FloorMapId",
                table: "CurrentLocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FloorMapZoneId",
                table: "CurrentLocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationEvents_FloorMapId",
                table: "LocationEvents",
                column: "FloorMapId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationEvents_FloorMapZoneId",
                table: "LocationEvents",
                column: "FloorMapZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentLocations_FloorMapId",
                table: "CurrentLocations",
                column: "FloorMapId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentLocations_FloorMapZoneId",
                table: "CurrentLocations",
                column: "FloorMapZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrentLocations_FloorMapZones_FloorMapZoneId",
                table: "CurrentLocations",
                column: "FloorMapZoneId",
                principalTable: "FloorMapZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrentLocations_FloorMaps_FloorMapId",
                table: "CurrentLocations",
                column: "FloorMapId",
                principalTable: "FloorMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationEvents_FloorMapZones_FloorMapZoneId",
                table: "LocationEvents",
                column: "FloorMapZoneId",
                principalTable: "FloorMapZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationEvents_FloorMaps_FloorMapId",
                table: "LocationEvents",
                column: "FloorMapId",
                principalTable: "FloorMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrentLocations_FloorMapZones_FloorMapZoneId",
                table: "CurrentLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrentLocations_FloorMaps_FloorMapId",
                table: "CurrentLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationEvents_FloorMapZones_FloorMapZoneId",
                table: "LocationEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationEvents_FloorMaps_FloorMapId",
                table: "LocationEvents");

            migrationBuilder.DropIndex(
                name: "IX_LocationEvents_FloorMapId",
                table: "LocationEvents");

            migrationBuilder.DropIndex(
                name: "IX_LocationEvents_FloorMapZoneId",
                table: "LocationEvents");

            migrationBuilder.DropIndex(
                name: "IX_CurrentLocations_FloorMapId",
                table: "CurrentLocations");

            migrationBuilder.DropIndex(
                name: "IX_CurrentLocations_FloorMapZoneId",
                table: "CurrentLocations");

            migrationBuilder.DropColumn(
                name: "FloorMapId",
                table: "LocationEvents");

            migrationBuilder.DropColumn(
                name: "FloorMapZoneId",
                table: "LocationEvents");

            migrationBuilder.DropColumn(
                name: "FloorMapId",
                table: "CurrentLocations");

            migrationBuilder.DropColumn(
                name: "FloorMapZoneId",
                table: "CurrentLocations");
        }
    }
}
