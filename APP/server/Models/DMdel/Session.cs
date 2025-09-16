using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Sessions", Schema = "public")]
  public partial class Session
  {
    public DateTime DateDebut
    {
      get;
      set;
    }
    public bool EnCours
    {
      get;
      set;
    }
    public DateTime DateFin
    {
      get;
      set;
    }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id
    {
      get;
      set;
    }

    public ICollection<RangsEtab> RangsEtabs { get; set; }
    public ICollection<Element> Elements { get; set; }
    public ICollection<Critere> Criteres { get; set; }
    public ICollection<Stagiaire> Stagiaires { get; set; }
    public ICollection<Rubrique> Rubriques { get; set; }
    public string NomSession
    {
      get;
      set;
    }
    public string CodeSession
    {
      get;
      set;
    }
  }
}
