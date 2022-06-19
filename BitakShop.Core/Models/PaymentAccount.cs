using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class PaymentAccount : IBaseEntity
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string PIN { get; set; }
        public string ComebackUrl { get; set; }
        public string PaymentUrl { get; set; }
        public int MerchantId { get; set; }
        public int TerminalId { get; set; }
        public ICollection<EPayment> EPayments { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
