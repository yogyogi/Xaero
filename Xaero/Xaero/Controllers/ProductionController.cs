using Microsoft.AspNetCore.Mvc;
using Xaero.Models;
using Xaero.Infrastructure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;

namespace Xaero.Controllers
{
    public class ProductionController : Controller
    {
        private MovieContext context;
        private IWebHostEnvironment hostingEnvironment;
        public ProductionController(MovieContext mc, IWebHostEnvironment environment)
        {
            context = mc;
            hostingEnvironment = environment;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductionCompany pc, IFormFile Logo)
        {
            if (Logo == null)
                ModelState.AddModelError(nameof(pc.Logo), "Please select logo file");
            else
            {
                string path = "Images/Production/" + Logo.FileName;
                using (var stream = new FileStream(Path.Combine(hostingEnvironment.WebRootPath, path), FileMode.Create))
                {
                    await Logo.CopyToAsync(stream);
                }
                pc.Logo = "~/" + path;
                ModelState.Remove(nameof(pc.Logo));
            }

            if (ModelState.IsValid)
            {
                var productionCompany = new ProductionCompany()
                {
                    Name = pc.Name,
                    Logo = pc.Logo,
                    AnnualRevenue = pc.AnnualRevenue,
                    EstablishmentDate = pc.EstablishmentDate
                };

                context.Add(productionCompany);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
                return View();
        }

        public IActionResult Index(int id)
        {
            string cookieValueFromReq = Request.Cookies["sortCookie"];

            List<ProductionCompany> pcList;
            if (string.IsNullOrEmpty(cookieValueFromReq))
                pcList = GetRecords(id);
            else
            {
                string sortColumn = cookieValueFromReq.Split(',')[0];
                string sortValue = cookieValueFromReq.Split(',')[1];

                pcList = GetRecords(id, sortColumn, sortValue);
            }
            return View(pcList);
        }

        List<ProductionCompany> GetRecords(int page, string sortColumn = "", string sortValue = "")
        {
            int pageSize = 3;

            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.CurrentPage = page == 0 ? 1 : page;
            pagingInfo.TotalItems = context.ProductionCompany.Count();
            pagingInfo.ItemsPerPage = pageSize;

            var skip = pageSize * (Convert.ToInt32(page) - 1);
            ViewBag.PagingInfo = pagingInfo;

            List<ProductionCompany> result;

            var query = context.ProductionCompany.AsQueryable();

            if (sortValue == "asc")
            {
                switch (sortColumn)
                {
                    case "Id":
                        query = query.OrderBy(s => s.Id);
                        break;
                    case "Name":
                        query = query.OrderBy(s => s.Name);
                        break;
                    case "Annual Revenue":
                        query = query.OrderBy(s => s.AnnualRevenue);
                        break;
                    case "Establishment Date":
                        query = query.OrderBy(s => s.EstablishmentDate);
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
                    case "Annual Revenue":
                        query = query.OrderByDescending(s => s.AnnualRevenue);
                        break;
                    case "Establishment Date":
                        query = query.OrderByDescending(s => s.EstablishmentDate);
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
            var pc = context.ProductionCompany.Where(a => a.Id == id).FirstOrDefault();
            return View(pc);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductionCompany pc, IFormFile mLogo)
        {
            ModelState.Remove(nameof(mLogo));
            if (ModelState.IsValid)
            {
                string path = pc.Logo;
                if (mLogo != null)
                {
                    path = "Images/Production/" + mLogo.FileName;
                    using (var stream = new FileStream(Path.Combine(hostingEnvironment.WebRootPath, path), FileMode.Create))
                    {
                        await mLogo.CopyToAsync(stream);
                    }
                }

                var productionCompany = new ProductionCompany()
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    Logo = path.Substring(0, 2) == "~/" ? path : "~/" + path,
                    AnnualRevenue = pc.AnnualRevenue,
                    EstablishmentDate = pc.EstablishmentDate
                };

                context.Update(productionCompany);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
                return View(pc);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var pc = context.ProductionCompany.Where(a => a.Id == id).FirstOrDefault();
            context.Remove(pc);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
