using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class DiscountCode : IBaseEntity
    {
        public int Id { get; set; }
        [Display(Name = "کد تخفیف")]
        public string DiscountCodeStr { get; set; }
        public DateTime ActivationStartDate { get; set; }
        public DateTime ActivationEndDate { get; set; }
        public bool IsActive { get; set; } // wheather been used or not
        public long Value { get; set; }    // the value of discount code 
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }

}
