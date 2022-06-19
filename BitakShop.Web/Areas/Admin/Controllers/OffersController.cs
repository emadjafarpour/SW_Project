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
    public class OffersController : Controller
    {
        private readonly OffersRepository _repo;
        public OffersController(OffersRepository repo)
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
        public ActionResult Create(Offer offer, HttpPostedFileBase OfferImage)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (OfferImage != null)
                {
                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(OfferImage.FileName);
                    OfferImage.SaveAs(Server.MapPath("/Files/OffersImages/Temp/" + newFileName));
                    // Resize Image
                    ImageResizer image = new ImageResizer(1600, 900);
                    image.Resize(Server.MapPath("/Files/OffersImages/Temp/" + newFileName),
                        Server.MapPath("/Files/OffersImages/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/OffersImages/Temp/" + newFileName));

                    offer.Image = newFileName;
                }
                #endregion

                _repo.Add(offer);
                return RedirectToAction("Index");
            }

            return View(offer);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offer image = _repo.Get(id.Value);
            if (image == null)
            {
                return HttpNotFound();
            }
            return PartialView(image);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Offer offer, HttpPostedFileBase OfferImage)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (OfferImage != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/OffersImages/" + offer.Image)))
                        System.IO.File.Delete(Server.MapPath("/Files/OffersImages/" + offer.Image));

                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(OfferImage.FileName);
                    OfferImage.SaveAs(Server.MapPath("/Files/OffersImages/Temp/" + newFileName));
                    // Resize Image
                    ImageResizer image = new ImageResizer(1600, 900);
                    image.Resize(Server.MapPath("/Files/OffersImages/Temp/" + newFileName),
                        Server.MapPath("/Files/OffersImages/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/OffersImages/Temp/" + newFileName));
                    offer.Image = newFileName;
                }
                #endregion

                _repo.Update(offer);
                return RedirectToAction("Index");
            }
            return View(offer);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Offer image = _repo.Get(id.Value);
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
            //    if (System.IO.File.Exists(Server.MapPath("/Files/OffersImages/" + image.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/OffersImages/" + image.Image));

            //    if (System.IO.File.Exists(Server.MapPath("/Files/OffersImages/" + image.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/OffersImages/" + image.Image));
            //}
            //#endregion

            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}