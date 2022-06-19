using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class InvoiceItem : IBaseEntity
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public long Price { get; set; }
        public long TotalPrice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int MainFeatureId { get; set; }
        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
