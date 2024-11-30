using Microsoft.AspNetCore.Mvc;
using Xaero.Models;
using Microsoft.EntityFrameworkCore;
using Xaero.Infrastructure;

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
            int pageSize = 1;

            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.CurrentPage = page == 0 ? 1 : page;
            pagingInfo.TotalItems = context.Distribution.Count();
            pagingInfo.ItemsPerPage = pageSize;

            var skip = pageSize * (Convert.ToInt32(page) - 1);
            ViewBag.PagingInfo = pagingInfo;

            List<Distribution> result;

            var query = context.Distribution.Include(s => s.MovieDistribution_R).ThenInclude(r => r.Movie_R).ThenInclude(t => t.MovieDetail_R).AsQueryable();

            if (sortValue == "asc")
            {
                switch (sortColumn)
                {
                    case "Id":
                        query = query.OrderBy(s => s.Id);
                        break;
                    case "query":
                        query = query.OrderBy(s => s.Name);
                        break;
                    case "Location":
                        query = query.OrderBy(s => s.Location);
                        break;
                    default:
                        query = query.OrderBy(s => s.Name);
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
                        query = query.OrderByDescending(s => s.Name);
                        break;
                    case "Location":
                        query = query.OrderByDescending(s => s.Location);
                        break;
                    default:
                        query = query.OrderByDescending(s => s.Name);
                        break;
                }
            }
            result = query.Skip(skip).Take(pageSize).ToList();

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
