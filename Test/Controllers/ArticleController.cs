using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;
using Test.ViewModels;

namespace Test.Controllers
{
    public class ArticleController : Controller
    {
        private ApplicationDbContext db;
        IWebHostEnvironment webHost;
        public ArticleController(ApplicationDbContext context, IWebHostEnvironment web)
        {
            db = context;
            webHost = web;
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id != null)
            {
                var articles = db.Articles;
                Article article = await articles.FirstOrDefaultAsync(p => p.Id == id);
                var avm = new ArticleViewModel()
                {
                Id = article.Id,
                Title = article.Title,
                ImageName = article.ImageName,
                Date = article.Date,
                Content = article.Content,
                ShortDesc = article.ShortDesc,
                Author = article.Author,
                ArticleTags = article.Tags,
                Articles = articles.Where(p => p.Id != id).ToList()
            };
                if (avm != null)
                    return View(avm);
            }
            return NotFound();
        }

        [Authorize(Roles = "admin, superadmin")]
        public ActionResult Create()
        {
          
            var avm = new ArticleViewModel();
            var tags = db.Articles;
            var tag = new List<string>();
            foreach (var item in tags)
            {
                if(item.Tags != null)
                    tag.AddRange(item.Tags.Split('~'));
            }
            avm.Tags = tag;
            return View(avm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ArticleViewModel avm) // ArticleVM
        {
            if (ModelState.IsValid)
            {
                if(avm.ImageFile != null)
                {
                    
                    using (var fs = new FileStream(InitializeImage("/img/blog/title/", avm), FileMode.Create))
                    {
                        await avm.ImageFile.CopyToAsync(fs);
                    }
                }
                

                if(avm.EditTags != null)
                    avm.ArticleTags = string.Join('~', avm.EditTags);
                string name;
                if (User.Identity.Name == "Admin")
                    name = "Елена Тимофеева";
                else if (User.Identity.Name == "Superadmin")
                    name = "Администратор";
                else
                    name = "user";
                avm.Author = name;
                var article = new Article()
                {
                    Author = avm.Author,
                    Content = avm.Content.Replace("<img>", "<img class=\"img-fluid\">"),
                    Date = avm.Date,
                    ShortDesc = avm.ShortDesc,
                    ImageName = avm.ImageName,
                    Tags = avm.ArticleTags,
                    Title = avm.Title
                };
               
                db.Articles.Add(article);
                await db.SaveChangesAsync();
                return RedirectToAction("Blog", "Home");
            }
            return NotFound();

        }

        [Authorize(Roles = "admin, superadmin")]
        public ActionResult Edit(int? id)
        {
            if (id != null)
            {
                var articles = db.Articles;
                var tag = new List<string>();
                foreach (var item in articles)
                {
                    if(item.Tags != null)
                        tag.AddRange(item.Tags.Split('~'));
                }
                var article = articles.FirstOrDefault(p => p.Id == id);
                var avm = new ArticleViewModel()
                {
                    Author = article.Author,
                    Content = article.Content,
                    Date = article.Date,
                    ShortDesc = article.ShortDesc,
                    ImageName = article.ImageName,
                    ArticleTags = article.Tags,
                    Tags = tag,
                    Title = article.Title
                };
                if (avm != null)
                    return View(avm);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<ActionResult> Edit(ArticleViewModel avm)
        {
            if(ModelState.IsValid)
            {
                if (avm.ImageFile != null)
                {
                    
                    using (var fs = new FileStream(InitializeImage("/img/blog/title/", avm), FileMode.Create))
                    {
                        await avm.ImageFile.CopyToAsync(fs);
                    }
                }
              
                avm.Author = User.Identity.Name;
                if(avm.EditTags != null)
                    avm.ArticleTags = string.Join('~', avm.EditTags);

                var article = new Article()
                {
                    Id = avm.Id,    
                    Author = avm.Author,
                    Content = avm.Content,
                    Date = avm.Date,
                    ShortDesc = avm.ShortDesc,
                    ImageName = avm.ImageName,
                    Tags = avm.ArticleTags,
                    Title = avm.Title
                };

                db.Articles.Update(article);

                await db.SaveChangesAsync();
                return RedirectToAction("Blog", "Home");

            }

            return NotFound();




        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                string wwwRootPath = webHost.WebRootPath;
                Article article = await db.Articles.FirstOrDefaultAsync(p => p.Id == id);
                if (article.ImageName != null)
                {
                    var path = Path.Combine(wwwRootPath, "/img/blog/title/", article.ImageName);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                db.Entry(article).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Blog", "Home");
            }
            return NotFound();
        }

       
        [HttpPost]
        public ActionResult UploadImage(List<IFormFile> files)
        {
            var path = "";
            foreach (var photo in Request.Form.Files)
            {
                string wwwRootPath = webHost.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(photo.FileName);
                string extension = Path.GetExtension(photo.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string serverMapPath = Path.Combine(wwwRootPath, "img/blog/content/", fileName);

                using (var fs = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(fs);
                }
                path = "https://elenatimofeeva.ru/" + "img/blog/content/" + fileName;
            }
            return Json(new { url = path });
        }
        public string InitializeImage(string path, in ArticleViewModel avm)
        {
            string wwwRootPath = webHost.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(avm.ImageFile.FileName);
            string extension = Path.GetExtension(avm.ImageFile.FileName);
            avm.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            return Path.Combine(wwwRootPath + path, fileName); ;
        }
    }
}
