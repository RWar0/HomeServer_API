using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleServiceAndVehicleServiceItemEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: true),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleServices_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleServiceItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Cost = table.Column<decimal>(type: "numeric", nullable: true),
                    VehicleServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServiceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleServiceItem_VehicleServices_VehicleServiceId",
                        column: x => x.VehicleServiceId,
                        principalTable: "VehicleServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceItem_Title",
                table: "VehicleServiceItem",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceItem_VehicleServiceId",
                table: "VehicleServiceItem",
                column: "VehicleServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceItem_VehicleServiceId_Title",
                table: "VehicleServiceItem",
                columns: new[] { "VehicleServiceId", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServices_Date",
                table: "VehicleServices",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServices_VehicleId",
                table: "VehicleServices",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServices_VehicleId_Date",
                table: "VehicleServices",
                columns: new[] { "VehicleId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleServiceItem");

            migrationBuilder.DropTable(
                name: "VehicleServices");
        }
    }
}
