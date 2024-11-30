using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Xaero.Models;
using Microsoft.EntityFrameworkCore;
using Xaero.Infrastructure;
using System.Text.RegularExpressions;

namespace Xaero.Controllers
{
    public class MovieController : Controller
    {
        private MovieContext context;
        private IWebHostEnvironment hostingEnvironment;
        public MovieController(MovieContext mc, IWebHostEnvironment environment)
        {
            context = mc;
            hostingEnvironment = environment;
        }

        public IActionResult Index(int id)
        {
            string cookieValueFromReq = Request.Cookies["sortCookie"];

            List<Movie> mList;
            if (string.IsNullOrEmpty(cookieValueFromReq))
                mList = GetRecords(id);
            else
            {
                string sortColumn = cookieValueFromReq.Split(',')[0];
                string sortValue = cookieValueFromReq.Split(',')[1];

                mList = GetRecords(id, sortColumn, sortValue);
            }
            return View(mList);
        }

        List<Movie> GetRecords(int page, string sortColumn = "", string sortValue = "")
        {
            int pageSize = 3;

            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.CurrentPage = page == 0 ? 1 : page;
            pagingInfo.TotalItems = context.Movie.Count();
            pagingInfo.ItemsPerPage = pageSize;

            var skip = pageSize * (Convert.ToInt32(page) - 1);
            ViewBag.PagingInfo = pagingInfo;

            List<Movie> result;

            var query = context.Movie.Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).AsQueryable();

            if (sortValue == "asc")
            {
                switch (sortColumn)
                {
                    case "Id":
                        query = query.OrderBy(s => s.Id);
                        break;
                    case "Name":
                        query = query.OrderBy(s => s.MovieDetail_R.Name);
                        break;
                    case "Budget":
                        query = query.OrderBy(s => s.MovieDetail_R.Budget);
                        break;
                    case "Gross":
                        query = query.OrderBy(s => s.MovieDetail_R.Gross);
                        break;
                    case "Release Date":
                        query = query.OrderBy(s => s.MovieDetail_R.ReleaseDate);
                        break;
                    default:
                        query = query.OrderBy(s => s.MovieDetail_R.Name);
                        break;
                }
            }
            else
            {
                switch (sortColumn)
                {
                    case "Id":
                        query = query.OrderByDescending(s => s.Id);
                        break;
                    case "Name":
                        query = query.OrderByDescending(s => s.MovieDetail_R.Name);
                        break;
                    case "Budget":
                        query = query.OrderByDescending(s => s.MovieDetail_R.Budget);
                        break;
                    case "Gross":
                        query = query.OrderByDescending(s => s.MovieDetail_R.Gross);
                        break;
                    case "Release Date":
                        query = query.OrderByDescending(s => s.MovieDetail_R.ReleaseDate);
                        break;
                    default:
                        query = query.OrderByDescending(s => s.MovieDetail_R.Name);
                        break;
                }
            }
            result = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public IActionResult Create()
        {
            GetProduction();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile Poster)
        {
            GetProduction();

            if (Poster == null)
                ModelState.AddModelError(nameof(movie.MovieDetail_R.Poster), "Please select Movie Poster");
            else
            {
                string path = "Images/Movie/" + Poster.FileName;
                using (var stream = new FileStream(Path.Combine(hostingEnvironment.WebRootPath, path), FileMode.Create))
                {
                    await Poster.CopyToAsync(stream);
                }
                movie.MovieDetail_R.Poster = "~/" + path;
                ModelState.Remove(nameof(movie.MovieDetail_R.Poster));
            }

            if (ModelState.IsValid)
            {
                var movieDetail = new MovieDetail()
                {
                    Name = movie.MovieDetail_R.Name,
                    Poster = movie.MovieDetail_R.Poster,
                    Budget = movie.MovieDetail_R.Budget,
                    Gross = movie.MovieDetail_R.Gross,
                    ReleaseDate = movie.MovieDetail_R.ReleaseDate
                };

                var m = new Movie()
                {
                    ProductionCompanyId = movie.ProductionCompanyId,
                    MovieDetail_R = movieDetail
                };

                context.Add(m);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
                return View();
        }

        void GetProduction()
        {
            List<SelectListItem> production = new List<SelectListItem>();
            production = context.ProductionCompany.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.Production = production;
        }

        public IActionResult Update(int id)
        {
            var movie = context.Movie.Where(a => a.Id == id).Include(s => s.MovieDetail_R).FirstOrDefault();
            GetProduction();

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Movie movie, IFormFile mPoster)
        {
            GetProduction();

            if (ModelState.IsValid)
            {
                string path = movie.MovieDetail_R.Poster;
                if (mPoster != null)
                {
                    path = "Images/Production/" + mPoster.FileName;
                    using (var stream = new FileStream(Path.Combine(hostingEnvironment.WebRootPath, path), FileMode.Create))
                    {
                        await mPoster.CopyToAsync(stream);
                    }
                }

                var md = context.MovieDetail.Where(a => a.MovieId == movie.Id).FirstOrDefault();
                md.Name = movie.MovieDetail_R.Name;
                md.Poster = path.Substring(0, 2) == "~/" ? path : "~/" + path;
                md.Budget = movie.MovieDetail_R.Budget;
                md.Gross = movie.MovieDetail_R.Gross;
                md.ReleaseDate = movie.MovieDetail_R.ReleaseDate;

                var m = new Movie()
                {
                    Id = movie.Id,
                    ProductionCompanyId = movie.ProductionCompanyId,
                    MovieDetail_R = md
                };

                context.Update(m);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
                return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = context.Movie.Where(a => a.Id == id).FirstOrDefault();
            context.Remove(movie);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
