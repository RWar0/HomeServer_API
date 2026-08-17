using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAquariumIdForWaterChangeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoredFiles_Aquariums_AquariumId",
                table: "StoredFiles");

            migrationBuilder.CreateIndex(
                name: "IX_WaterChanges_ChangeDate",
                table: "WaterChanges",
                column: "ChangeDate");

            migrationBuilder.AddForeignKey(
                name: "FK_StoredFiles_Aquariums_AquariumId",
                table: "StoredFiles",
                column: "AquariumId",
                principalTable: "Aquariums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoredFiles_Aquariums_AquariumId",
                table: "StoredFiles");

            migrationBuilder.DropIndex(
                name: "IX_WaterChanges_ChangeDate",
                table: "WaterChanges");

            migrationBuilder.AddForeignKey(
                name: "FK_StoredFiles_Aquariums_AquariumId",
                table: "StoredFiles",
                column: "AquariumId",
                principalTable: "Aquariums",
                principalColumn: "Id");
        }
    }
}
