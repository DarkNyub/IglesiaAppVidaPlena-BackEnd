using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IglesiaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "photo_url",
                table: "organization_structures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo_url",
                table: "members",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "photo_url",
                table: "organization_structures");

            migrationBuilder.DropColumn(
                name: "photo_url",
                table: "members");
        }
    }
}
