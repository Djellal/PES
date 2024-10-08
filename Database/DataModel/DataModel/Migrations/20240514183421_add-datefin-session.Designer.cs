﻿// <auto-generated />
using System;
using DataModel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataModel.Migrations
{
    [DbContext(typeof(DataModelContext))]
    [Migration("20240514183421_add-datefin-session")]
    partial class adddatefinsession
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataModel.Models.Critere", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Elementid")
                        .HasColumnType("integer");

                    b.Property<string>("NomCritere")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.HasIndex("Elementid");

                    b.ToTable("Criteres");
                });

            modelBuilder.Entity("DataModel.Models.Departement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Facid")
                        .HasColumnType("integer");

                    b.Property<string>("NomDepartement")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.HasIndex("Facid");

                    b.ToTable("Departements");
                });

            modelBuilder.Entity("DataModel.Models.Echelle", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<double>("Val")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Echelles");
                });

            modelBuilder.Entity("DataModel.Models.Element", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NomElement")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<int?>("Rubid")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Rubid");

                    b.ToTable("Elements");
                });

            modelBuilder.Entity("DataModel.Models.Etablissement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NomEtablissement")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<double>("Rang")
                        .HasColumnType("double precision");

                    b.Property<int?>("Regid")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Regid");

                    b.ToTable("Etablissements");
                });

            modelBuilder.Entity("DataModel.Models.Evaluation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Criterid")
                        .HasColumnType("integer");

                    b.Property<string>("Echellid")
                        .HasColumnType("text");

                    b.Property<bool>("EstSynthese")
                        .HasColumnType("boolean");

                    b.Property<string>("MembreId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("NomElement")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<string>("NomRubrique")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<double>("NoteSynthese")
                        .HasColumnType("double precision");

                    b.Property<int?>("Stagid")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Criterid");

                    b.HasIndex("Echellid");

                    b.HasIndex("Stagid");

                    b.ToTable("Evaluations");
                });

            modelBuilder.Entity("DataModel.Models.Faculte", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Etabid")
                        .HasColumnType("integer");

                    b.Property<string>("NomFaculte")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.HasIndex("Etabid");

                    b.ToTable("Facultes");
                });

            modelBuilder.Entity("DataModel.Models.RangEtab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Etabid")
                        .HasColumnType("integer");

                    b.Property<double>("Moyenne")
                        .HasColumnType("double precision");

                    b.Property<double>("Rang")
                        .HasColumnType("double precision");

                    b.Property<int?>("Sessionid")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Etabid");

                    b.HasIndex("Sessionid");

                    b.ToTable("RangsEtabs");
                });

            modelBuilder.Entity("DataModel.Models.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NomRegion")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("DataModel.Models.Rubrique", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("NomRubrique")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.ToTable("Rubriques");
                });

            modelBuilder.Entity("DataModel.Models.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateDebut")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateFin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("EnCours")
                        .HasColumnType("boolean");

                    b.Property<string>("NomSession")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("DataModel.Models.Stagiaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool?>("CourEnligne")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("DateNaissance")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("Departid")
                        .HasColumnType("integer");

                    b.Property<int?>("Etabid")
                        .HasColumnType("integer");

                    b.Property<int?>("Facid")
                        .HasColumnType("integer");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<double>("Note")
                        .HasColumnType("double precision");

                    b.Property<double>("NoteCC")
                        .HasColumnType("double precision");

                    b.Property<double>("NoteFinale")
                        .HasColumnType("double precision");

                    b.Property<string>("Prenom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefAttestation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("Sessionid")
                        .HasColumnType("integer");

                    b.Property<string>("URLcour")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("portefolio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Departid");

                    b.HasIndex("Etabid");

                    b.HasIndex("Facid");

                    b.HasIndex("Sessionid");

                    b.ToTable("Stagiaires");
                });

            modelBuilder.Entity("DataModel.Models.Critere", b =>
                {
                    b.HasOne("DataModel.Models.Element", "Element")
                        .WithMany("Criteres")
                        .HasForeignKey("Elementid");

                    b.Navigation("Element");
                });

            modelBuilder.Entity("DataModel.Models.Departement", b =>
                {
                    b.HasOne("DataModel.Models.Faculte", "Faculte")
                        .WithMany()
                        .HasForeignKey("Facid");

                    b.Navigation("Faculte");
                });

            modelBuilder.Entity("DataModel.Models.Element", b =>
                {
                    b.HasOne("DataModel.Models.Rubrique", "Rubrique")
                        .WithMany("Elements")
                        .HasForeignKey("Rubid");

                    b.Navigation("Rubrique");
                });

            modelBuilder.Entity("DataModel.Models.Etablissement", b =>
                {
                    b.HasOne("DataModel.Models.Region", "Region")
                        .WithMany()
                        .HasForeignKey("Regid");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("DataModel.Models.Evaluation", b =>
                {
                    b.HasOne("DataModel.Models.Critere", "Critere")
                        .WithMany()
                        .HasForeignKey("Criterid");

                    b.HasOne("DataModel.Models.Echelle", "Echelle")
                        .WithMany()
                        .HasForeignKey("Echellid");

                    b.HasOne("DataModel.Models.Stagiaire", "Stagiaire")
                        .WithMany("Evaluations")
                        .HasForeignKey("Stagid");

                    b.Navigation("Critere");

                    b.Navigation("Echelle");

                    b.Navigation("Stagiaire");
                });

            modelBuilder.Entity("DataModel.Models.Faculte", b =>
                {
                    b.HasOne("DataModel.Models.Etablissement", "Etablissement")
                        .WithMany()
                        .HasForeignKey("Etabid");

                    b.Navigation("Etablissement");
                });

            modelBuilder.Entity("DataModel.Models.RangEtab", b =>
                {
                    b.HasOne("DataModel.Models.Etablissement", "Etablissement")
                        .WithMany()
                        .HasForeignKey("Etabid");

                    b.HasOne("DataModel.Models.Session", "Session")
                        .WithMany()
                        .HasForeignKey("Sessionid");

                    b.Navigation("Etablissement");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("DataModel.Models.Stagiaire", b =>
                {
                    b.HasOne("DataModel.Models.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("Departid");

                    b.HasOne("DataModel.Models.Etablissement", "Etablissement")
                        .WithMany()
                        .HasForeignKey("Etabid");

                    b.HasOne("DataModel.Models.Faculte", "Faculte")
                        .WithMany()
                        .HasForeignKey("Facid");

                    b.HasOne("DataModel.Models.Session", "Session")
                        .WithMany()
                        .HasForeignKey("Sessionid");

                    b.Navigation("Departement");

                    b.Navigation("Etablissement");

                    b.Navigation("Faculte");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("DataModel.Models.Element", b =>
                {
                    b.Navigation("Criteres");
                });

            modelBuilder.Entity("DataModel.Models.Rubrique", b =>
                {
                    b.Navigation("Elements");
                });

            modelBuilder.Entity("DataModel.Models.Stagiaire", b =>
                {
                    b.Navigation("Evaluations");
                });
#pragma warning restore 612, 618
        }
    }
}
