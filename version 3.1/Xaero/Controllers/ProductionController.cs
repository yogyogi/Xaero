using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xaero.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Xaero.Infrastructure;

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

            if (sortColumn == "")
                result = context.ProductionCompany.Skip(skip).Take(pageSize).ToList();
            else
            {
                if (sortValue == "asc")
                {
                    switch (sortColumn)
                    {
                        case "Id":
                            result = context.ProductionCompany.OrderBy(s => s.Id).Skip(skip).Take(pageSize).ToList();
                            break;
                        case "Name":
                            result = context.ProductionCompany.OrderBy(s => s.Name).Skip(skip).Take(pageSize).ToList();
                            break;
                        case "Annual Revenue":
                            result = context.ProductionCompany.OrderBy(s => s.AnnualRevenue).Skip(skip).Take(pageSize).ToList();
                            break;
                        case "Establishment Date":
                            result = context.ProductionCompany.OrderBy(s => s.EstablishmentDate).Skip(skip).Take(pageSize).ToList();
                            break;
                        default:
                            result = context.ProductionCompany.OrderBy(s => s.Name).Skip(skip).Take(pageSize).ToList();
                            break;
                    }
                }
                else
                {
                    switch (sortColumn)
                    {
                        case "Id":
                            result = context.ProductionCompany.OrderByDescending(s => s.Id).Skip(skip).Take(pageSize).ToList();
                            break;
                        case "Name":
                            result = context.ProductionCompany.OrderByDescending(s => s.Name).Skip(skip).Take(pageSize).ToList();
                            break;
                        case "Annual Revenue":
                            result = context.ProductionCompany.OrderByDescending(s => s.AnnualRevenue).Skip(skip).Take(pageSize).ToList();
                            break;
                        case "Establishment Date":
                            result = context.ProductionCompany.OrderByDescending(s => s.EstablishmentDate).Skip(skip).Take(pageSize).ToList();
                            break;
                        default:
                            result = context.ProductionCompany.OrderByDescending(s => s.Name).Skip(skip).Take(pageSize).ToList();
                            break;
                    }
                }
            }

            return result;
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

            if (ModelState.IsValid)
            {
                string path = "Images/Production/" + Logo.FileName;
                using (var stream = new FileStream(Path.Combine(hostingEnvironment.WebRootPath, path), FileMode.Create))
                {
                    await Logo.CopyToAsync(stream);
                }

                var productionCompany = new ProductionCompany()
                {
                    Name = pc.Name,
                    Logo = "~/" + path,
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

        public IActionResult Update(int id)
        {
            var pc = context.ProductionCompany.Where(a => a.Id == id).FirstOrDefault();
            return View(pc);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductionCompany pc, IFormFile mLogo)
        {
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