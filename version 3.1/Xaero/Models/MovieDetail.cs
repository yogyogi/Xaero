using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Xaero.Models
{
    public class MovieDetail
    {
        public int MovieId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Poster { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Budget { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Gross { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public Movie Movie_R { get; set; } // one-to-one with Movie
    }
}
