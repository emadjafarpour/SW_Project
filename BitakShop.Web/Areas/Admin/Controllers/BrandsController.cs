using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitakShop.Core.Models;
using BitakShop.Infrastructure.Helpers;
using BitakShop.Infrastructure.Repositories;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class BrandsController : Controller
    {
        private readonly BrandsRepository _repo;
        public BrandsController(BrandsRepository repo)
        {
            _repo = repo;
        }
        public ActionResult Index()
        {
            return View(_repo.GetAll());
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Brand brand, HttpPostedFileBase BrandImage)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (BrandImage != null)
                {
                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(BrandImage.FileName);
                    BrandImage.SaveAs(Server.MapPath("/Files/BrandsImages/Temp/" + newFileName));
                    // Resize Image
                    ImageResizer image = new ImageResizer(400, 200, true);
                    image.Resize(Server.MapPath("/Files/BrandsImages/Temp/" + newFileName),
                        Server.MapPath("/Files/BrandsImages/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/BrandsImages/Temp/" + newFileName));

                    brand.Logo = newFileName;
                }
                #endregion

                _repo.Add(brand);
                return RedirectToAction("Index");
            }

            return View(brand);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand image = _repo.Get(id.Value);
            if (image == null)
            {
                return HttpNotFound();
            }
            return PartialView(image);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Brand brand, HttpPostedFileBase BrandImage)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (BrandImage != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/BrandsImages/" + brand.Logo)))
                        System.IO.File.Delete(Server.MapPath("/Files/BrandsImages/" + brand.Logo));

                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(BrandImage.FileName);
                    BrandImage.SaveAs(Server.MapPath("/Files/BrandsImages/Temp/" + newFileName));
                    // Resize Image
                    ImageResizer image = new ImageResizer(400, 200, true);
                    image.Resize(Server.MapPath("/Files/BrandsImages/Temp/" + newFileName),
                        Server.MapPath("/Files/BrandsImages/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/BrandsImages/Temp/" + newFileName));
                    brand.Logo = newFileName;
                }
                #endregion

                _repo.Update(brand);
                return RedirectToAction("Index");
            }
            return View(brand);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand image = _repo.Get(id.Value);
            if (image == null)
            {
                return HttpNotFound();
            }
            return PartialView(image);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var image = _repo.Get(id);

            //#region Delete Image
            //if (image.Image != null)
            //{
            //    if (System.IO.File.Exists(Server.MapPath("/Files/BrandsImages/" + image.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/BrandsImages/" + image.Image));

            //    if (System.IO.File.Exists(Server.MapPath("/Files/BrandsImages/" + image.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/BrandsImages/" + image.Image));
            //}
            //#endregion

            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}