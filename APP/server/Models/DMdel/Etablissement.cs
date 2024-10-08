using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("Etablissements", Schema = "public")]
  public partial class Etablissement
  {
    public int? Regid
    {
      get;
      set;
    }
    public Region Region { get; set; }
    public double Rang
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

    public ICollection<Faculte> Facultes { get; set; }
    public ICollection<RangsEtab> RangsEtabs { get; set; }
    public ICollection<Stagiaire> Stagiaires { get; set; }
    public string NomEtablissement
    {
      get;
      set;
    }
  }
}
