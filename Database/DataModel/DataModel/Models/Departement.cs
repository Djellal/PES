using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Departement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(300)]
        public required string NomDepartement { get; set; } = string.Empty;

        [ForeignKey("Faculte")]
        public int? Facid { get; set; }
        public Faculte? Faculte { get; set; }
    }
}
