using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Rubriques", Schema = "public")]
  public partial class Rubrique
  {
    public double Coeff
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

    public ICollection<Element> Elements { get; set; }
    public int? Sessionid
    {
      get;
      set;
    }
    public Session Session { get; set; }
    public string NomRubrique
    {
      get;
      set;
    }
  }
}
