﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pes.Data;

#nullable disable

namespace Pes.Data.Migrations
{
    [DbContext(typeof(ApplicationIdentityDbContext))]
    [Migration("20240320111244_ExtendApplicationUser")]
    partial class ExtendApplicationUser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Pes.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<int?>("Departid")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<int?>("Etabid")
                        .HasColumnType("integer");

                    b.Property<int?>("Facultid")
                        .HasColumnType("integer");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("Qualite")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int?>("Regid")
                        .HasColumnType("integer");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("Departid");

                    b.HasIndex("Etabid");

                    b.HasIndex("Facultid");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("Regid");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Pes.Models.DMdel.Critere", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Elementid")
                        .HasColumnType("integer");

                    b.Property<string>("NomCritere")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Elementid");

                    b.ToTable("Criteres", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Departement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Facid")
                        .HasColumnType("integer");

                    b.Property<int?>("FaculteId")
                        .HasColumnType("integer");

                    b.Property<string>("NomDepartement")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FaculteId");

                    b.ToTable("Departements", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Echelle", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<double>("Val")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Echelles", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Element", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NomElement")
                        .HasColumnType("text");

                    b.Property<int?>("Rubid")
                        .HasColumnType("integer");

                    b.Property<int?>("RubriqueId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RubriqueId");

                    b.ToTable("Elements", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Etablissement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NomEtablissement")
                        .HasColumnType("text");

                    b.Property<double>("Rang")
                        .HasColumnType("double precision");

                    b.Property<int?>("Regid")
                        .HasColumnType("integer");

                    b.Property<int?>("RegionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RegionId");

                    b.ToTable("Etablissements", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Evaluation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CritereId")
                        .HasColumnType("integer");

                    b.Property<int?>("Criterid")
                        .HasColumnType("integer");

                    b.Property<string>("EchelleId")
                        .HasColumnType("text");

                    b.Property<string>("Echellid")
                        .HasColumnType("text");

                    b.Property<bool>("EstSynthese")
                        .HasColumnType("boolean");

                    b.Property<string>("MembreId")
                        .HasColumnType("text");

                    b.Property<string>("NomElement")
                        .HasColumnType("text");

                    b.Property<string>("NomRubrique")
                        .HasColumnType("text");

                    b.Property<int?>("StagiaireId")
                        .HasColumnType("integer");

                    b.Property<int?>("Stagid")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CritereId");

                    b.HasIndex("EchelleId");

                    b.HasIndex("StagiaireId");

                    b.ToTable("Evaluations", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Faculte", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Etabid")
                        .HasColumnType("integer");

                    b.Property<int?>("EtablissementId")
                        .HasColumnType("integer");

                    b.Property<string>("NomFaculte")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EtablissementId");

                    b.ToTable("Facultes", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.RangsEtab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Etabid")
                        .HasColumnType("integer");

                    b.Property<int?>("EtablissementId")
                        .HasColumnType("integer");

                    b.Property<double>("Rang")
                        .HasColumnType("double precision");

                    b.Property<int?>("Sessionid")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EtablissementId");

                    b.HasIndex("Sessionid");

                    b.ToTable("RangsEtabs", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NomRegion")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Regions", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Rubrique", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NomRubrique")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Rubriques", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateDebut")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("EnCours")
                        .HasColumnType("boolean");

                    b.Property<string>("NomSession")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Sessions", "public");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Stagiaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateNaissance")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("DepartementId")
                        .HasColumnType("integer");

                    b.Property<int?>("Departid")
                        .HasColumnType("integer");

                    b.Property<int?>("Etabid")
                        .HasColumnType("integer");

                    b.Property<int?>("EtablissementId")
                        .HasColumnType("integer");

                    b.Property<int?>("Facid")
                        .HasColumnType("integer");

                    b.Property<int?>("FaculteId")
                        .HasColumnType("integer");

                    b.Property<string>("Nom")
                        .HasColumnType("text");

                    b.Property<double>("Note")
                        .HasColumnType("double precision");

                    b.Property<double>("NoteCC")
                        .HasColumnType("double precision");

                    b.Property<double>("NoteFinale")
                        .HasColumnType("double precision");

                    b.Property<string>("Prenom")
                        .HasColumnType("text");

                    b.Property<int?>("Sessionid")
                        .HasColumnType("integer");

                    b.Property<string>("URLcour")
                        .HasColumnType("text");

                    b.Property<string>("email")
                        .HasColumnType("text");

                    b.Property<string>("portefolio")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DepartementId");

                    b.HasIndex("EtablissementId");

                    b.HasIndex("FaculteId");

                    b.HasIndex("Sessionid");

                    b.ToTable("Stagiaires", "public");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Pes.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Pes.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pes.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Pes.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pes.Models.ApplicationUser", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("Departid");

                    b.HasOne("Pes.Models.DMdel.Etablissement", "Etablissement")
                        .WithMany()
                        .HasForeignKey("Etabid");

                    b.HasOne("Pes.Models.DMdel.Faculte", "Faculte")
                        .WithMany()
                        .HasForeignKey("Facultid");

                    b.HasOne("Pes.Models.DMdel.Region", "Region")
                        .WithMany()
                        .HasForeignKey("Regid");

                    b.Navigation("Departement");

                    b.Navigation("Etablissement");

                    b.Navigation("Faculte");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Critere", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Element", "Element")
                        .WithMany("Criteres")
                        .HasForeignKey("Elementid");

                    b.Navigation("Element");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Departement", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Faculte", "Faculte")
                        .WithMany("Departements")
                        .HasForeignKey("FaculteId");

                    b.Navigation("Faculte");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Element", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Rubrique", "Rubrique")
                        .WithMany("Elements")
                        .HasForeignKey("RubriqueId");

                    b.Navigation("Rubrique");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Etablissement", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Region", "Region")
                        .WithMany("Etablissements")
                        .HasForeignKey("RegionId");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Evaluation", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Critere", "Critere")
                        .WithMany("Evaluations")
                        .HasForeignKey("CritereId");

                    b.HasOne("Pes.Models.DMdel.Echelle", "Echelle")
                        .WithMany("Evaluations")
                        .HasForeignKey("EchelleId");

                    b.HasOne("Pes.Models.DMdel.Stagiaire", "Stagiaire")
                        .WithMany("Evaluations")
                        .HasForeignKey("StagiaireId");

                    b.Navigation("Critere");

                    b.Navigation("Echelle");

                    b.Navigation("Stagiaire");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Faculte", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Etablissement", "Etablissement")
                        .WithMany("Facultes")
                        .HasForeignKey("EtablissementId");

                    b.Navigation("Etablissement");
                });

            modelBuilder.Entity("Pes.Models.DMdel.RangsEtab", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Etablissement", "Etablissement")
                        .WithMany("RangsEtabs")
                        .HasForeignKey("EtablissementId");

                    b.HasOne("Pes.Models.DMdel.Session", "Session")
                        .WithMany("RangsEtabs")
                        .HasForeignKey("Sessionid");

                    b.Navigation("Etablissement");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Stagiaire", b =>
                {
                    b.HasOne("Pes.Models.DMdel.Departement", "Departement")
                        .WithMany("Stagiaires")
                        .HasForeignKey("DepartementId");

                    b.HasOne("Pes.Models.DMdel.Etablissement", "Etablissement")
                        .WithMany("Stagiaires")
                        .HasForeignKey("EtablissementId");

                    b.HasOne("Pes.Models.DMdel.Faculte", "Faculte")
                        .WithMany("Stagiaires")
                        .HasForeignKey("FaculteId");

                    b.HasOne("Pes.Models.DMdel.Session", "Session")
                        .WithMany("Stagiaires")
                        .HasForeignKey("Sessionid");

                    b.Navigation("Departement");

                    b.Navigation("Etablissement");

                    b.Navigation("Faculte");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Critere", b =>
                {
                    b.Navigation("Evaluations");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Departement", b =>
                {
                    b.Navigation("Stagiaires");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Echelle", b =>
                {
                    b.Navigation("Evaluations");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Element", b =>
                {
                    b.Navigation("Criteres");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Etablissement", b =>
                {
                    b.Navigation("Facultes");

                    b.Navigation("RangsEtabs");

                    b.Navigation("Stagiaires");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Faculte", b =>
                {
                    b.Navigation("Departements");

                    b.Navigation("Stagiaires");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Region", b =>
                {
                    b.Navigation("Etablissements");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Rubrique", b =>
                {
                    b.Navigation("Elements");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Session", b =>
                {
                    b.Navigation("RangsEtabs");

                    b.Navigation("Stagiaires");
                });

            modelBuilder.Entity("Pes.Models.DMdel.Stagiaire", b =>
                {
                    b.Navigation("Evaluations");
                });
#pragma warning restore 612, 618
        }
    }
}
