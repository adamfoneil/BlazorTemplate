using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class Widgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Widget",
                table: "Widget");

            migrationBuilder.RenameTable(
                name: "Widget",
                newName: "Widgets");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Widgets_Name",
                table: "Widgets",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Widgets",
                table: "Widgets",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Widgets_Name",
                table: "Widgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Widgets",
                table: "Widgets");

            migrationBuilder.RenameTable(
                name: "Widgets",
                newName: "Widget");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Widget",
                table: "Widget",
                column: "Id");
        }
    }
}
