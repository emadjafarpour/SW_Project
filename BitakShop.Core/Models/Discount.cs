using BitakShop.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class Discount : IBaseEntity
    {
        public int Id { get; set; }
        public string GroupIdentifier { get; set; }
        [Display(Name = "عنوان تخفیف")]
        public string Title { get; set; }
        [Display(Name = "تاریخ پایان")]
        public DateTime DeadLine { get; set; }
        public int DiscountType { get; set; }
        public long Amount { get; set; }
        public int? OfferId { get; set; }
        [Display(Name = "پیشنهاد")]
        public Offer Offer { get; set; }
        public int? ProductId { get; set; }
        [Display(Name = "محصول")]
        public Product Product { get; set; }
        public int? ProductGroupId { get; set; }
        [Display(Name = "دسته")]
        public ProductGroup ProductGroup { get; set; }
        public int? BrandId { get; set; }
        [Display(Name = "برند")]
        public Brand Brand { get; set; }
        public DiscountLocationType DiscountLocationType { get; set; }
        [Display(Name ="تصویر")]
        public string Image { get; set; }

        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
