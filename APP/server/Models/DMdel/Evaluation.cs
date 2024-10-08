using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Evaluations", Schema = "public")]
  public partial class Evaluation
  {
    public bool EstSynthese
    {
      get;
      set;
    }
    public int? Stagid
    {
      get;
      set;
    }
    public Stagiaire Stagiaire { get; set; }
    public int? Criterid
    {
      get;
      set;
    }
    public Critere Critere { get; set; }
    public double NoteSynthese
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
    public string MembreId
    {
      get;
      set;
    }
    public string NomRubrique
    {
      get;
      set;
    }
    public string NomElement
    {
      get;
      set;
    }
    public string Echellid
    {
      get;
      set;
    }
    public Echelle Echelle { get; set; }
  }
}
