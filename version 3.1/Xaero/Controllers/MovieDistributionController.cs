using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xaero.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Xaero.Controllers
{
    public class MovieDistributionController : Controller
    {
        private MovieContext context;
        public MovieDistributionController(MovieContext mc)
        {
            context = mc;
        }

        public IActionResult Update(int id)
        {
            GetMovieDistribution(id);

            var movie = context.Movie.Where(a => a.Id == id).Include(s => s.MovieDetail_R).FirstOrDefault();
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Movie movie, string[] distribution)
        {
            GetMovieDistribution(movie.Id);

            if (ModelState.IsValid)
            {
                context.RemoveRange(context.MovieDistribution.Where(t => t.MovieId == movie.Id).ToList());

                List<MovieDistribution> mdList = new List<MovieDistribution>();

                foreach (string d in distribution)
                {
                    var md = new MovieDistribution()
                    {
                        MovieId = movie.Id,
                        DistributionId = Convert.ToInt32(d)
                    };
                    mdList.Add(md);
                }

                context.AddRange(mdList);

                await context.SaveChangesAsync();

                return RedirectToAction("Index", "Movie");
            }
            else
                return View(movie);
        }

        void GetMovieDistribution(int movie)
        {
            List<SelectListItem> md = new List<SelectListItem>();
            md = context.Distribution.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.MovieDistribution_R.Where(y => y.MovieId == movie).Any(z => z.DistributionId == x.Id) }).ToList();

            ViewBag.MD = md;
        }
    }
}