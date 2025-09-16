using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Critere
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(300)]
        public required string NomCritere { get; set; } = string.Empty;

        [ForeignKey("Element")]
        public int? Elementid { get; set; }
        public Element? Element { get; set; }


        [ForeignKey("Session")]
        public int? Sessionid { get; set; }
        public Session? Session { get; set; }
    }
}
