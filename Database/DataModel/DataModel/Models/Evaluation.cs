using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Evaluation
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool EstSynthese { get; set; } = false;

        [MaxLength(100)]
        public string MembreId { get; set; } = string.Empty;


        [ForeignKey("Stagiaire")]
        public int? Stagid { get; set; }
        public Stagiaire? Stagiaire { get; set; }


        [MaxLength(300)]
        public string NomRubrique { get; set; } = string.Empty;

        [MaxLength(300)]
        public string NomElement { get; set; } = string.Empty;


        [ForeignKey("Critere")]
        public int? Criterid { get; set; }
        public Critere? Critere { get; set; }


        [ForeignKey("Echelle")]
        public String? Echellid { get; set; }
        public Echelle? Echelle { get; set; }


        public double NoteSynthese { get; set; } = 0;

    }
}
