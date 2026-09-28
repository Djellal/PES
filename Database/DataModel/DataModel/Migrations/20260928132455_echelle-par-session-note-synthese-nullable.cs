using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataModel.Migrations
{
    /// <inheritdoc />
    public partial class echelleparsessionnotesynthesenullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Echelles_Echellid",
                table: "Evaluations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Echelles",
                table: "Echelles");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Echelles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "IdScale",
                table: "Echelles",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Sessionid",
                table: "Echelles",
                type: "integer",
                nullable: true);

            // Replique l'echelle globale (Sessionid IS NULL = echelle par defaut) dans chaque session.
            // L'echelle par defaut reste en place : elle sert de modele pour les nouvelles sessions.
            migrationBuilder.Sql(@"
                INSERT INTO ""Echelles"" (""Id"", ""Val"", ""Sessionid"")
                SELECT src.""Id"", src.""Val"", s.""Id""
                FROM ""Echelles"" src
                CROSS JOIN ""Sessions"" s
                WHERE src.""Sessionid"" IS NULL
                  AND NOT EXISTS (
                      SELECT 1 FROM ""Echelles"" e2
                      WHERE e2.""Id"" = src.""Id"" AND e2.""Sessionid"" = s.""Id""
                  );");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Echelles",
                table: "Echelles",
                column: "IdScale");

            // Evaluations.Echellid passe du libelle (texte) a la cle de substitution (entier).
            // PostgreSQL n'accepte pas de sous-requete dans ALTER ... USING, on passe par une colonne tampon.
            // Resolution par libelle + session du stagiaire, avec repli sur l'echelle par defaut.
            migrationBuilder.Sql(@"
                ALTER TABLE ""Evaluations"" ADD COLUMN ""EchellidNew"" integer;");

            migrationBuilder.Sql(@"
                UPDATE ""Evaluations"" ev
                SET ""EchellidNew"" = COALESCE(
                    (
                        SELECT sc.""IdScale""
                        FROM ""Echelles"" sc
                        JOIN ""Stagiaires"" st ON st.""Id"" = ev.""Stagid""
                        WHERE sc.""Id"" = ev.""Echellid""
                          AND sc.""Sessionid"" = st.""Sessionid""
                        LIMIT 1
                    ),
                    (
                        SELECT sc.""IdScale""
                        FROM ""Echelles"" sc
                        WHERE sc.""Id"" = ev.""Echellid""
                          AND sc.""Sessionid"" IS NULL
                        LIMIT 1
                    )
                );");

            migrationBuilder.Sql(@"
                ALTER TABLE ""Evaluations"" DROP COLUMN ""Echellid"";
                ALTER TABLE ""Evaluations"" RENAME COLUMN ""EchellidNew"" TO ""Echellid"";");

            migrationBuilder.AlterColumn<double>(
                name: "NoteSynthese",
                table: "Evaluations",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            // Une synthese NaN signifie « aucun membre du jury n'a note ce critere » -> NULL.
            migrationBuilder.Sql(@"
                UPDATE ""Evaluations"" SET ""NoteSynthese"" = NULL WHERE ""NoteSynthese"" = 'NaN'::float8;");

            migrationBuilder.CreateIndex(
                name: "IX_Echelles_Sessionid",
                table: "Echelles",
                column: "Sessionid");

            migrationBuilder.CreateIndex(
                name: "IX_Echelles_Id_Sessionid",
                table: "Echelles",
                columns: new[] { "Id", "Sessionid" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Echelles_Sessions_Sessionid",
                table: "Echelles",
                column: "Sessionid",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Echelles_Echellid",
                table: "Evaluations",
                column: "Echellid",
                principalTable: "Echelles",
                principalColumn: "IdScale");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Echelles_Echellid",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Echelles_Sessions_Sessionid",
                table: "Echelles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Echelles",
                table: "Echelles");

            migrationBuilder.DropIndex(
                name: "IX_Echelles_Id_Sessionid",
                table: "Echelles");

            migrationBuilder.DropIndex(
                name: "IX_Echelles_Sessionid",
                table: "Echelles");

            migrationBuilder.Sql(@"
                ALTER TABLE ""Evaluations"" ADD COLUMN ""EchellidOld"" text;");

            migrationBuilder.Sql(@"
                UPDATE ""Evaluations"" ev
                SET ""EchellidOld"" = (SELECT e.""Id"" FROM ""Echelles"" e WHERE e.""IdScale"" = ev.""Echellid"");");

            migrationBuilder.Sql(@"
                ALTER TABLE ""Evaluations"" DROP COLUMN ""Echellid"";
                ALTER TABLE ""Evaluations"" RENAME COLUMN ""EchellidOld"" TO ""Echellid"";");

            // Supprime les repliques par session, on ne garde que l'echelle globale d'origine.
            migrationBuilder.Sql(@"
                DELETE FROM ""Echelles""
                WHERE ""Id"" NOT IN (SELECT DISTINCT ""Id"" FROM ""Echelles"" WHERE ""Sessionid"" IS NULL);");

            migrationBuilder.DropColumn(
                name: "IdScale",
                table: "Echelles");

            migrationBuilder.DropColumn(
                name: "Sessionid",
                table: "Echelles");

            migrationBuilder.Sql(@"
                UPDATE ""Evaluations"" SET ""NoteSynthese"" = 0.0 WHERE ""NoteSynthese"" IS NULL;");

            migrationBuilder.AlterColumn<double>(
                name: "NoteSynthese",
                table: "Evaluations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Echelles",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Echelles",
                table: "Echelles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Echelles_Echellid",
                table: "Evaluations",
                column: "Echellid",
                principalTable: "Echelles",
                principalColumn: "Id");
        }
    }
}
