using BitakShop.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitakShop.Web.ViewModels
{
    public class ArticleFormViewModel
    {
        public int Id { get; set; }
        [Display(Name = "عنوان مقاله")]
        [MaxLength(600, ErrorMessage = "{0} باید از 600 کارکتر کمتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        [Display(Name = "توضیح")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Description { get; set; }
        public int ArticleCategoryId { get; set; }
        public HttpPostedFileBase ArticleImage { get; set; }

        public List<ArticleHeadLineViewModel> ArticleHeadLines { get; set; }
    }
    public class ArticleHeadLineViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Description { get; set; }
    }

    public class ArticleInfoViewModel
    {
        public ArticleInfoViewModel()
        {
            
        }

        public ArticleInfoViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.Author = article.User != null? $"{article.User.FirstName} {article.User.LastName}" : "-";
            this.ArticleCategory = article.ArticleCategory != null? article.ArticleCategory.Title : "-";
            this.PersianAddedDate = article.AddedDate != null? new PersianDateTime(article.AddedDate.Value).ToString() : "-";
            this.AddedDate = article.AddedDate;
        }
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        public string Title { get; set; }
        [Display(Name = "نویسنده")]
        public string Author { get; set; }
        [Display(Name = "دسته بندی")]
        public string ArticleCategory { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public string PersianAddedDate { get; set; }
        public DateTime? AddedDate { get; set; }
    }
    public class CommentWithPersianDateViewModel : ArticleComment
    {
        public CommentWithPersianDateViewModel(ArticleComment comment)
        {
            this.Comment = comment;
            this.PersianDate = comment.AddedDate != null ? new PersianDateTime(comment.AddedDate.Value).ToString("dddd d MMMM yyyy") : "-";
        }
        public ArticleComment Comment { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public string PersianDate { get; set; }
    }

    public class LatestArticlesViewModel
    {
        public LatestArticlesViewModel()
        {
        }

        public LatestArticlesViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.Image = article.Image;
            this.ShortDescription = article.ShortDescription;
            this.Author = $"{article.User.FirstName} {article.User.LastName}";
            this.PersianDate = article.AddedDate != null ? new PersianDateTime(article.AddedDate.Value).ToString("dddd d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string Author { get; set; }
        public string PersianDate { get; set; }
    }
    public class ArticleDetailsViewModel
    {
        public ArticleDetailsViewModel()
        {

        }
        public ArticleDetailsViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.Image = article.Image;
            this.ShortDescription = article.ShortDescription;
            this.Description = article.Description;
            this.Author = article.User != null ? $"{article.User.FirstName} {article.User.LastName}" : "-";
            this.AuthorImage = article.User != null ? article.User.Avatar : "user-avatar.png";
            this.AuthorInfo = article.User != null ? article.User.Information : "";
            this.PersianDate = article.AddedDate != null ? new PersianDateTime(article.AddedDate.Value).ToString("dddd d MMMM yyyy") : "-";
            this.CategoryId = article.ArticleCategory.Id;
            this.CategoryName = article.ArticleCategory.Title;

        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string AuthorImage { get; set; }
        public string AuthorInfo { get; set; }
        public string PersianDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? PreviousArticleID { get; set; }
        public int? NextArticleID { get; set; }
        public List<ArticleHeadLine> HeadLines { get; set; }
        public List<ArticleTag> Tags { get; set; }
        public List<ArticleCommentViewModel> ArticleComments { get; set; }
        public CommentFormViewModel CommentForm { get; set; }
        public List<ArticleDetailsViewModel> RelatedArticles { get; set; }
    }

    public class ArticleCommentViewModel
    {
        public ArticleCommentViewModel()
        {

        }

        public ArticleCommentViewModel(ArticleComment comment)
        {
            this.Id = comment.Id;
            this.ParentId = comment.ParentId;
            this.Name = comment.Name;
            this.Email = comment.Email;
            this.Message = comment.Message;
            this.AddedDate = comment.AddedDate != null ? new PersianDateTime(comment.AddedDate.Value).ToString("dddd d MMMM yyyy") : "-";
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string AddedDate { get; set; }
    }
}