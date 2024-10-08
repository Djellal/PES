using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Stagiaire
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(300)]
        public required string Nom { get; set; } = string.Empty;
        public required string Prenom { get; set; } = string.Empty;
        public required string email { get; set; } = string.Empty;
        public required DateTime DateNaissance { get; set; } = DateTime.Now.Date;        
        public required string portefolio { get; set; } = string.Empty;

        public required string RefAttestation { get; set; } = string.Empty;

        public bool? CourEnligne { get; set; } = false;
        public required string URLcour { get; set; } = string.Empty;

        

        [ForeignKey("Etablissement")]
        public int? Etabid { get; set; }
        public Etablissement? Etablissement { get; set; }

        [ForeignKey("Faculte")]
        public int? Facid { get; set; }
        public Faculte? Faculte { get; set; }

        [ForeignKey("Departement")]
        public int? Departid { get; set; }
        public Departement? Departement { get; set; }

        [ForeignKey("Session")]
        public int? Sessionid { get; set; }
        public Session? Session { get; set; }

        public double NoteCC { get; set; } = 0;        
        public double Note { get; set; } = 0;

        public double NoteFinale { get; set; } = 0;

        public ICollection<Evaluation> Evaluations { get; set; }
    }
}
