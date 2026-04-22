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
                    ExportVersion = table.Column<string>(type: "TEXT", nullable: false),
                    ExportType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "HeartRateDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AvgBpm = table.Column<int>(type: "INTEGER", nullable: false),
                    MinBpm = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxBpm = table.Column<int>(type: "INTEGER", nullable: false),
                    MeasurementCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastImportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastImportBatchId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeartRateDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeartRateDay_ImportBatch_LastImportBatchId",
                        column: x => x.LastImportBatchId,
                        principalTable: "ImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SleepSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    LastImportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastImportBatchId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleepSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SleepSession_ImportBatch_LastImportBatchId",
                        column: x => x.LastImportBatchId,
                        principalTable: "ImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeartRateHourlyRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HeartRateDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    Hour = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AvgBpm = table.Column<int>(type: "INTEGER", nullable: false),
                    MinBpm = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxBpm = table.Column<int>(type: "INTEGER", nullable: false),
                    MeasurementCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeartRateHourlyRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeartRateHourlyRecord_HeartRateDay_HeartRateDayId",
                        column: x => x.HeartRateDayId,
                        principalTable: "HeartRateDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SleepStage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SleepSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Stage = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleepStage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SleepStage_SleepSession_SleepSessionId",
                        column: x => x.SleepSessionId,
                        principalTable: "SleepSession",
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

            migrationBuilder.CreateIndex(
                name: "IX_HeartRateDay_LastImportBatchId",
                table: "HeartRateDay",
                column: "LastImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_HeartRateDay_Source_Date",
                table: "HeartRateDay",
                columns: new[] { "Source", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeartRateHourlyRecord_HeartRateDayId_Hour",
                table: "HeartRateHourlyRecord",
                columns: new[] { "HeartRateDayId", "Hour" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SleepSession_LastImportBatchId",
                table: "SleepSession",
                column: "LastImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SleepSession_Source_StartTimeUtc",
                table: "SleepSession",
                columns: new[] { "Source", "StartTimeUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SleepStage_SleepSessionId",
                table: "SleepStage",
                column: "SleepSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityDay");

            migrationBuilder.DropTable(
                name: "HeartRateHourlyRecord");

            migrationBuilder.DropTable(
                name: "SleepStage");

            migrationBuilder.DropTable(
                name: "HeartRateDay");

            migrationBuilder.DropTable(
                name: "SleepSession");

            migrationBuilder.DropTable(
                name: "ImportBatch");
        }
    }
}
