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
using BitakShop.Web.ViewModels;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ProductGroupsController : Controller
    {
        private readonly ProductGroupsRepository _repo;
        public ProductGroupsController(ProductGroupsRepository repo)
        {
            _repo = repo;
        }
        // GET: Admin/ProductGroups
        public ActionResult Index(int? parentId)
        {
            List<ProductGroup> productGroups;
            if (parentId == null)
                productGroups = _repo.GetProductGroupTable();
            else
            {
                productGroups = _repo.GetProductGroupTable(parentId.Value);
                var parent = _repo.Get(parentId.Value);
                ViewBag.PrevParent = parent.ParentId;
                ViewBag.ParentId = parentId;
                ViewBag.ParentName = parent.Title;
            }
            return View(productGroups);
        }
        // GET: Admin/ProductGroups/Create
        public ActionResult Create()
        {
            ViewBag.Features = _repo.GetFeatures();
            ViewBag.Brands = _repo.GetBrands();
            ViewBag.ProductGroups = _repo.GetProductGroups();
            return View();
        }

        [HttpPost]
        public int? Create(NewProductGroupViewModel productGroup)
        {
            if (ModelState.IsValid)
            {
                var product =_repo.AddNewProductGroup(productGroup.ParentGroupId,productGroup.Title,productGroup.BrandIds,productGroup.ProductGroupFeatureIds);
                return product.Id;
            }

            return null;
        }
        // GET: Admin/ProductGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroup productGroup = _repo.GetProductGroup(id.Value);
            if (productGroup == null)
            {
                return HttpNotFound();
            }

            ViewBag.Features = _repo.GetFeatures();
            ViewBag.Brands = _repo.GetBrands();
            ViewBag.ProductGroups = _repo.GetProductGroups();
            return View(productGroup);
        }

        [HttpPost]
        public int? Edit(UpdateProductGroupViewModel productGroup)
        {
            if (ModelState.IsValid)
            {
                var product = _repo.UpdateProductGroup(productGroup.ParentGroupId,productGroup.Id, productGroup.Title, productGroup.BrandIds, productGroup.ProductGroupFeatureIds);
                return product.Id;
            }

            return null;
        }
        // GET: Admin/ProductGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroup productGroup = _repo.Get(id.Value);
            if (productGroup == null)
            {
                return HttpNotFound();
            }
            return PartialView(productGroup);
        }
        [HttpPost]
        public bool UploadImage(int id, HttpPostedFileBase File)
        {
            #region Upload Image
            if (File != null)
            {
                var productGroup = _repo.Get(id);
                if (productGroup.Image != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Image/" + productGroup.Image)))
                        System.IO.File.Delete(Server.MapPath("/Files/ProductGroupImages/Image/" + productGroup.Image));

                    if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Thumb/" + productGroup.Image)))
                        System.IO.File.Delete(Server.MapPath("/Files/ProductGroupImages/Thumb/" + productGroup.Image));
                }
                // Saving Temp Image
                var newFileName = Guid.NewGuid() + Path.GetExtension(File.FileName);
                File.SaveAs(Server.MapPath("/Files/ProductGroupImages/Temp/" + newFileName));
                // Resize Image
                ImageResizer image = new ImageResizer(850, 400, true);
                image.Resize(Server.MapPath("/Files/ProductGroupImages/Temp/" + newFileName),
                    Server.MapPath("/Files/ProductGroupImages/Image/" + newFileName));

                ImageResizer thumb = new ImageResizer(200, 200, true);
                thumb.Resize(Server.MapPath("/Files/ProductGroupImages/Temp/" + newFileName),
                    Server.MapPath("/Files/ProductGroupImages/Thumb/" + newFileName));

                // Deleting Temp Image
                System.IO.File.Delete(Server.MapPath("/Files/ProductGroupImages/Temp/" + newFileName));
                productGroup.Image = newFileName;
                _repo.Update(productGroup);
                return true;
            }
            #endregion
            return false;
        }
        // POST: Admin/ProductGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var productGroup = _repo.Get(id);
            var parentId = productGroup.ParentId;
            //#region Delete ProductGroup Image
            //if (productGroup.Image != null)
            //{
            //    if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Image/" + productGroup.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/ProductGroupImages/Image/" + productGroup.Image));

            //    if (System.IO.File.Exists(Server.MapPath("/Files/ProductGroupImages/Thumb/" + productGroup.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/ProductGroupImages/Thumb/" + productGroup.Image));
            //}
            //#endregion

            _repo.Delete(id);
            return RedirectToAction("Index",new { parentId});
        }
    }
}