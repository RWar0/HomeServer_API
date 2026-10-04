using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParameterChecksMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParameterChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ph = table.Column<double>(type: "double precision", nullable: true),
                    Kh = table.Column<double>(type: "double precision", nullable: true),
                    Gh = table.Column<double>(type: "double precision", nullable: true),
                    No3 = table.Column<double>(type: "double precision", nullable: true),
                    No2 = table.Column<double>(type: "double precision", nullable: true),
                    Temperature = table.Column<double>(type: "double precision", nullable: true),
                    MeasuredAt = table.Column<DateOnly>(type: "date", nullable: false),
                    AquariumId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParameterChecks_Aquariums_AquariumId",
                        column: x => x.AquariumId,
                        principalTable: "Aquariums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterChecks_AquariumId",
                table: "ParameterChecks",
                column: "AquariumId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterChecks_MeasuredAt",
                table: "ParameterChecks",
                column: "MeasuredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParameterChecks");
        }
    }
}
