using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBloodOxygenDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BloodOxygenDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AvgPercent = table.Column<double>(type: "REAL", nullable: false),
                    MinPercent = table.Column<double>(type: "REAL", nullable: false),
                    MaxPercent = table.Column<double>(type: "REAL", nullable: false),
                    MeasurementCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastImportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastImportBatchId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodOxygenDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodOxygenDay_ImportBatch_LastImportBatchId",
                        column: x => x.LastImportBatchId,
                        principalTable: "ImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodOxygenDay_LastImportBatchId",
                table: "BloodOxygenDay",
                column: "LastImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodOxygenDay_Source_Date",
                table: "BloodOxygenDay",
                columns: new[] { "Source", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodOxygenDay");
        }
    }
}
