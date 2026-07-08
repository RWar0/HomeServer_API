using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAquariumBaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AquariumId",
                table: "StoredFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Aquariums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aquariums", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_AquariumId",
                table: "StoredFiles",
                column: "AquariumId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoredFiles_Aquariums_AquariumId",
                table: "StoredFiles",
                column: "AquariumId",
                principalTable: "Aquariums",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoredFiles_Aquariums_AquariumId",
                table: "StoredFiles");

            migrationBuilder.DropTable(
                name: "Aquariums");

            migrationBuilder.DropIndex(
                name: "IX_StoredFiles_AquariumId",
                table: "StoredFiles");

            migrationBuilder.DropColumn(
                name: "AquariumId",
                table: "StoredFiles");
        }
    }
}
