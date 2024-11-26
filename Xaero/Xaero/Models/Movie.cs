namespace Xaero.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public int ProductionCompanyId { get; set; } // foreign key to ProductionCompany
        public ProductionCompany ProductionCompany_R { get; set; } = null!; // one-to-Many with ProductionCompany

        public MovieDetail MovieDetail_R { get; set; } // one-to-one with MovieDetail
        public List<MovieDistribution> MovieDistribution_R { get; } = []; // Many-to-Many with Distribution
    }
}