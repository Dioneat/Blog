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

namespace Test.Controllers
{
    public class AdditionalInformationController : Controller
    {
        private ApplicationDbContext db;
        IWebHostEnvironment webHost;
        public IActionResult ForParents()
        {
            return View(db.Posts.ToList());
        }
        public AdditionalInformationController(ApplicationDbContext context, IWebHostEnvironment web)
        {
            db = context;
            webHost = web;
        }
        public async Task<ActionResult> Post(int? id)
        {
            if (id != null)
            {
              
                Post post = await db.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (post != null)
                    return View(post);
            }
            return NotFound();
        }

        [Authorize(Roles = "admin, superadmin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Post post) // ArticleVM
        {
            if (ModelState.IsValid)
            {
                if (post.File != null)
                {
                    string wwwRootPath = webHost.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(post.File.FileName);
                    string extension = Path.GetExtension(post.File.FileName);
                    post.Image = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/img/forparents/title/", fileName);
                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        await post.File.CopyToAsync(fs);
                    }
                }
                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return RedirectToAction("ForParents", "AdditionalInformation");
            }
            return NotFound();

        }

        [Authorize(Roles = "admin, superadmin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (post != null)
                    return View(post);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                    if (post.File != null)
                {
                    string wwwRootPath = webHost.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(post.File.FileName);
                    string extension = Path.GetExtension(post.File.FileName);
                    post.Image = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/img/forparents/title/", fileName);
                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        await post.File.CopyToAsync(fs);
                    }
                }
                
                db.Posts.Update(post);

                await db.SaveChangesAsync();
                return RedirectToAction("ForParents", "AdditionalInformation");

            }

            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Post post = await db.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (post.Image != null)
                {
                    var path = Path.Combine(webHost.WebRootPath, "/img/forparents/title/", post.Image);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                db.Entry(post).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("ForParents", "AdditionalInformation");
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
                string serverMapPath = Path.Combine(wwwRootPath, "/img/forparents/content/", fileName);

                using (var fs = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(fs);
                }
                path = "https://elenatimofeeva.ru/" + "img/forparents/content/" + fileName;





            }
            return Json(new { url = path });
        }


        public IActionResult Other()
        {
            return View();
        }
    }
}
