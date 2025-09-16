using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Stagiaires", Schema = "public")]
  public partial class Stagiaire
  {
    public DateTime DateNaissance
    {
      get;
      set;
    }
    public int? Etabid
    {
      get;
      set;
    }
    public Etablissement Etablissement { get; set; }
    public int? Facid
    {
      get;
      set;
    }
    public Faculte Faculte { get; set; }
    public int? Departid
    {
      get;
      set;
    }
    public Departement Departement { get; set; }
    public int? Sessionid
    {
      get;
      set;
    }
    public Session Session { get; set; }
    public double NoteCC
    {
      get;
      set;
    }
    public double Note
    {
      get;
      set;
    }
    public double NoteFinale
    {
      get;
      set;
    }
    public bool? CourEnligne
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

    public ICollection<Evaluation> Evaluations { get; set; }
    public string Prenom
    {
      get;
      set;
    }
    public string email
    {
      get;
      set;
    }
    public string portefolio
    {
      get;
      set;
    }
    public string URLcour
    {
      get;
      set;
    }
    public string RefAttestation
    {
      get;
      set;
    }
    public string Nom
    {
      get;
      set;
    }
  }
}
