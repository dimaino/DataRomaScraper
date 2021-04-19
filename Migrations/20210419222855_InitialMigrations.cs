﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataRomaScraper.Migrations
{
    public partial class InitialMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companys",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    NumberOfStocks = table.Column<int>(type: "int", nullable: false),
                    PortfolioValue = table.Column<double>(type: "double", nullable: false),
                    HoldingURL = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    DateRecorded = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    DatePulled = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companys", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "CompanyHoldings",
                columns: table => new
                {
                    CompanyHoldingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ticker = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    StockName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PercentOfPortfolio = table.Column<double>(type: "double", nullable: false),
                    NumberOfShares = table.Column<int>(type: "int", nullable: false),
                    RecentActivity = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ReportedPrice = table.Column<double>(type: "double", nullable: false),
                    Value = table.Column<double>(type: "double", nullable: false),
                    DateRecorded = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    DatePulled = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyHoldings", x => x.CompanyHoldingId);
                    table.ForeignKey(
                        name: "FK_CompanyHoldings_Companys_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companys",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyHoldings_CompanyId",
                table: "CompanyHoldings",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyHoldings");

            migrationBuilder.DropTable(
                name: "Companys");
        }
    }
}