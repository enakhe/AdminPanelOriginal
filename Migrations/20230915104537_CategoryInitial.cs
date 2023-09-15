using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanel.Migrations
{
    /// <inheritdoc />
    public partial class CategoryInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Identity",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                schema: "Identity",
                table: "Role",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "Icon",
                schema: "Identity",
                table: "Role",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerID",
                schema: "Identity",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                schema: "Identity",
                table: "Role",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                schema: "Identity",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationCategory",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationRoleCategory",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationRoleId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRoleCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationRoleCategory_ApplicationCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Identity",
                        principalTable: "ApplicationCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApplicationRoleCategory_Role_ApplicationRoleId",
                        column: x => x.ApplicationRoleId,
                        principalSchema: "Identity",
                        principalTable: "Role",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRoleCategory_ApplicationRoleId",
                schema: "Identity",
                table: "ApplicationRoleCategory",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRoleCategory_CategoryId",
                schema: "Identity",
                table: "ApplicationRoleCategory",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationRoleCategory",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ApplicationCategory",
                schema: "Identity");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Identity",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                schema: "Identity",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "Identity",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ManagerID",
                schema: "Identity",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Identity",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Tag",
                schema: "Identity",
                table: "Role");
        }
    }
}
