using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Echelles", Schema = "public")]
  public partial class Echelle
  {
    public double Val
    {
      get;
      set;
    }
    [Key]
    public string Id
    {
      get;
      set;
    }

    public ICollection<Evaluation> Evaluations { get; set; }
  }
}
