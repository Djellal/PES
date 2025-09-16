using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Element
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(300)]
        public required string NomElement { get; set; } = string.Empty;

        [ForeignKey("Rubrique")]
        public int? Rubid { get; set; }
        public Rubrique? Rubrique { get; set; }

        public ICollection<Critere> Criteres { get; set; }


        [ForeignKey("Session")]
        public int? Sessionid { get; set; }
        public Session? Session { get; set; }
    }
}
