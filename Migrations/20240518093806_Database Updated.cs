using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanel.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDisabled",
                schema: "Identity",
                table: "PersonalizationInfos",
                newName: "IsDisabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDisabled",
                schema: "Identity",
                table: "PersonalizationInfos",
                newName: "isDisabled");
        }
    }
}
