using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitakShop.Infrastructure.Helpers;
using System.Web.Mvc;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Core.Models;
using System.IO;
using System.Net;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    public class TeamMembersController : Controller
    {
        private readonly TeamMembersRepository _repo;
        public TeamMembersController(TeamMembersRepository repo)
        {
            _repo = repo;
        }

        // GET: Admin/TeamMembers
        public ActionResult Index()
        {
            var members = _repo.GetTeamMembers();
            return View(members);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamMember teamMember, HttpPostedFileBase TeamMemberImage)
        {
            if (ModelState.IsValid)
            {

                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    ViewBag.Message = "کاربر وارد کننده پیدا نشد.";
                    return View(teamMember);
                }


                #region Upload Image
                if (TeamMemberImage != null)
                {
                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(TeamMemberImage.FileName);
                    TeamMemberImage.SaveAs(Server.MapPath("/Files/TeamMembersImages/Temp/" + newFileName));

                    // Resize Image
                    ImageResizer image = new ImageResizer(300, 300, true);
                    image.Resize(Server.MapPath("/Files/TeamMembersImages/Temp/" + newFileName),
                        Server.MapPath("/Files/TeamMembersImages/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/TeamMembersImages/Temp/" + newFileName));

                    teamMember.Image = newFileName;
                }
                #endregion

                _repo.Add(teamMember);


                return RedirectToAction("Index");
            }

            return View(teamMember);
        }

        // GET: Admin/Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamMember teamMember = _repo.Get(id.Value);
            if (teamMember == null)
            {
                return HttpNotFound();
            }

            return View(teamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamMember teamMember, HttpPostedFileBase TeamMemberImage)
        {
            if (ModelState.IsValid)
            {
                #region Upload Image
                if (TeamMemberImage != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("/Files/TeamMembersImages/" + teamMember.Image)))
                        System.IO.File.Delete(Server.MapPath("/Files/TeamMembersImages/" + teamMember.Image));

                    // Saving Temp Image
                    var newFileName = Guid.NewGuid() + Path.GetExtension(TeamMemberImage.FileName);
                    TeamMemberImage.SaveAs(Server.MapPath("/Files/TeamMembersImages/Temp/" + newFileName));

                    // Resize Image
                    ImageResizer image = new ImageResizer(300, 300, true);
                    image.Resize(Server.MapPath("/Files/TeamMembersImages/Temp/" + newFileName),
                        Server.MapPath("/Files/TeamMembersImages/" + newFileName));

                    // Deleting Temp Image
                    System.IO.File.Delete(Server.MapPath("/Files/TeamMembersImages/Temp/" + newFileName));

                    teamMember.Image = newFileName;
                }
                #endregion

                _repo.Update(teamMember);


                return RedirectToAction("Index");
            }

            return View(teamMember);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamMember teamMember = _repo.Get(id.Value);
            if (teamMember == null)
            {
                return HttpNotFound();
            }
            return PartialView(teamMember);
        }

        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var teamMember = _repo.Get(id);

            //#region Delete Article Image
            //if (article.Image != null)
            //{
            //    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Image/" + article.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Image/" + article.Image));

            //    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Thumb/" + article.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Thumb/" + article.Image));
            //}
            //#endregion

            _repo.Delete(id);
            return RedirectToAction("Index");
        }

    }
}