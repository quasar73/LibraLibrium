using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraLibrium.Services.Profile.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "profile");

            migrationBuilder.CreateSequence(
                name: "badgeseq",
                schema: "profile",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "profileseq",
                schema: "profile",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "BadgeType",
                schema: "profile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BadgeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "profile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Identity = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    State = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Country = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false, defaultValue: 0.5f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Badges",
                schema: "profile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "[Badge Name]"),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, defaultValue: "[Badge Description]"),
                    Level = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Badges_BadgeType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "profile",
                        principalTable: "BadgeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileBadges",
                schema: "profile",
                columns: table => new
                {
                    BadgesId = table.Column<int>(type: "integer", nullable: false),
                    UserProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileBadges", x => new { x.BadgesId, x.UserProfileId });
                    table.ForeignKey(
                        name: "FK_ProfileBadges_Badges_BadgesId",
                        column: x => x.BadgesId,
                        principalSchema: "profile",
                        principalTable: "Badges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileBadges_Profiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "profile",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Badges_TypeId",
                schema: "profile",
                table: "Badges",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileBadges_UserProfileId",
                schema: "profile",
                table: "ProfileBadges",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileBadges",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "Badges",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "BadgeType",
                schema: "profile");

            migrationBuilder.DropSequence(
                name: "badgeseq",
                schema: "profile");

            migrationBuilder.DropSequence(
                name: "profileseq",
                schema: "profile");
        }
    }
}
