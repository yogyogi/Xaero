using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xaero.Models
{
    public class MovieDistribution
    {
        public int MovieId { get; set; } //foreign key property
        public int DistributionId { get; set; } //foreign key property

        public Movie Movie_R { get; set; } //Reference navigation property
        public Distribution Distribution_R { get; set; } //Reference navigation property
    }
}
