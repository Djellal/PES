using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public required string CodeSession { get; set; } = string.Empty;

        [MaxLength(300)]
        public required string NomSession { get; set; } = string.Empty;

        public required DateTime DateDebut { get; set; } = DateTime.Now.Date;

        public required DateTime DateFin { get; set; } = DateTime.Now.Date.AddMonths(3);
        public bool EnCours { get; set; } = false;
    }
}
