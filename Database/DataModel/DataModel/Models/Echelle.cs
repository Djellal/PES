using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Echelle
    {
        [Key]       
        public String Id { get; set; } = string.Empty;
        public double Val { get; set; } = 0;
    }
}
