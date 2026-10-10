using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedParameterCheckAddedMoreParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Fe",
                table: "ParameterChecks",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "MeasuredTime",
                table: "ParameterChecks",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Nh3",
                table: "ParameterChecks",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Po4",
                table: "ParameterChecks",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fe",
                table: "ParameterChecks");

            migrationBuilder.DropColumn(
                name: "MeasuredTime",
                table: "ParameterChecks");

            migrationBuilder.DropColumn(
                name: "Nh3",
                table: "ParameterChecks");

            migrationBuilder.DropColumn(
                name: "Po4",
                table: "ParameterChecks");
        }
    }
}
