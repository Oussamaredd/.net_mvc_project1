using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhotoSharingApplication.Models;
using PhotoSharingApplication.Models.ViewModels;

namespace PhotoSharingApplication.Controllers
{
    [RoutePrefix("photos")]
    [HandleError(View = "Error")]
    [ValueReporter]
    public class PhotoController : Controller
    {
        private IPhotoSharingContext context;

        public PhotoController()
        {
            context = new PhotoSharingContext();
        }

        public PhotoController(IPhotoSharingContext Context)
        {
            context = Context;
        }

        // GET /photos?q=...&page=...&sort=...
        [HttpGet]
        [Route("")]
        public ActionResult Index(int page = 1, string q = null, string sort = "latest")
        {
            const int pageSize = 12;
            if (page < 1) page = 1;

            IQueryable<Photo> query = context.Photos;

            // Search (null-safe)
            if (!string.IsNullOrWhiteSpace(q))
            {
                string term = q.Trim();

                query = query.Where(p =>
                    (p.Title != null && p.Title.Contains(term)) ||
                    (p.UserName != null && p.UserName.Contains(term)) ||
                    (p.Description != null && p.Description.Contains(term))
                );

                q = term;
            }

            // Sort
            sort = string.IsNullOrWhiteSpace(sort) ? "latest" : sort.Trim().ToLowerInvariant();

            switch (sort)
            {
                case "oldest":
                    query = query.OrderBy(p => p.CreatedDate).ThenBy(p => p.PhotoID);
                    break;

                case "title":
                    query = query.OrderBy(p => p.Title).ThenByDescending(p => p.CreatedDate);
                    break;

                case "user":
                    query = query.OrderBy(p => p.UserName).ThenByDescending(p => p.CreatedDate);
                    break;

                case "latest":
                default:
                    query = query.OrderByDescending(p => p.CreatedDate).ThenByDescending(p => p.PhotoID);
                    sort = "latest";
                    break;
            }

            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            List<Photo> photos = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new PhotoIndexViewModel
            {
                Photos = photos,
                Query = q,
                Sort = sort,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return View("Index", vm);
        }

        // GET /photos/{id}
        [HttpGet]
        [Route("{id:int}")]
        public ActionResult Display(int id)
        {
            Photo photo = context.FindPhotoById(id);
            if (photo == null) return HttpNotFound();

            List<Comment> comments = context.Comments
                .Where(c => c.PhotoID == id)
                .OrderByDescending(c => c.CommentID)
                .ToList();

            var vm = new PhotoDisplayViewModel
            {
                Photo = photo,
                Comments = comments,
                NewComment = new Comment { PhotoID = id }
            };

            return View("Display", vm);
        }

        // POST /photos/{id}/comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id:int}/comments")]
        public ActionResult AddComment(int id, [Bind(Prefix = "NewComment")] Comment comment)
        {
            Photo photo = context.FindPhotoById(id);
            if (photo == null) return HttpNotFound();

            comment.PhotoID = id;

            if (string.IsNullOrWhiteSpace(comment.UserName))
                comment.UserName = "Anonymous";

            if (!ModelState.IsValid)
            {
                var vm = new PhotoDisplayViewModel
                {
                    Photo = photo,
                    Comments = context.Comments.Where(c => c.PhotoID == id)
                                              .OrderByDescending(c => c.CommentID)
                                              .ToList(),
                    NewComment = comment
                };

                return View("Display", vm);
            }

            context.Add(comment);
            context.SaveChanges();

            return RedirectToAction("Display", new { id = id });
        }

        // GET /photos/create
        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            var newPhoto = new Photo { CreatedDate = DateTime.Today };
            return View("Create", newPhoto);
        }

        // POST /photos/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public ActionResult Create(Photo photo, HttpPostedFileBase image)
        {
            photo.CreatedDate = DateTime.Today;

            if (image == null || image.ContentLength == 0)
            {
                ModelState.AddModelError("PhotoFile", "Please choose an image file.");
            }
            else
            {
                const int maxBytes = 5 * 1024 * 1024; // 5 MB
                if (image.ContentLength > maxBytes)
                    ModelState.AddModelError("PhotoFile", "Image is too large. Max size is 5 MB.");

                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "image/jpeg",
                    "image/png",
                    "image/webp"
                };

                if (!allowed.Contains(image.ContentType))
                    ModelState.AddModelError("PhotoFile", "Only JPG, PNG, or WEBP images are allowed.");

                if (ModelState.IsValid)
                {
                    photo.ImageMimeType = image.ContentType;
                    photo.PhotoFile = new byte[image.ContentLength];
                    image.InputStream.Read(photo.PhotoFile, 0, image.ContentLength);
                }
            }

            if (!ModelState.IsValid)
                return View("Create", photo);

            context.Add(photo);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET /photos/{id}/delete
        [HttpGet]
        [Route("{id:int}/delete")]
        public ActionResult Delete(int id)
        {
            Photo photo = context.FindPhotoById(id);
            if (photo == null) return HttpNotFound();

            return View("Delete", photo);
        }

        // POST /photos/{id}/delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Route("{id:int}/delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = context.FindPhotoById(id);
            if (photo == null) return HttpNotFound();

            context.Delete(photo);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET /photos/{id}/image
        [HttpGet]
        [Route("{id:int}/image")]
        public ActionResult GetImage(int id)
        {
            Photo photo = context.FindPhotoById(id);
            if (photo == null || photo.PhotoFile == null) return HttpNotFound();

            return File(photo.PhotoFile, photo.ImageMimeType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = context as System.IDisposable;
                if (disposable != null) disposable.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
