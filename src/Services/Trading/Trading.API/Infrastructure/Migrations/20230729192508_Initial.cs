using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LibraLibrium.Services.Trading.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "trading");

            migrationBuilder.CreateSequence(
                name: "entryseq",
                schema: "trading",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Books",
                schema: "trading",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });

            migrationBuilder.CreateTable(
                name: "EntryType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                schema: "trading",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedByReceiver = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptedBySender = table.Column<bool>(type: "boolean", nullable: false),
                    GenerationClosed = table.Column<bool>(type: "boolean", nullable: false),
                    Closed = table.Column<bool>(type: "boolean", nullable: false),
                    ReceiverId = table.Column<string>(type: "text", nullable: false),
                    SenderId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                schema: "trading",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false),
                    Generation = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TradeId = table.Column<int>(type: "integer", nullable: false),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    TraderId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entries_Books_BookId",
                        column: x => x.BookId,
                        principalSchema: "trading",
                        principalTable: "Books",
                        principalColumn: "BookId");
                    table.ForeignKey(
                        name: "FK_Entries_EntryType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "EntryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entries_Trades_TradeId",
                        column: x => x.TradeId,
                        principalSchema: "trading",
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_BookId",
                schema: "trading",
                table: "Entries",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_TradeId",
                schema: "trading",
                table: "Entries",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_TypeId",
                schema: "trading",
                table: "Entries",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries",
                schema: "trading");

            migrationBuilder.DropTable(
                name: "Books",
                schema: "trading");

            migrationBuilder.DropTable(
                name: "EntryType");

            migrationBuilder.DropTable(
                name: "Trades",
                schema: "trading");

            migrationBuilder.DropSequence(
                name: "entryseq",
                schema: "trading");
        }
    }
}
