using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class ProductGroupFeature : IBaseEntity
    {
        public int ProductGroupId { get; set; }
        public ProductGroup ProductGroup { get; set; }
        public int FeatureId { get; set; }
        public Feature Feature { get; set; }
        public int Id { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
