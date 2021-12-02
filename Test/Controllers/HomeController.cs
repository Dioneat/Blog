using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;
using Test.ViewModels;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        IWebHostEnvironment webHost;
        private ApplicationDbContext db;
        private readonly EmailService service;
       
        public HomeController(IWebHostEnvironment webHost, ApplicationDbContext context, EmailService service)
        {
            this.service = service;
            this.webHost = webHost;
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            var source = db.Articles;
            var items = await source.Take(3).ToListAsync();
            var sections = db.Sections.ToList();

            var hvm = new HomeViewModel()
            {
                IndexSection = sections,
                Articles = items
            };

            return View(hvm);
        }
        [Authorize(Roles = "admin, superadmin")]
        public IActionResult CreateSection()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateSection(IndexSection section)
        {
            if (ModelState.IsValid)
            {

                db.Sections.Add(section);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return NotFound();

        }
        [Authorize(Roles = "admin, superadmin")]
        public IActionResult EditSection(int? id)
        {
            var section = db.Sections.FirstOrDefault(p => p.Id == id);
            return View(section);

        }
        [HttpPost]
        public async Task<IActionResult> EditSection(IndexSection section)
        {
            if (ModelState.IsValid)
            {
                db.Sections.Update(section);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> DeleteSection(int? id)
        {
            if (id != null)
            {
                IndexSection section = await db.Sections.FirstOrDefaultAsync(p => p.Id == id);
                db.Entry(section).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return NotFound();
        }
        public IActionResult About()
        {
            return View(db.About.FirstOrDefault());
        }

        [Authorize(Roles = "admin, superadmin")]
        public IActionResult EditAbout()
        {
            return View(db.About.FirstOrDefault());

        }
        [HttpPost]
        public async Task<IActionResult> EditAbout(AboutSection about)
        {
            if (ModelState.IsValid)
            {
                bool hasImage = false;
                var section = db.About.FirstOrDefault();
                var fileNames = new List<string>();
                if(about.EditImages != null)
                    fileNames.AddRange(about.EditImages);
                if (about.FileImages != null)
                {
                    hasImage = true;
                    foreach (var image in about.FileImages)
                    {
                        if (image != null)
                        {
                            string wwwRootPath = webHost.WebRootPath;
                            string fileName = Path.GetFileNameWithoutExtension(image.FileName);
                            string extension = Path.GetExtension(image.FileName);
                            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                            string path = Path.Combine(webHost.WebRootPath + "/img/about/portfolio/", fileName);
                            using (var fs = new FileStream(path, FileMode.Create))
                            {
                                await image.CopyToAsync(fs);
                            }
                            fileNames.Add(fileName);
                        }

                    }
                }
                if (section != null)
                {
                    section.Content = about.Content;
                    if (hasImage is true)
                    {
                        section.Description = string.Join('~', about.EditDescription);

                        section.Images = string.Join('~', fileNames);

                    }

                    db.About.Update(section);
                }
                else
                {
                    if (hasImage is true)
                    {
                        about.Description = string.Join('~', about.EditDescription);
                        about.Images = string.Join('~', fileNames);
                    }

                    db.About.Update(about);
                }

                await db.SaveChangesAsync();
                return RedirectToAction("About", "Home");
            }
            return NotFound();
        }
        public async Task<IActionResult> Blog(string tag, string name, int page = 1)
        {
            int pageSize = 6; 

            IQueryable<Article> source = db.Articles;

            var hs = new HashSet<string>();
            
            foreach (var article in source)
            {
                if(article.Tags != null)
                {
                    foreach (var t in article.Tags.Split('~'))
                    {
                        hs.Add(t);
                    }
                }
                
            }
            if (!String.IsNullOrEmpty(name))
            {
                source = source.Where(p => p.Title.Contains(name));
            }
            if (!String.IsNullOrEmpty(tag))
            {
                source = source.Where(p => p.Tags.Contains(tag));
            }
            

            var count = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                FilterViewModel = new FilterViewModel(hs, tag, name),
                Articles = items,
                Tags = hs
            };
            return View(viewModel);
        }
        public IActionResult Contacts()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendEmail(string name, string content, string subject, string email)
        {
            service.SendMail(email, content, subject, name);
            return RedirectToAction("Contacts", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
