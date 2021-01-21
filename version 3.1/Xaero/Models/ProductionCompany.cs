using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xaero.Models
{
    public class ProductionCompany
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Logo { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal AnnualRevenue { get; set; }

        [Required]
        public DateTime EstablishmentDate { get; set; }

        public ICollection<Movie> Movie_R { get; set; } // one-to-Many with Movie
    }
}
