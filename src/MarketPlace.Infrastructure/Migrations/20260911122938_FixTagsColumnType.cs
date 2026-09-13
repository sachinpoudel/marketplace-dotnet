using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPlace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTagsColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
    // 1. Drop the old conflicting column structure from Neon
    migrationBuilder.Sql(@"ALTER TABLE ""Products"" DROP COLUMN IF EXISTS ""Tags"";");

    // 2. Add the clean new PostgreSQL text array column
    migrationBuilder.Sql(@"ALTER TABLE ""Products"" ADD COLUMN ""Tags"" text[];");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }
    }
}
