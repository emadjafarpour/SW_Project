using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using BitakShop.Core.Models;
using BitakShop.Core.Utility;

namespace BitakShop.Web.ViewModels
{
    public class DiscountFormViewModel
    {
        [DisplayName("عنوان تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} باید کمتر از 500 کارکتر باشد")]
        public string Title { get; set; }
        public int DiscountType { get; set; }
        [DisplayName("میزان تخفیف")]
        [Required(ErrorMessage = "لطفا میزان تخفیف را وارد کنید")]
        public long Amount { get; set; }
        public string DeadLine { get; set; }
        public List<int> BrandIds { get; set; }
        public List<int> ProductIds { get; set; }
        public List<int> ProductGroupIds { get; set; }
        public string Image { get; set; }
        public bool IsOffer { get; set; }
        public int? OfferId { get; set; }
        public DiscountLocationType DiscountLocationType { get; set; }

        // Edit Props
        public string GroupIdentifier { get; set; }
        public List<Discount> PreviousDiscounts { get; set; }
    }

    public class TestimonialViewModel
    {
        public TestimonialViewModel()
        {
        }

        public TestimonialViewModel(Testimonial testimonial)
        {
            this.Name = testimonial.Name;
            this.Image = testimonial.Image;
            this.Description = testimonial.Description;
            this.PersianDate = new PersianDateTime(testimonial.InsertDate.Value).ToString("dddd d MMMM yyyy");
        }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string PersianDate { get; set; }
    }
    public class ContactUsViewModel
    {
        public string Map { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class CartModel
    {
        public CartModel()
        {
        }

        public CartModel(string json)
        {
            JObject jObject = JObject.Parse(json);
            var jItems = jObject["CartItems"].ToList();
            var cartItems = new List<CartItemModel>();
            foreach (var item in jItems)
            {
                cartItems.Add(new CartItemModel()
                {
                    Id = Convert.ToInt32(item["Id"]),
                    ProductName = (string)item["ProductName"],
                    Image = (string)item["Image"],
                    Price = Convert.ToInt64(item["Price"]),
                    MainFeatureId = Convert.ToInt32(item["MainFeatureId"]),
                    Quantity = Convert.ToInt32(item["Quantity"])
                });
            }

            this.CartItems = cartItems;
            this.TotalPrice = Convert.ToInt64(jObject["TotalPrice"]);
        }
        public List<CartItemModel> CartItems { get; set; }
        public long TotalPrice { get; set; }
    }
    public class CartItemModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public long Price { get; set; }
        public long OffAmount { get; set; }
        public int Quantity { get; set; }
        public int MainFeatureId { get; set; }
    }
    public class WishListModel
    {
        public WishListModel()
        {
        }

        public WishListModel(string json)
        {
            JObject jObject = JObject.Parse(json);
            var jItems = jObject["WishListItems"].ToList();
            var wishListItems = new List<WishListItemModel>();
            foreach (var item in jItems)
            {
                wishListItems.Add(new WishListItemModel()
                {
                    Id = Convert.ToInt32(item["Id"]),
                    ProductName = (string)item["ProductName"],
                    Image = (string)item["Image"],
                });
            }

            this.WishListItems = wishListItems;
        }
        public List<WishListItemModel> WishListItems { get; set; }
    }
    public class WishListItemModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
    }

    public class CommentFormViewModel
    {
        public int? ParentId { get; set; }
        public int? ArticleId { get; set; }
        public int? ProductId { get; set; }
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} باید کمتر از 300 کارکتر باشد")]
        public string Name { get; set; }
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل نا معتبر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} باید کمتر از 400 کارکتر باشد")]
        public string Email { get; set; }
        [Display(Name = "پیام")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(800, ErrorMessage = "{0} باید کمتر از 800 کارکتر باشد")]
        public string Message { get; set; }

        [Required(ErrorMessage = "لطفا {0} را تعیین کنید")]
        [Display(Name = "امتیاز")]
        public int? Rate { get; set; }
    }
    public class CheckoutForm
    {
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; }
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل نا معتبر")]
        [MaxLength(400, ErrorMessage = "{0} باید کمتر از 400 کارکتر باشد")]
        public string Email { get; set; }
        [Display(Name = "تلفن")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} باید کمتر از 400 کارکتر باشد")]
        public string Phone { get; set; }
        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Address { get; set; }
        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PostalCode { get; set; }
        public int GeoDivisionId { get; set; }
        /*[Display(Name = "پیام")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(800, ErrorMessage = "{0} باید کمتر از 800 کارکتر باشد")]*/
        public string Message { get; set; }

        public string DiscountCode { get; set; }
        public string InvoiceNumber { get; set; }
    }
}