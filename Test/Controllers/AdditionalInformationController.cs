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
    public class AdditionalInformationController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHost;
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

        [Authorize(Roles = "admin")]
        public ActionResult Create() 
        {
            var post = new PostViewModel();
            var postTags = db.Articles.Select(p => p.Tags).ToList();
            var tags = postTags;
            tags.AddRange(db.Posts.Select(p => p.Tags));
            var tag = new List<string>();
            foreach (var item in tags)
            {
                if (item != null)
                    tag.AddRange(item.Split('~'));
            }
            post.PostTags = String.Join("~", postTags);
            post.Tags = tag;
            return View(post);
        }

        [HttpPost]
        public async Task<ActionResult> Create(PostViewModel pvm) // 
        {
            if (ModelState.IsValid)
            {
                if (pvm.File != null)
                {
                    var path = pvm.ImageName = InitializePath(pvm.File.FileName, "/img/forparents/title/");
                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        await pvm.File.CopyToAsync(fs);
                    }
                }
                var post = new Post
                {
                    Title = pvm.Title,
                    Content = pvm.Content,
                    ShortDesc = pvm.ShortDesc,
                    Tags = string.Join("~", pvm.EditTags),
                    Image = pvm.ImageName
                };
                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return RedirectToAction("ForParents", "AdditionalInformation");
            }
            return NotFound();

        }

        private string InitializePath(string file, string path)
        {
            string wwwRootPath = webHost.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            return Path.Combine(wwwRootPath + path, fileName);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id != null)
            {
                var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (post != null)
                {
                    var pvm = new PostViewModel()
                    {
                        Title = post.Title,
                        Content = post.Content,
                        ImageName = post.Image,
                        ShortDesc = post.ShortDesc,
                        PostTags = post.Tags,
                        
                    };
                    var tags = new List<string>();
                    tags.AddRange(db.Posts.Select(p => p.Tags));
                    tags.AddRange(db.Articles.Select(p => p.Tags));
                    var tag = new List<string>();
                    foreach (var item in tags)
                    {
                        if (item != null)
                            tag.AddRange(item.Split('~'));
                    }
                    pvm.Tags = tag;
                    return View(pvm);
                }
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PostViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                if (pvm.File != null)
                {
                    string path = pvm.ImageName = InitializePath(pvm.File.FileName, "/img/forparents/title/");
                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        await pvm.File.CopyToAsync(fs);
                    }
                }
                var post = new Post
                {
                    Id = pvm.Id,
                    Title = pvm.Title,
                    Content = pvm.Content,
                    ShortDesc = pvm.ShortDesc,
                    Tags = string.Join("~", pvm.EditTags),
                    Image = pvm.ImageName
                };
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
                string serverMapPath = InitializePath(photo.FileName, "/img/forparents/content/");

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
