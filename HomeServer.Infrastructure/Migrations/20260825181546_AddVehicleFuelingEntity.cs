using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleFuelingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleFuelings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleFuelings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleFuelings_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelings_Date",
                table: "VehicleFuelings",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelings_VehicleId",
                table: "VehicleFuelings",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleFuelings");
        }
    }
}
