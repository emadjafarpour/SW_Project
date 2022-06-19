using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class Invoice : IBaseEntity
    {
        public int Id { get; set; }

        [DisplayName("میزان تخفیف")]
        public long DiscountAmount { get; set; }
        [DisplayName("قیمت بدون تخفیف")]
        public long TotalPriceBeforeDiscount { get; set; }
        [DisplayName("قیمت نهایی")]
        public long TotalPrice { get; set; }
        [DisplayName("تاریخ ثبت")]
        public DateTime AddedDate { get; set; }
        [DisplayName("شماره سفارش")]
        public string InvoiceNumber { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        [DisplayName("نام مشتری")]
        [MaxLength(500,ErrorMessage = "نام وارد شده باید از 500 کارکتر کمتر باشد")]
        public string CustomerName { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
        public int? GeoDivisionId { get; set; }
        public GeoDivision GeoDivision { get; set; }
        [MaxLength(500, ErrorMessage = "آدرس وارد شده باید از 500 کارکتر کمتر باشد")]
        [DisplayName("آدرس")]
        public string Address { get; set; }
        [MaxLength(50, ErrorMessage = "شماره تلفن وارد شده باید از 50 کارکتر کمتر باشد")]
        [DisplayName("شماره تلفن")]
        public string Phone { get; set; }
        [MaxLength(50, ErrorMessage = "کد پستی وارد شده باید از 50 کارکتر کمتر باشد")]
        [DisplayName("کد پستی")]
        public string PostalCode { get; set; }
        [DisplayName("ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل نا معتبر")]
        public string Email { get; set; }
        [DisplayName("پرداخت شده")]
        public bool IsPayed { get; set; }
        public int? DiscountCodeId { get; set; }
        public DiscountCode DiscountCode { get; set; }
        public ICollection<EPayment> EPayments { get; set; }

        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
