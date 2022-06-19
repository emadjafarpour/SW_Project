using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BitakShop.Core.Models
{

    public class TeamMember : IBaseEntity
    {
        public int Id { get; set; }

        [Display(Name = "نام و نام خانوادگی:")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; }

        [Display(Name = "عنوان شغلی:")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string JobTitle { get; set; }

        [Display(Name = "تصویر:")]
        public string Image { get; set; }

        [Display(Name = "پیج اینستاگرام:")]
        public string InstagramPage { get; set; }

        [Display(Name = "کانال تلگرام:")]
        public string TelegramChannel { get; set; }

        [Display(Name = "پیج یوتیوب:")]
        public string YoutubePage { get; set; }

        [Display(Name = "پیج تویتر:")]
        public string TwitterPage { get; set; }

        public DateTime? AddedDate { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
