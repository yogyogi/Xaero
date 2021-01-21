using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Xaero.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Xaero.Infrastructure;
using Microsoft.EntityFrameworkCore;

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

            if (sortColumn == "")
                result = context.Movie.Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
            else
            {
                if (sortValue == "asc")
                {
                    switch (sortColumn)
                    {
                        case "Id":
                            result = context.Movie.OrderBy(s => s.Id).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        case "Name":
                            result = context.Movie.OrderBy(s => s.MovieDetail_R.Name).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        case "Budget":
                            result = context.Movie.OrderBy(s => s.MovieDetail_R.Budget).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        case "Gross":
                            result = context.Movie.OrderBy(s => s.MovieDetail_R.Gross).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        case "Release Date":
                            result = context.Movie.OrderBy(s => s.MovieDetail_R.ReleaseDate).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        default:
                            result = context.Movie.OrderBy(s => s.MovieDetail_R.Name).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                    }
                }
                else
                {
                    switch (sortColumn)
                    {
                        case "Id":
                            result = context.Movie.OrderByDescending(s => s.Id).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        case "Name":
                            result = context.Movie.OrderByDescending(s => s.MovieDetail_R.Name).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        case "Budget":
                            result = context.Movie.OrderByDescending(s => s.MovieDetail_R.Budget).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        case "Gross":
                            result = context.Movie.OrderByDescending(s => s.MovieDetail_R.Gross).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        case "Release Date":
                            result = context.Movie.OrderByDescending(s => s.MovieDetail_R.ReleaseDate).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                        default:
                            result = context.Movie.OrderByDescending(s => s.MovieDetail_R.Name).Skip(skip).Take(pageSize).Include(s => s.MovieDetail_R).Include(s => s.ProductionCompany_R).ToList();
                            break;
                    }
                }
            }

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
                ModelState.AddModelError("MovieDetail_R.Poster", "Please select Movie Poster");

            if (ModelState.IsValid)
            {
                string path = "Images/Movie/" + Poster.FileName;
                using (var stream = new FileStream(Path.Combine(hostingEnvironment.WebRootPath, path), FileMode.Create))
                {
                    await Poster.CopyToAsync(stream);
                }

                var movieDetail = new MovieDetail()
                {
                    Name = movie.MovieDetail_R.Name,
                    Poster = "~/" + path,
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

                var movieDetail = new MovieDetail()
                {
                    MovieId = movie.Id,
                    Name = movie.MovieDetail_R.Name,
                    Poster = path.Substring(0, 2) == "~/" ? path : "~/" + path,
                    Budget = movie.MovieDetail_R.Budget,
                    Gross = movie.MovieDetail_R.Gross,
                    ReleaseDate = movie.MovieDetail_R.ReleaseDate
                };

                var m = new Movie()
                {
                    Id = movie.Id,
                    ProductionCompanyId = movie.ProductionCompanyId,
                    MovieDetail_R = movieDetail
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