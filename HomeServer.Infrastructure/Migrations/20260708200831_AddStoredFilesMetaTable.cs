using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredFilesMetaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoredFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoredName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelativePath = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_OriginalName",
                table: "StoredFiles",
                column: "OriginalName");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_RelativePath",
                table: "StoredFiles",
                column: "RelativePath",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredFiles");
        }
    }
}
