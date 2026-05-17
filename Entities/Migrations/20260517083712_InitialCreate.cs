using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceiveNewsLetter = table.Column<bool>(type: "bit", nullable: false),
                    TFN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, defaultValue: "ABC")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_Persons_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryID");
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryID", "CountryName" },
                values: new object[,]
                {
                    { new Guid("d9c7b5a1-1234-4abc-8def-123456789001"), "USA" },
                    { new Guid("d9c7b5a1-1234-4abc-8def-123456789002"), "Nepal" },
                    { new Guid("d9c7b5a1-1234-4abc-8def-123456789003"), "India" },
                    { new Guid("d9c7b5a1-1234-4abc-8def-123456789004"), "UK" },
                    { new Guid("d9c7b5a1-1234-4abc-8def-123456789005"), "Canada" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonId", "Address", "CountryId", "DateOfBirth", "Email", "Gender", "Name", "ReceiveNewsLetter" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-1234-4abc-8def-123456789001"), "123 Main St, New York, NY", new Guid("d9c7b5a1-1234-4abc-8def-123456789001"), new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "Male", "John Doe", true },
                    { new Guid("a1b2c3d4-1234-4abc-8def-123456789002"), "456 Oak Ave, Toronto, ON", new Guid("d9c7b5a1-1234-4abc-8def-123456789005"), new DateTime(1992, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Female", "Jane Smith", false },
                    { new Guid("a1b2c3d4-1234-4abc-8def-123456789003"), "789 MG Road, Mumbai", new Guid("d9c7b5a1-1234-4abc-8def-123456789003"), new DateTime(1988, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "raj.patel@example.com", "Male", "Raj Patel", true },
                    { new Guid("a1b2c3d4-1234-4abc-8def-123456789004"), "10 Downing St, London", new Guid("d9c7b5a1-1234-4abc-8def-123456789004"), new DateTime(1995, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "emily.watson@example.com", "Female", "Emily Watson", true },
                    { new Guid("a1b2c3d4-1234-4abc-8def-123456789005"), "Basantapur, Kathmandu", new Guid("d9c7b5a1-1234-4abc-8def-123456789002"), new DateTime(1993, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "sita.sharma@example.com", "Female", "Sita Sharma", false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryId",
                table: "Persons",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Email",
                table: "Persons",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
