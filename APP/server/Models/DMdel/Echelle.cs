using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Echelles", Schema = "public")]
  public partial class Echelle
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdScale
    {
      get;
      set;
    }
    public string Id
    {
      get;
      set;
    }
    public double Val
    {
      get;
      set;
    }
    public int? Sessionid
    {
      get;
      set;
    }
    [NotMapped]
    public string Libelle
    {
      get
      {
        return Id;
      }
      set
      {
        Id = value;
      }
    }

    public Session Session { get; set; }
    public ICollection<Evaluation> Evaluations { get; set; }
  }
}
