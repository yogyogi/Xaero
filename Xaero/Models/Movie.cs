using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xaero.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public int ProductionCompanyId { get; set; }

        public ProductionCompany ProductionCompany_R { get; set; } // one-to-Many with ProductionCompany
        public MovieDetail MovieDetail_R { get; set; } // one-to-one with MovieDetail
        public IList<MovieDistribution> MovieDistribution_R { get; set; } // Many-to-Many with Distribution
    }
}
