using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Elements", Schema = "public")]
  public partial class Element
  {
    public int? Rubid
    {
      get;
      set;
    }
    public Rubrique Rubrique { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id
    {
      get;
      set;
    }

    public ICollection<Critere> Criteres { get; set; }
    public int? Sessionid
    {
      get;
      set;
    }
    public Session Session { get; set; }
    public string NomElement
    {
      get;
      set;
    }
  }
}
