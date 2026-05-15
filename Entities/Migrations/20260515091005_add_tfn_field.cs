using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class add_tfn_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('Persons', 'TFN') IS NULL
                BEGIN
                    ALTER TABLE [Persons] ADD [TFN] nvarchar(200) NULL DEFAULT N'ABC';
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('Persons', 'TFN') IS NOT NULL
                BEGIN
                    ALTER TABLE [Persons] DROP COLUMN [TFN];
                END
                """);
        }
    }
}
