using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  [Table("RangsEtabs", Schema = "public")]
  public partial class RangsEtab
  {
    public int? Etabid
    {
      get;
      set;
    }
    public Etablissement Etablissement { get; set; }
    public int? Sessionid
    {
      get;
      set;
    }
    public Session Session { get; set; }
    public double Rang
    {
      get;
      set;
    }
    public double Moyenne
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
  }
}
