using System.ComponentModel.DataAnnotations;

namespace Xaero.Models
{
    public class MovieDetail
    {
        public int Id { get; set; } 

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

        public int MovieId { get; set; } // foreign key to Movie 

        public Movie Movie_R { get; set; } = null!; // one-to-one with Movie
    }
}
