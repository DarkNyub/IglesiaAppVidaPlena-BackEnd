using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IglesiaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowMultipleSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_record_types");

            migrationBuilder.DropTable(
                name: "organization_members");

            migrationBuilder.DropTable(
                name: "record_type_fields");

            migrationBuilder.DropTable(
                name: "registry_events");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "user_system_roles");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "record_types");

            migrationBuilder.DropTable(
                name: "system_roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "organization_structures");

            migrationBuilder.DropTable(
                name: "church_function_roles");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "organization_types");
        }
    }
}
