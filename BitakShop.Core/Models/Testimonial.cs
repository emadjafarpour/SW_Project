using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class Testimonial : IBaseEntity
        {
            public int Id { get; set; }
            [Display(Name = "نام")]
            [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
            public string Name { get; set; }
            [Display(Name = "تصویر")]
            public string Image { get; set; }
            [Display(Name = "توضیح")]
            [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
            [DataType(DataType.MultilineText)]
            public string Description { get; set; }
            public string InsertUser { get; set; }
            public DateTime? InsertDate { get; set; }
            public string UpdateUser { get; set; }
            public DateTime? UpdateDate { get; set; }
            public bool IsDeleted { get; set; }
        }
}
