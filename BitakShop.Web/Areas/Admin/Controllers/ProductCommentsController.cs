using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Core.Models;
using System.Net;
using BitakShop.Web.ViewModels;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ProductCommentsController : Controller
    {
        private readonly ProductCommentsRepository _repo;
        public ProductCommentsController(ProductCommentsRepository repo)
        {
            _repo = repo;
        }
        public ActionResult Index(int productId)
        {
            ViewBag.ProductName = _repo.GetProductName(productId);
            ViewBag.ProductId = productId;
            var comments = _repo.GetProductComments(productId);
            var commentsVm = new List<ProductCommentWithPersianDateViewModel>();
            foreach (var comment in comments)
            {
                var commentVm = new ProductCommentWithPersianDateViewModel(comment);
                commentsVm.Add(commentVm);
            }
            return View(commentsVm);
        }

        public ActionResult Create(int productId)
        {
            ViewBag.ProductId = productId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductComment comment)
        {
            if (ModelState.IsValid)
            {
                comment.AddedDate = DateTime.Now;
                _repo.Add(comment);
                return RedirectToAction("Index", new { productId = comment.ProductId });
            }
            ViewBag.ProductId = comment.ProductId;
            return View(comment);
        }
        public ActionResult AnswerComment(int productId, int parentCommentId)
        {
            ViewBag.ProductId = productId;
            ViewBag.ParentId = parentCommentId;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AnswerComment(ProductComment comment)
        {
            var user = _repo.GetCurrentUser();
            comment.Name = user != null ? $"{user.FirstName} {user.LastName}" : "ادمین";
            comment.Email = user != null ? user.Email : "-";
            comment.AddedDate = DateTime.Now;
            _repo.Add(comment);
            return RedirectToAction("Index", new { productId = comment.ProductId });
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductComment comment = _repo.Get(id.Value);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductComment comment)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(comment);
                return RedirectToAction("Index", new { productId = comment.ProductId });
            }
            return View(comment);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductComment comment = _repo.Get(id.Value);
            if (comment == null)
            {
                return HttpNotFound();
            }

            var commentVm = new ProductCommentWithPersianDateViewModel(comment);
            return PartialView(commentVm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var productId = _repo.Get(id).ProductId;
            _repo.DeleteComment(id);
            return RedirectToAction("Index", new { productId });
        }
    }
}