using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Etablissement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(300)]
        public required string NomEtablissement { get; set; } = string.Empty;



        [ForeignKey("Region")]
        public int? Regid { get; set; }
        public Region? Region { get; set; }

        public double Rang { get; set; } = 0;
    }
}
