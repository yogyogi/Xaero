using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xaero.Models;
using Xaero.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Xaero.Controllers
{
    public class DistributionController : Controller
    {
        private MovieContext context;
        public DistributionController(MovieContext mc)
        {
            context = mc;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Distribution distribution)
        {
            if (ModelState.IsValid)
            {
                context.Add(distribution);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
                return View();
        }

        public IActionResult Index(int id)
        {
            string cookieValueFromReq = Request.Cookies["sortCookie"];

            List<Distribution> dList;
            if (string.IsNullOrEmpty(cookieValueFromReq))
                dList = GetRecords(id);
            else
            {
                string sortColumn = cookieValueFromReq.Split(',')[0];
                string sortValue = cookieValueFromReq.Split(',')[1];

                dList = GetRecords(id, sortColumn, sortValue);
            }
            return View(dList);
        }

        List<Distribution> GetRecords(int page, string sortColumn = "", string sortValue = "")
        {
            int pageSize = 3;

            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.CurrentPage = page == 0 ? 1 : page;
            pagingInfo.TotalItems = context.Distribution.Count();
            pagingInfo.ItemsPerPage = pageSize;

            var skip = pageSize * (Convert.ToInt32(page) - 1);
            ViewBag.PagingInfo = pagingInfo;

            List<Distribution> result;

            if (sortColumn == "")
                result = context.Distribution.Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
            else
            {
                if (sortValue == "asc")
                {
                    switch (sortColumn)
                    {
                        case "Id":
                            result = context.Distribution.OrderBy(s => s.Id).Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
                            break;
                        case "Name":
                            result = context.Distribution.OrderBy(s => s.Name).Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
                            break;
                        case "Location":
                            result = context.Distribution.OrderBy(s => s.Location).Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
                            break;
                        default:
                            result = context.Distribution.OrderBy(s => s.Name).Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
                            break;
                    }
                }
                else
                {
                    switch (sortColumn)
                    {
                        case "Id":
                            result = context.Distribution.OrderByDescending(s => s.Id).Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
                            break;
                        case "Name":
                            result = context.Distribution.OrderByDescending(s => s.Name).Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
                            break;
                        case "Location":
                            result = context.Distribution.OrderByDescending(s => s.Location).Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
                            break;
                        default:
                            result = context.Distribution.OrderByDescending(s => s.Name).Skip(skip).Take(pageSize).Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).ToList();
                            break;
                    }
                }
            }

            return result;
        }

        public IActionResult Update(int id)
        {
            return View(context.Distribution.Where(a => a.Id == id).FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> Update(Distribution distribution)
        {
            if (ModelState.IsValid)
            {
                context.Update(distribution);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
                return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var distribution = context.Distribution.Where(a => a.Id == id).FirstOrDefault();
            context.Remove(distribution);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}