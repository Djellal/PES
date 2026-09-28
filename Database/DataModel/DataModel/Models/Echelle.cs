using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Echelle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdScale { get; set; }

        [Required]
        [MaxLength(100)]
        public string Id { get; set; } = string.Empty;

        public double Val { get; set; } = 0;

        [ForeignKey("Session")]
        public int? Sessionid { get; set; }
        public Session? Session { get; set; }
    }
}
