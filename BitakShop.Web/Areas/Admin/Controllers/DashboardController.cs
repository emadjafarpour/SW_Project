using BitakShop.Infrastructure;
using BitakShop.Infrastructure.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitakShop.Web.ViewModels;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private MyDbContext db = new MyDbContext();
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserPartial()
        {
            var currentUserId = CheckPermission.GetCurrentUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var userVm = new UserPartialViewModel();
            userVm.Id = user.Id;
            userVm.FirstName = user.FirstName;
            userVm.LastName = user.LastName;
            userVm.Email = user.Email;
            userVm.RoleName = db.Roles.FirstOrDefault(r => r.Id == db.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id).RoleId).Name;
            userVm.UserAvatar = user.Avatar;
            return PartialView(userVm);
        }
        public PartialViewResult ShowError(String sErrorMessage)
        {
            return PartialView("ErrorMessageView");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}