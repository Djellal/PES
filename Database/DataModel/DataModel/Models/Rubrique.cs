using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Rubrique
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(300)]
        public required string NomRubrique { get; set; } = string.Empty;

        public ICollection<Element> Elements { get; set; }

        public double Coeff { get; set; } = 1;
    }
}
