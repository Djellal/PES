using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pes.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AddColumn<int>(
                name: "Departid",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Etabid",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Facultid",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Qualite",
                table: "AspNetUsers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Regid",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

           

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Departid",
                table: "AspNetUsers",
                column: "Departid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Etabid",
                table: "AspNetUsers",
                column: "Etabid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Facultid",
                table: "AspNetUsers",
                column: "Facultid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Regid",
                table: "AspNetUsers",
                column: "Regid");


           

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departements_Departid",
                table: "AspNetUsers",
                column: "Departid",
                principalSchema: "public",
                principalTable: "Departements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Etablissements_Etabid",
                table: "AspNetUsers",
                column: "Etabid",
                principalSchema: "public",
                principalTable: "Etablissements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Facultes_Facultid",
                table: "AspNetUsers",
                column: "Facultid",
                principalSchema: "public",
                principalTable: "Facultes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Regions_Regid",
                table: "AspNetUsers",
                column: "Regid",
                principalSchema: "public",
                principalTable: "Regions",
                principalColumn: "Id");

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departements_Departid",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Etablissements_Etabid",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Facultes_Facultid",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Regions_Regid",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Evaluations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RangsEtabs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Criteres",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Echelles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Stagiaires",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Elements",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Departements",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Rubriques",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Facultes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Etablissements",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Regions",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Departid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Etabid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Facultid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Regid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Departid",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Etabid",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Facultid",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Qualite",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Regid",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetUserClaims",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetRoleClaims",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
