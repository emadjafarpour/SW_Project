using BitakShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Infrastructure.Repositories
{
    public class EPaymentRepository : BaseRepository<EPayment, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public EPaymentRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public EPayment GetInvoiceLatestUnprocessedEPayment(int invoiceId)
        {
            return _context.EPayments.FirstOrDefault(e => e.IsDeleted == false && e.InvoiceId == invoiceId && e.PaymentStatus == Core.Utility.PaymentStatus.Unprocessed);
        }

        public int GetPaymentAccountId()
        {
            return _context.PaymentAccounts.ToList()[0].Id;
        }

        public void ExpireEPayments()
        {
            var epaymentList = _context.EPayments.Where(e => e.PaymentStatus == Core.Utility.PaymentStatus.Unprocessed && e.IsDeleted == false).ToList();
            foreach (var payment in epaymentList)
            {
                var currentTime = DateTime.Now;
                if (currentTime.Subtract(payment.InsertDate.Value).TotalMinutes > 10)
                    payment.PaymentStatus = Core.Utility.PaymentStatus.Expired;
            }

            _context.SaveChanges();
        }

    }


}
