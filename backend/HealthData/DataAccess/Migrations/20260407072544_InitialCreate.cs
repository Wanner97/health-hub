using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportBatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ExportVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    ExportedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ImportedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    RecordCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportBatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StepEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImportBatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepEntry_ImportBatch_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalTable: "ImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StepEntry_ImportBatchId",
                table: "StepEntry",
                column: "ImportBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepEntry");

            migrationBuilder.DropTable(
                name: "ImportBatch");
        }
    }
}
