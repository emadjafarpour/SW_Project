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
    public class SubFeaturesController : Controller
    {
        private readonly SubFeaturesRepository _repo;
        public SubFeaturesController(SubFeaturesRepository repo)
        {
            _repo = repo;
        }
        // GET: Admin/SubFeatures
        public ActionResult Index(int featureId)
        {
            ViewBag.FeatureName = _repo.GetFeatureName(featureId);
            ViewBag.FeatureId = featureId;
            return View(_repo.GetSubFeatures(featureId));
        }

        // GET: Admin/SubFeatures/Create
        public ActionResult Create(int featureId)
        {
            ViewBag.FeatureId = featureId;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubFeature subFeature, HttpPostedFileBase SubFeatureImage)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (SubFeatureImage != null)
                {
                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(SubFeatureImage.FileName);
                    SubFeatureImage.SaveAs(Server.MapPath("/Files/SubFeaturesImages/Temp/" + newFileName));
                    // Resize Image
                    ImageResizer image = new ImageResizer(1920, 1080);
                    image.Resize(Server.MapPath("/Files/SubFeaturesImages/Temp/" + newFileName),
                        Server.MapPath("/Files/SubFeaturesImages/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/SubFeaturesImages/Temp/" + newFileName));

                    subFeature.Image = newFileName;
                }
                #endregion
                _repo.Add(subFeature);
                return RedirectToAction("Index",new {featureId = subFeature.FeatureId});
            }

            return View(subFeature);
        }

        // GET: Admin/SubFeatures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubFeature subFeature = _repo.Get(id.Value);
            if (subFeature == null)
            {
                return HttpNotFound();
            }
            return PartialView(subFeature);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SubFeature subFeature, HttpPostedFileBase SubFeatureImage)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (SubFeatureImage != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/SubFeaturesImages/" + subFeature.Image)))
                        System.IO.File.Delete(Server.MapPath("/Files/SubFeaturesImages/" + subFeature.Image));

                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(SubFeatureImage.FileName);
                    SubFeatureImage.SaveAs(Server.MapPath("/Files/SubFeaturesImages/Temp/" + newFileName));
                    // Resize Image
                    ImageResizer image = new ImageResizer(1920, 1080);
                    image.Resize(Server.MapPath("/Files/SubFeaturesImages/Temp/" + newFileName),
                        Server.MapPath("/Files/SubFeaturesImages/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/SubFeaturesImages/Temp/" + newFileName));

                    subFeature.Image = newFileName;
                }
                #endregion

                _repo.Update(subFeature);
                return RedirectToAction("Index", new { featureId = subFeature.FeatureId });
            }
            return View(subFeature);
        }

        // GET: Admin/SubFeatures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubFeature subFeature = _repo.Get(id.Value);
            if (subFeature == null)
            {
                return HttpNotFound();
            }
            return PartialView(subFeature);
        }

        // POST: Admin/SubFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var featureId = _repo.Get(id).FeatureId;
            _repo.Delete(id);
            return RedirectToAction("Index", new { featureId });
        }
    }
}