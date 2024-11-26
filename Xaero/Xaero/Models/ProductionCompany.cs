using System.ComponentModel.DataAnnotations;

namespace Xaero.Models
{
    public class ProductionCompany
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Logo { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal AnnualRevenue { get; set; }

        [Required]
        public DateTime EstablishmentDate { get; set; }

        public ICollection<Movie> Movie_R { get; } = new List<Movie>(); // one-to-Many with Movie
    }
}
