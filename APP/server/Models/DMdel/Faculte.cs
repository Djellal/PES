using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Facultes", Schema = "public")]
  public partial class Faculte
  {
    public int? Etabid
    {
      get;
      set;
    }
    public Etablissement Etablissement { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id
    {
      get;
      set;
    }

    public ICollection<Departement> Departements { get; set; }
    public ICollection<Stagiaire> Stagiaires { get; set; }
    public string NomFaculte
    {
      get;
      set;
    }
  }
}
