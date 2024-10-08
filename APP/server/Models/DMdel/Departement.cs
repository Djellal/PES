using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Departements", Schema = "public")]
  public partial class Departement
  {
    public int? Facid
    {
      get;
      set;
    }
    public Faculte Faculte { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id
    {
      get;
      set;
    }

    public ICollection<Stagiaire> Stagiaires { get; set; }
    public string NomDepartement
    {
      get;
      set;
    }
  }
}
