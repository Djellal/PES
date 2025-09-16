using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Criteres", Schema = "public")]
  public partial class Critere
  {
    public int? Sessionid
    {
      get;
      set;
    }
    public Session Session { get; set; }
    public int? Elementid
    {
      get;
      set;
    }
    public Element Element { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id
    {
      get;
      set;
    }

    public ICollection<Evaluation> Evaluations { get; set; }
    public string NomCritere
    {
      get;
      set;
    }
  }
}
