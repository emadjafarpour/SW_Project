using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Core.Models;
using System.Net;
using System.IO;
using BitakShop.Infrastructure.Helpers;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ProductGalleriesController : Controller
    {
        private readonly ProductGalleriesRepository _repo;
        public ProductGalleriesController(ProductGalleriesRepository repo)
        {
            _repo = repo;
        }
        public ActionResult Index(int productId)
        {
            ViewBag.ProductName = _repo.GetProductName(productId);
            ViewBag.ProductId = productId;
            return View(_repo.GetProductGalleries(productId));
        }

        public ActionResult Create(int productId)
        {
            ViewBag.ProductId = productId;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductGallery productGallery, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (Image != null)
                {
                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(Image.FileName);
                    Image.SaveAs(Server.MapPath("/Files/ProductImages/Temp/" + newFileName));
                    // Resize Image
                    ImageResizer image = new ImageResizer(750, 900, true);
                    image.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileName),
                        Server.MapPath("/Files/ProductImages/ProductGallery/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Temp/" + newFileName));

                    productGallery.Image = newFileName;
                }
                #endregion
                _repo.Add(productGallery);
                return RedirectToAction("Index", new { productId = productGallery.ProductId });
            }
            ViewBag.ProductId = productGallery.ProductId;
            return View(productGallery);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGallery productGallery = _repo.Get(id.Value);
            if (productGallery == null)
            {
                return HttpNotFound();
            }
            return PartialView(productGallery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductGallery productGallery, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (Image != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/ProductImages/ProductGallery/" + productGallery.Image)))
                        System.IO.File.Delete(Server.MapPath("/Files/ProductImages/ProductGallery/" + productGallery.Image));
                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(Image.FileName);
                    Image.SaveAs(Server.MapPath("/Files/ProductImages/Temp/" + newFileName));
                    // Resize Image
                    ImageResizer image = new ImageResizer(750, 900, true);
                    image.Resize(Server.MapPath("/Files/ProductImages/Temp/" + newFileName),
                        Server.MapPath("//Files/ProductImages/ProductGallery/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/ProductImages/Temp/" + newFileName));

                    productGallery.Image = newFileName;
                }
                #endregion
                _repo.Update(productGallery);
                return RedirectToAction("Index", new { productId = productGallery.ProductId });
            }
            return View(productGallery);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGallery productGallery = _repo.Get(id.Value);
            if (productGallery == null)
            {
                return HttpNotFound();
            }
            return PartialView(productGallery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var productId = _repo.Get(id).ProductId;
            _repo.Delete(id);
            return RedirectToAction("Index", new { productId });
        }
    }
}