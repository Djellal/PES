using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models
{
  [Table("AuditLogs", Schema = "public")]
  public partial class AuditLog
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id
    {
      get;
      set;
    }

    public DateTime Timestamp
    {
      get;
      set;
    }

    public string UserId
    {
      get;
      set;
    }

    public string UserName
    {
      get;
      set;
    }

    public string Action
    {
      get;
      set;
    }

    public string EntityType
    {
      get;
      set;
    }

    public string EntityId
    {
      get;
      set;
    }

    public string Summary
    {
      get;
      set;
    }

    public string DetailsJson
    {
      get;
      set;
    }

    public string IpAddress
    {
      get;
      set;
    }

    public string Source
    {
      get;
      set;
    }
  }
}
