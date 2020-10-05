using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public IList<MovieDistribution> MovieDistribution_R { get; set; } // Many-to-Many with Movie
    }
}
