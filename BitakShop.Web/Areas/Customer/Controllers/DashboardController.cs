using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BitakShop.Core.Models;
using BitakShop.Core.Utility;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Web.ViewModels;

namespace BitakShop.Web.Areas.Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    public class DashboardController : Controller
    {
        private UsersRepository _usersRepo;
        private CustomersRepository _customerRepo;
        private InvoicesRepository _invoiceRepo;
        private GeoDivisionsRepository _geoDivisonsRepo;
        public DashboardController(UsersRepository usersRepo,
            CustomersRepository customerRepo,
            InvoicesRepository invoiceRepo, 
            GeoDivisionsRepository geoDivisonsRepo)
        {
            _usersRepo = usersRepo;
            _customerRepo = customerRepo;
            _invoiceRepo = invoiceRepo;
            _geoDivisonsRepo = geoDivisonsRepo;
        }
        // GET: Customer/Dashboard
        public ActionResult Index()
        {
            var customer = _customerRepo.GetCurrentCustomer();
            var invoices = _invoiceRepo.GetCustomerInvoices(customer.Id);
            var invoicesVm = new List<CustomerInvoiceViewModel>();
            foreach (var item in invoices)
                invoicesVm.Add(new CustomerInvoiceViewModel(item));
            var vm = new CustomerDashboardViewModel()
            {
                Customer = customer,
                Invoices = invoicesVm
            };
            return View(vm);
        }

        public ActionResult EditMyProfile()
        {
            var customer = _customerRepo.GetCurrentCustomer();
            if (customer == null)
            {
                return HttpNotFound();
            }

            var form = new EditCustomerViewModel()
            {
                UserId = customer.User.Id,
                UserName = customer.User.UserName,
                CustomerId = customer.Id,
                FirstName = customer.User.FirstName,
                LastName = customer.User.LastName,
                Email = customer.User.Email,
                PhoneNumber = customer.User.PhoneNumber,
                Avatar = customer.User.Avatar,
                NationalCode = customer.NationalCode,
                Address = customer.Address,
                PostalCode = customer.PostalCode,
                GeoDivisionId = customer.GeoDivisionId
            };
            ViewBag.GeoDivisionId = new SelectList(_geoDivisonsRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
            return View(form);
        }
        [HttpPost]
        public ActionResult EditMyProfile(EditCustomerViewModel form, HttpPostedFileBase UserAvatar)
        {
            if (ModelState.IsValid)
            {
                #region Check for duplicate username or email

                if (form.UserName != null)
                {
                    if (_usersRepo.UserNameExists(form.UserName, form.UserId))
                    {
                        ViewBag.Message = "کاربر دیگری با همین نام کاربری در سیستم ثبت شده";
                        ViewBag.GeoDivisionId = new SelectList(_geoDivisonsRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);

                        return View(form);
                    }
                }
                if (_usersRepo.PhoneNumberExists(form.PhoneNumber, form.UserId))
                {
                    ViewBag.Message = "کاربر دیگری با همین شماره تلفن در سیستم ثبت شده";
                    ViewBag.GeoDivisionId = new SelectList(_geoDivisonsRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
                    return View(form);
                }
                if (_usersRepo.EmailExists(form.Email, form.UserId))
                {
                    ViewBag.Message = "کاربر دیگری با همین ایمیل در سیستم ثبت شده";
                    ViewBag.GeoDivisionId = new SelectList(_geoDivisonsRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
                    return View(form);
                }
                #endregion

                #region Upload Image
                if (UserAvatar != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/UserAvatars/" + form.Avatar)))
                        System.IO.File.Delete(Server.MapPath("/Files/UserAvatars/" + form.Avatar));

                    var newFileName = Guid.NewGuid() + Path.GetExtension(UserAvatar.FileName);
                    UserAvatar.SaveAs(Server.MapPath("/Files/UserAvatars/" + newFileName));

                    form.Avatar = newFileName;
                }
                #endregion

                var user = _usersRepo.GetUser(form.UserId);
                user.UserName = form.UserName ?? form.PhoneNumber;
                user.FirstName = form.FirstName;
                user.LastName = form.LastName;
                user.Email = form.Email;
                user.PhoneNumber = form.PhoneNumber;
                user.Avatar = form.Avatar;

                _usersRepo.UpdateUser(user);

                var customer = _customerRepo.Get(form.CustomerId.Value);

                customer.NationalCode = form.NationalCode;
                customer.Address = form.Address;
                customer.PostalCode = form.PostalCode;
                customer.GeoDivisionId = form.GeoDivisionId;
                _customerRepo.Update(customer);

                return RedirectToAction("Index");
            }
            ViewBag.GeoDivisionId = new SelectList(_geoDivisonsRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
            return View(form);

        }
        public ActionResult ResetMyPassword(string id)
        {
            ViewBag.Message = null;
            ViewBag.UserId = id;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetMyPassword(ResetMyPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var validatePassword = await _usersRepo.ValidatePassword(model.OldPassword);
                if (validatePassword.Succeeded)
                {
                    var result = await _usersRepo.SetNewPassword(model.UserId, model.OldPassword, model.Password);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                }

                ViewBag.Message = "رمز عبور وارد شده صحیح نیست";
                ViewBag.UserId = model.UserId;
            }
            return View(model);
        }

        public ActionResult Invoices()
        {
            var customer = _customerRepo.GetCurrentCustomer();
            var invoices = _invoiceRepo.GetCustomerInvoices(customer.Id);
            var invoicesVm = new List<CustomerInvoiceViewModel>();
            foreach (var item in invoices)
                invoicesVm.Add(new CustomerInvoiceViewModel(item));

            return View(invoicesVm);
        }

        public ActionResult InvoiceDetails(int id = 0)
        {
            if(id < 1)
            {
                return Redirect("/Customer/Dashboard");
            }

            var vm = new ViewInvoiceViewModel();
            var invoice = _invoiceRepo.GetInvoice(id);

            if(invoice==null)
                return Redirect("/Customer/Dashboard");

            vm.Invoice = invoice;
            vm.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
            vm.InvoiceItems = new List<InvoiceItemWithMainFeatureViewModel>();
            // Getting Invoice Item SubFeatures
            foreach (var invoiceItem in invoice.InvoiceItems)
            {
                var invoiceItemWithMainFeature = new InvoiceItemWithMainFeatureViewModel
                {
                    InvoiceItem = invoiceItem,
                    MainFeature = _invoiceRepo.GetInvoiceItemsMainFeature(invoiceItem.Id)
                };
                invoiceItemWithMainFeature.InvoiceItem.TotalPrice = invoiceItemWithMainFeature.InvoiceItem.Quantity * invoiceItemWithMainFeature.InvoiceItem.Price;
                vm.InvoiceItems.Add(invoiceItemWithMainFeature);

            }
            return View(vm);
        }

    }
}