using Blog10.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Blog10.Controllers
{
    public class SitemapController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SitemapController(AppDbContext context)
        {
            _context = context;
        }

        [Route("sitemap.xml")]
        public async Task<IActionResult> GetSitemap()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            AddUrl(sb, baseUrl, "", "1.0", "daily");
            AddUrl(sb, baseUrl, "/about", "0.8", "monthly");
            AddUrl(sb, baseUrl, "/reviews", "0.8", "weekly");
            AddUrl(sb, baseUrl, "/blog", "0.9", "daily");

            var posts = await _context.Articles
                .AsNoTracking()
                .Where(p => !p.IsDraft)
                .Select(p => new { p.Id, p.Slug, p.CreatedAt })
                .ToListAsync();

            foreach (var post in posts)
            {
                var slugPath = string.IsNullOrEmpty(post.Slug) ? post.Id.ToString() : $"{post.Id}/{post.Slug}";
                AddUrl(sb, baseUrl, $"/blog/{slugPath}", "0.7", "monthly");
            }

            sb.AppendLine("</urlset>");

            return Content(sb.ToString(), "application/xml", Encoding.UTF8);
        }

        private void AddUrl(StringBuilder sb, string baseUri, string path, string priority, string freq)
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{baseUri}{path}</loc>");
            sb.AppendLine($"    <changefreq>{freq}</changefreq>");
            sb.AppendLine($"    <priority>{priority}</priority>");
            sb.AppendLine("  </url>");
        }
    }
}