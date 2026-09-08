using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IglesiaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddFormulaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "formula_expression",
                table: "record_type_fields",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_formula",
                table: "record_type_fields",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "formula_expression",
                table: "record_type_fields");

            migrationBuilder.DropColumn(
                name: "is_formula",
                table: "record_type_fields");
        }
    }
}
