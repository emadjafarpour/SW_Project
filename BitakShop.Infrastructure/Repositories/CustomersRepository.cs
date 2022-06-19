using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitakShop.Core.Models;
using BitakShop.Infrastructure.Helpers;

namespace BitakShop.Infrastructure.Repositories
{
    public class CustomersRepository : BaseRepository<Customer, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public CustomersRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Customer> GetCustomerTable()
        {
            return _context.Customers.Where(c => c.IsDeleted == false).Include(c => c.User).ToList();
        }

        public Customer GetCustomer(int id)
        {
            return _context.Customers.Include(c=>c.User).FirstOrDefault(c => c.Id == id);
        }
        public Customer GetCurrentCustomer()
        {
            var currentUserId = CheckPermission.GetCurrentUserId();
            return _context.Customers.Include(c => c.User).Include(c=>c.GeoDivision).FirstOrDefault(c => c.UserId == currentUserId);
        }

        public Customer GetCustomerUsingPaymentId(int paymentId)
        {
            var invoiceId = _context.EPayments.FirstOrDefault(e => e.Id == paymentId).InvoiceId;
            var invoice = _context.Invoices.Include(i=>i.Customer.User).FirstOrDefault(i => i.Id == invoiceId);

            return invoice.Customer;
        }
    }
}
