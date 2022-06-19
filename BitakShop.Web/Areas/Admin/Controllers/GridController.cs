﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using BitakShop.Core.Models;
using BitakShop.Infrastructure;

namespace BitakShop.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class GridController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public ActionResult Index()
        {
            return View(db.Articles.ToList());
        }

        public ActionResult Articles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Article> articles = db.Articles.Include(a=>a.ArticleCategory);
            DataSourceResult result = articles.ToDataSourceResult(request, article => new {
                Id = article.Id,
                Title = article.Title,
                Description = article.Description,
                ViewCount = article.ViewCount,
                Image = article.Image,
                AddedDate = article.AddedDate,
                ArticleCategory = article.ArticleCategory
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Articles_Create([DataSourceRequest]DataSourceRequest request, Article article)
        {
            if (ModelState.IsValid)
            {
                var entity = new Article
                {
                    Title = article.Title,
                    Description = article.Description,
                    ViewCount = article.ViewCount,
                    Image = article.Image,
                    AddedDate = article.AddedDate,
                };

                db.Articles.Add(entity);
                db.SaveChanges();
                article.Id = entity.Id;
            }

            return Json(new[] { article }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Articles_Update([DataSourceRequest]DataSourceRequest request, Article article)
        {
            if (ModelState.IsValid)
            {
                var entity = new Article
                {
                    Id = article.Id,
                    Title = article.Title,
                    Description = article.Description,
                    ViewCount = article.ViewCount,
                    Image = article.Image,
                    AddedDate = article.AddedDate,
                };

                db.Articles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { article }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Articles_Destroy([DataSourceRequest]DataSourceRequest request, Article article)
        {
            if (ModelState.IsValid)
            {
                var entity = new Article
                {
                    Id = article.Id,
                    Title = article.Title,
                    Description = article.Description,
                    ViewCount = article.ViewCount,
                    Image = article.Image,
                    AddedDate = article.AddedDate,
                };

                db.Articles.Attach(entity);
                db.Articles.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { article }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
