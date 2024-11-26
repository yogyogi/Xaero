using System.ComponentModel.DataAnnotations;

namespace Xaero.Models
{
    public class Distribution
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Telephone { get; set; }

        public List<MovieDistribution> MovieDistribution_R { get; } = []; // Many-to-Many with Movie
    }
}