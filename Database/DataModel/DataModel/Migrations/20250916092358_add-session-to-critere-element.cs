using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataModel.Migrations
{
    /// <inheritdoc />
    public partial class addsessiontocritereelement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sessionid",
                table: "Rubriques",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sessionid",
                table: "Elements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sessionid",
                table: "Criteres",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rubriques_Sessionid",
                table: "Rubriques",
                column: "Sessionid");

            migrationBuilder.CreateIndex(
                name: "IX_Elements_Sessionid",
                table: "Elements",
                column: "Sessionid");

            migrationBuilder.CreateIndex(
                name: "IX_Criteres_Sessionid",
                table: "Criteres",
                column: "Sessionid");

            migrationBuilder.AddForeignKey(
                name: "FK_Criteres_Sessions_Sessionid",
                table: "Criteres",
                column: "Sessionid",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_Sessions_Sessionid",
                table: "Elements",
                column: "Sessionid",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rubriques_Sessions_Sessionid",
                table: "Rubriques",
                column: "Sessionid",
                principalTable: "Sessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Criteres_Sessions_Sessionid",
                table: "Criteres");

            migrationBuilder.DropForeignKey(
                name: "FK_Elements_Sessions_Sessionid",
                table: "Elements");

            migrationBuilder.DropForeignKey(
                name: "FK_Rubriques_Sessions_Sessionid",
                table: "Rubriques");

            migrationBuilder.DropIndex(
                name: "IX_Rubriques_Sessionid",
                table: "Rubriques");

            migrationBuilder.DropIndex(
                name: "IX_Elements_Sessionid",
                table: "Elements");

            migrationBuilder.DropIndex(
                name: "IX_Criteres_Sessionid",
                table: "Criteres");

            migrationBuilder.DropColumn(
                name: "Sessionid",
                table: "Rubriques");

            migrationBuilder.DropColumn(
                name: "Sessionid",
                table: "Elements");

            migrationBuilder.DropColumn(
                name: "Sessionid",
                table: "Criteres");
        }
    }
}
