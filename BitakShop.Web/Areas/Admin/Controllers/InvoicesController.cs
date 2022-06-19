using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitakShop.Core.Models;
using BitakShop.Core.Utility;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Web.ViewModels;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly InvoicesRepository _repo;
        private readonly GeoDivisionsRepository _GeoRepo;

        public InvoicesController(InvoicesRepository repo, GeoDivisionsRepository geoRepo)
        {
            _repo = repo;
            _GeoRepo = geoRepo;
        }

        // GET: Admin/Invoices
        public ActionResult Index()
        {
            var invoices = _repo.GetInvoices();
            var vm = new List<InvoiceTableViewModel>();
            foreach (var invoice in invoices)
            {
               vm.Add(new InvoiceTableViewModel(invoice)); 
            }
            return View(vm);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = _repo.Get(id.Value);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.GeoDivisionId = new SelectList(_GeoRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", invoice.GeoDivisionId);
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Invoice invoice)
        {
            if (ModelState.IsValid)
            {

                _repo.Update(invoice);
                return RedirectToAction("Index");
            }
            ViewBag.GeoDivisionId = new SelectList(_GeoRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", invoice.GeoDivisionId);
            return View(invoice);
        }

        public ActionResult ViewInvoice(int invoiceId)
        {
            var vm = new ViewInvoiceViewModel();
            var invoice = _repo.GetInvoice(invoiceId);
            vm.Invoice = invoice;
            vm.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
            vm.InvoiceItems = new List<InvoiceItemWithMainFeatureViewModel>();
            // Getting Invoice Item SubFeatures
            foreach (var invoiceItem in invoice.InvoiceItems)
            {
                var invoiceItemWithMainFeature = new InvoiceItemWithMainFeatureViewModel
                {
                    InvoiceItem = invoiceItem, MainFeature = _repo.GetInvoiceItemsMainFeature(invoiceItem.Id)
                };
                vm.InvoiceItems.Add(invoiceItemWithMainFeature);

            }
            return View(vm);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = _repo.Get(id.Value);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
            return PartialView(invoice);
        }

        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var invoice = _repo.Get(id);
            _repo.Delete(id);
            return RedirectToAction("Index");
        }

    }
}