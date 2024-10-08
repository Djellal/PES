using Microsoft.AspNetCore.Identity;
using Pes.Models.DMdel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models
{
    public partial class ApplicationUser
    {
        

        [ForeignKey("Region")]
        public int? Regid { get; set; }
        public Region? Region { get; set; }


        [ForeignKey("Etablissement")]
        public int? Etabid { get; set; }
        public Etablissement? Etablissement { get; set; }

        [ForeignKey("Faculte")]
        public int? Facultid { get; set; }
        public Faculte? Faculte { get; set; }

        [ForeignKey("Departement")]
        public int? Departid { get; set; }
        public Departement? Departement { get; set; }

        [MaxLength(200)]
        public string Qualite { get; set; }

       

    }
    
}
