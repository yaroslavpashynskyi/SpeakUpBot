using System;
using Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:english_level", "pre_intermediate,intermediate,upper_intermediate,advanced")
                .Annotation("Npgsql:Enum:payment_status", "pending,to_be_approved,paid_by_card,to_be_paid_by_cash,cancelled")
                .Annotation("Npgsql:Enum:role", "member,admin");

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    LocationUrl = table.Column<string>(type: "text", nullable: false),
                    InstagramUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<Role>(type: "role", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    EnglishLevel = table.Column<EnglishLevel>(type: "english_level", nullable: false),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferTicket = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Speakings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Intro = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Seats = table.Column<int>(type: "integer", nullable: false),
                    TimeOfEvent = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speakings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Speakings_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdinalNumber = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    SpeakingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photo_Speakings_SpeakingId",
                        column: x => x.SpeakingId,
                        principalTable: "Speakings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpeakingId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentStatus = table.Column<PaymentStatus>(type: "payment_status", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Speakings_SpeakingId",
                        column: x => x.SpeakingId,
                        principalTable: "Speakings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "IsCustom", "Title" },
                values: new object[,]
                {
                    { new Guid("4ee0db47-324f-499c-a4d4-584b5b065f7c"), false, "Рекомендація від друзів" },
                    { new Guid("51786e0f-ebf3-46ba-9e40-cab616a32588"), false, "News Brovary" },
                    { new Guid("5f854f90-b8be-44b9-8a7a-7704f489fef6"), false, "Інстраграм сторінка SpeakUp" },
                    { new Guid("6c60c909-c857-4006-834b-f0a55ee0c936"), false, "Кафе \"І не тільки\"" },
                    { new Guid("94a4d26e-7684-4cd9-9bf6-63e250ac3713"), false, "Реклама в інстаграмі" },
                    { new Guid("963e4265-8d78-4107-ae64-e352aad047b6"), false, "Книжковий клуб \"Культурні\"" },
                    { new Guid("c8825caf-2504-4af3-9596-4a8392a03fc9"), false, "Телеграм канал SpeakUp" },
                    { new Guid("e31133c4-578f-4abc-a678-cfcb258354a3"), false, "Кафе \"Nest City Cafe\"" },
                    { new Guid("e66d6631-33d6-4d74-9ba0-e6a87658e377"), false, "Кафе \"Buono\"" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photo_SpeakingId",
                table: "Photo",
                column: "SpeakingId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_SpeakingId",
                table: "Registrations",
                column: "SpeakingId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_UserId",
                table: "Registrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Speakings_VenueId",
                table: "Speakings",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SourceId",
                table: "Users",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TelegramId",
                table: "Users",
                column: "TelegramId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photo");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Speakings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "Sources");
        }
    }
}
