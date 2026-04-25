using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VUP.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnknownVerbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RawAction = table.Column<string>(type: "text", nullable: false),
                    DetectedType = table.Column<int>(type: "integer", nullable: false),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    FirstSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnknownVerbs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Verbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Lemma = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verbs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerbPatterns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VerbId = table.Column<int>(type: "integer", nullable: false),
                    Particle = table.Column<string>(type: "text", nullable: true),
                    Preposition = table.Column<string>(type: "text", nullable: true),
                    TypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerbPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerbPatterns_Verbs_VerbId",
                        column: x => x.VerbId,
                        principalTable: "Verbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnknownVerbs_RawAction",
                table: "UnknownVerbs",
                column: "RawAction");

            migrationBuilder.CreateIndex(
                name: "IX_VerbPatterns_VerbId",
                table: "VerbPatterns",
                column: "VerbId");

            migrationBuilder.CreateIndex(
                name: "IX_Verbs_Lemma",
                table: "Verbs",
                column: "Lemma",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnknownVerbs");

            migrationBuilder.DropTable(
                name: "VerbPatterns");

            migrationBuilder.DropTable(
                name: "Verbs");
        }
    }
}
