using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSleepImportSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExportType",
                table: "ImportBatch",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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
                name: "SleepStage");

            migrationBuilder.DropTable(
                name: "SleepSession");

            migrationBuilder.DropColumn(
                name: "ExportType",
                table: "ImportBatch");
        }
    }
}
