using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class ProductFeatureValue : IBaseEntity
    {
        public int Id { get; set; }
        [Display(Name = "ویژگی")]
        [MaxLength(600, ErrorMessage = "{0} باید کمتر از 600 کارکتر باشد")]
        public string Value { get; set; }
        [Display(Name = "اطلاعات دیگر")]
        [MaxLength(600, ErrorMessage = "{0} باید کمتر از 600 کارکتر باشد")]
        public string OtherInfo { get; set; }
        public int? FeatureId { get; set; }
        public Feature Feature { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int? SubFeatureId { get; set; }
        public SubFeature SubFeature { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
