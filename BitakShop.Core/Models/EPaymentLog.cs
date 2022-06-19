using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Core.Models
{
    public class EPaymentLog : IBaseEntity
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }
        public DateTime LogDate { get; set; }
        [MaxLength(50)]
        public string LogType { get; set; }
        public int PaymentKey { get; set; }
        public int EPaymentId { get; set; }
        public EPayment EPayment { get; set; }
        [MaxLength(100)]
        public string MethodName { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public long Amount { get; set; }
        [MaxLength(50)]
        public string RetrievalRefNo { get; set; }
        [MaxLength(50)]
        public string StackTraceNo { get; set; }
        [MaxLength(100)]
        public string Token { get; set; }
        [MaxLength(200)]
        public string Url { get; set; }
        public string ReturnObjectSerialization { get; set; }
        [MaxLength(200)]
        public string ReturnUrl { get; set; }
        [MaxLength(500)]
        public string AdditionalData { get; set; }
        [MaxLength(10)]
        public string ResCode { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
