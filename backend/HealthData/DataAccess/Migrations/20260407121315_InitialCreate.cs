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
                    ExportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RangeStartUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RangeEndUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceivedRecordCount = table.Column<int>(type: "INTEGER", nullable: false),
                    InsertedRecordCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedRecordCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UnchangedRecordCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ExportVersion = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportBatch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Steps = table.Column<int>(type: "INTEGER", nullable: false),
                    DistanceMeters = table.Column<double>(type: "REAL", nullable: false),
                    LastImportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastImportBatchId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityDay_ImportBatch_LastImportBatchId",
                        column: x => x.LastImportBatchId,
                        principalTable: "ImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDay_LastImportBatchId",
                table: "ActivityDay",
                column: "LastImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDay_Source_Date",
                table: "ActivityDay",
                columns: new[] { "Source", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityDay");

            migrationBuilder.DropTable(
                name: "ImportBatch");
        }
    }
}
