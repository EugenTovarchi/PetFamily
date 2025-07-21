using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "volunteers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    middle_name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    volunteer_info = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    volunteer_exp_years = table.Column<decimal>(type: "numeric", maxLength: 40, nullable: false),
                    volunteer_requisites = table.Column<string>(type: "jsonb", nullable: true),
                    volunteer_social_media = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volunteers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    health_info = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: true),
                    height = table.Column<double>(type: "double precision", nullable: true),
                    owner_phone = table.Column<string>(type: "text", nullable: false),
                    castrated = table.Column<bool>(type: "boolean", nullable: true),
                    vaccinated = table.Column<bool>(type: "boolean", nullable: false),
                    birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    pet_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "date", nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    breed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    color = table.Column<int>(type: "integer", nullable: false),
                    volunteer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    city = table.Column<string>(type: "text", nullable: false),
                    flat = table.Column<long>(type: "bigint", nullable: false),
                    house = table.Column<long>(type: "bigint", nullable: false),
                    street = table.Column<string>(type: "text", nullable: false),
                    pet_requisites = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pets_volunteers_volunteer_id",
                        column: x => x.volunteer_id,
                        principalTable: "volunteers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_pets_volunteer_id",
                table: "pets",
                column: "volunteer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "volunteers");
        }
    }
}
