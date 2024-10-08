using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class RangEtab
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Etablissement")]
        public int? Etabid { get; set; }
        public Etablissement? Etablissement { get; set; }

        [ForeignKey("Session")]
        public int? Sessionid { get; set; }
        public Session? Session { get; set; }

        public double Moyenne { get; set; } = 0;
        public double Rang { get; set; } = 0;

    }
}
