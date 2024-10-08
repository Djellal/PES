using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

using Pes.Models.DMdel;

namespace Pes.Data
{
  public partial class DMdelContext : Microsoft.EntityFrameworkCore.DbContext
  {
    public DMdelContext(DbContextOptions<DMdelContext> options):base(options)
    {
    }

    public DMdelContext()
    {
    }

    partial void OnModelBuilding(ModelBuilder builder);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Pes.Models.DMdel.Critere>()
              .HasOne(i => i.Element)
              .WithMany(i => i.Criteres)
              .HasForeignKey(i => i.Elementid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Departement>()
              .HasOne(i => i.Faculte)
              .WithMany(i => i.Departements)
              .HasForeignKey(i => i.Facid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Element>()
              .HasOne(i => i.Rubrique)
              .WithMany(i => i.Elements)
              .HasForeignKey(i => i.Rubid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Etablissement>()
              .HasOne(i => i.Region)
              .WithMany(i => i.Etablissements)
              .HasForeignKey(i => i.Regid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Evaluation>()
              .HasOne(i => i.Stagiaire)
              .WithMany(i => i.Evaluations)
              .HasForeignKey(i => i.Stagid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Evaluation>()
              .HasOne(i => i.Critere)
              .WithMany(i => i.Evaluations)
              .HasForeignKey(i => i.Criterid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Evaluation>()
              .HasOne(i => i.Echelle)
              .WithMany(i => i.Evaluations)
              .HasForeignKey(i => i.Echellid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Faculte>()
              .HasOne(i => i.Etablissement)
              .WithMany(i => i.Facultes)
              .HasForeignKey(i => i.Etabid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.RangsEtab>()
              .HasOne(i => i.Etablissement)
              .WithMany(i => i.RangsEtabs)
              .HasForeignKey(i => i.Etabid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.RangsEtab>()
              .HasOne(i => i.Session)
              .WithMany(i => i.RangsEtabs)
              .HasForeignKey(i => i.Sessionid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Stagiaire>()
              .HasOne(i => i.Etablissement)
              .WithMany(i => i.Stagiaires)
              .HasForeignKey(i => i.Etabid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Stagiaire>()
              .HasOne(i => i.Faculte)
              .WithMany(i => i.Stagiaires)
              .HasForeignKey(i => i.Facid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Stagiaire>()
              .HasOne(i => i.Departement)
              .WithMany(i => i.Stagiaires)
              .HasForeignKey(i => i.Departid)
              .HasPrincipalKey(i => i.Id);
        builder.Entity<Pes.Models.DMdel.Stagiaire>()
              .HasOne(i => i.Session)
              .WithMany(i => i.Stagiaires)
              .HasForeignKey(i => i.Sessionid)
              .HasPrincipalKey(i => i.Id);

        builder.Entity<Pes.Models.DMdel.Evaluation>()
              .Property(p => p.NoteSynthese)
              .HasDefaultValueSql("0.0").ValueGeneratedNever();

        builder.Entity<Pes.Models.DMdel.RangsEtab>()
              .Property(p => p.Moyenne)
              .HasDefaultValueSql("0.0").ValueGeneratedNever();

        builder.Entity<Pes.Models.DMdel.Rubrique>()
              .Property(p => p.Coeff)
              .HasDefaultValueSql("0.0").ValueGeneratedNever();

        builder.Entity<Pes.Models.DMdel.Session>()
              .Property(p => p.DateFin)
              .HasDefaultValueSql("'-infinity'::timestamp with time zone");

        builder.Entity<Pes.Models.DMdel.Session>()
              .Property(p => p.CodeSession)
              .HasDefaultValueSql("''::character varying");

        builder.Entity<Pes.Models.DMdel.Stagiaire>()
              .Property(p => p.RefAttestation)
              .HasDefaultValueSql("''::text");

        this.OnModelBuilding(builder);
    }


    public DbSet<Pes.Models.DMdel.Critere> Criteres
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Departement> Departements
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Echelle> Echelles
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Element> Elements
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Etablissement> Etablissements
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Evaluation> Evaluations
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Faculte> Facultes
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.RangsEtab> RangsEtabs
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Region> Regions
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Rubrique> Rubriques
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Session> Sessions
    {
      get;
      set;
    }

    public DbSet<Pes.Models.DMdel.Stagiaire> Stagiaires
    {
      get;
      set;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
    }
  }
}
