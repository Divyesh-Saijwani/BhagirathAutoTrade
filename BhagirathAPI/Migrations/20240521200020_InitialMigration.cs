using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BhagirathAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Exchange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instrument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    StrickPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CMP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Average = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SST = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RST = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    S_Bap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    R_Bap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    S3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    R2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StopLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Target = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockData_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockData_StockId",
                table: "StockData",
                column: "StockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockData");

            migrationBuilder.DropTable(
                name: "Stock");
        }
    }
}
