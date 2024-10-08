using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataModel.Models;

namespace DataModel.Data
{
    public class DataModelContext : DbContext
    {
        public DataModelContext (DbContextOptions<DataModelContext> options)
            : base(options)
        {
        }

       
        public DbSet<Region> Regions { get; set; }= default!;
        public DbSet<Etablissement> Etablissements { get; set; } = default!;
        public DbSet<Faculte> Facultes { get; set; } = default!;
        public DbSet<Departement> Departements { get; set; } = default!;
        public DbSet<Stagiaire> Stagiaires { get; set; } = default!;
        public DbSet<Session> Sessions { get; set; } = default!;

        public DbSet<Rubrique> Rubriques { get; set; } = default!;
        public DbSet<Element> Elements { get; set; } = default!;
        public DbSet<Critere> Criteres { get; set; } = default!;

        public DbSet<Echelle> Echelles { get; set; } = default!;
        public DbSet<Evaluation> Evaluations { get; set; } = default!;

        public DbSet<RangEtab> RangsEtabs { get; set; } = default!;

    }
}
