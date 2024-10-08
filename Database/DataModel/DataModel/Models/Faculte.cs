using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Faculte
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(300)]
        public required string NomFaculte { get; set; } = string.Empty;

        [ForeignKey("Etablissement")]
        public int? Etabid { get; set; }
        public Etablissement? Etablissement { get; set; }
    }
}
