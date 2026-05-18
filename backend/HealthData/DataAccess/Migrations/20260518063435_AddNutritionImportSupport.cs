using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionImportSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NutritionDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    RecordCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastCalculatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastImportBatchId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionDay_ImportBatch_LastImportBatchId",
                        column: x => x.LastImportBatchId,
                        principalTable: "ImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionDayNutrientTotal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NutritionDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    NutrientKey = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    TotalAmount = table.Column<double>(type: "REAL", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionDayNutrientTotal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionDayNutrientTotal_NutritionDay_NutritionDayId",
                        column: x => x.NutritionDayId,
                        principalTable: "NutritionDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionMealTypeSummary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NutritionDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    MealType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecordCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionMealTypeSummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionMealTypeSummary_NutritionDay_NutritionDayId",
                        column: x => x.NutritionDayId,
                        principalTable: "NutritionDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NutritionDayId = table.Column<int>(type: "INTEGER", nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    HealthConnectRecordId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MealType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LastImportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastImportBatchId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionRecord_ImportBatch_LastImportBatchId",
                        column: x => x.LastImportBatchId,
                        principalTable: "ImportBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NutritionRecord_NutritionDay_NutritionDayId",
                        column: x => x.NutritionDayId,
                        principalTable: "NutritionDay",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NutritionMealTypeNutrientTotal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NutritionMealTypeSummaryId = table.Column<int>(type: "INTEGER", nullable: false),
                    NutrientKey = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    TotalAmount = table.Column<double>(type: "REAL", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionMealTypeNutrientTotal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionMealTypeNutrientTotal_NutritionMealTypeSummary_NutritionMealTypeSummaryId",
                        column: x => x.NutritionMealTypeSummaryId,
                        principalTable: "NutritionMealTypeSummary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionRecordNutrient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NutritionRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    NutrientKey = table.Column<string>(type: "TEXT", unicode: false, maxLength: 100, nullable: false),
                    Amount = table.Column<double>(type: "REAL", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionRecordNutrient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionRecordNutrient_NutritionRecord_NutritionRecordId",
                        column: x => x.NutritionRecordId,
                        principalTable: "NutritionRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NutritionDay_LastImportBatchId",
                table: "NutritionDay",
                column: "LastImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionDay_Source_Date",
                table: "NutritionDay",
                columns: new[] { "Source", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionDayNutrientTotal_NutritionDayId_NutrientKey",
                table: "NutritionDayNutrientTotal",
                columns: new[] { "NutritionDayId", "NutrientKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionMealTypeNutrientTotal_NutritionMealTypeSummaryId_NutrientKey",
                table: "NutritionMealTypeNutrientTotal",
                columns: new[] { "NutritionMealTypeSummaryId", "NutrientKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionMealTypeSummary_NutritionDayId_MealType",
                table: "NutritionMealTypeSummary",
                columns: new[] { "NutritionDayId", "MealType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionRecord_LastImportBatchId",
                table: "NutritionRecord",
                column: "LastImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionRecord_NutritionDayId",
                table: "NutritionRecord",
                column: "NutritionDayId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionRecord_Source_Date",
                table: "NutritionRecord",
                columns: new[] { "Source", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_NutritionRecord_Source_HealthConnectRecordId",
                table: "NutritionRecord",
                columns: new[] { "Source", "HealthConnectRecordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionRecordNutrient_NutritionRecordId_NutrientKey",
                table: "NutritionRecordNutrient",
                columns: new[] { "NutritionRecordId", "NutrientKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NutritionDayNutrientTotal");

            migrationBuilder.DropTable(
                name: "NutritionMealTypeNutrientTotal");

            migrationBuilder.DropTable(
                name: "NutritionRecordNutrient");

            migrationBuilder.DropTable(
                name: "NutritionMealTypeSummary");

            migrationBuilder.DropTable(
                name: "NutritionRecord");

            migrationBuilder.DropTable(
                name: "NutritionDay");
        }
    }
}
