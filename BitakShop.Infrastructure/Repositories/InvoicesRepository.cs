using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using BitakShop.Core.Models;

namespace BitakShop.Infrastructure.Repositories
{
    public class InvoicesRepository : BaseRepository<Invoice, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public InvoicesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Invoice> GetInvoices()
        {
            return _context.Invoices.Include(i => i.Customer.User).Where(i => i.IsDeleted == false).ToList();
        }
        public Invoice GetInvoice(int invoiceId)
        {
            return _context.Invoices.Include(i => i.Customer.User).Include(i=>i.InvoiceItems.Select(c => c.Product)).FirstOrDefault(i=>i.Id == invoiceId && i.IsDeleted==false);
        }

        public string GetInvoiceItemsMainFeature(int invoiceItemId)
        {
            var invoiceItem = _context.InvoiceItems.Find(invoiceItemId);
            var mainFeature = _context.ProductMainFeatures.Include(m=>m.SubFeature).FirstOrDefault(m=>m.Id == invoiceItem.MainFeatureId);
            return mainFeature.SubFeature == null ? mainFeature.Value : mainFeature.SubFeature.Value;
        }

        public List<Product> GertTopSoldProducts(int take)
        {
            List<Product> products = new List<Product>();
            var productIds = _context.InvoiceItems.GroupBy(i => i.ProductId)
                .OrderByDescending(pi => pi.Count())
                .Select(g => g.Key).ToList();
            foreach (var id in productIds)
            {
                if (products.Count < take)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == id);
                    if (product != null && product.IsDeleted == false)
                    {
                        products.Add(product);
                    }
                }
            }

            return products;
        }

        public List<Invoice> GetCustomerInvoices(int customerId)
        {
            return _context.Invoices.Where(i => i.IsDeleted == false && i.CustomerId == customerId).ToList();
        }

        public Invoice GetInvoice(string invoiceNumber)
        {
            return _context.Invoices.Include(i => i.Customer.User).Include(i => i.InvoiceItems).Include(i => i.DiscountCode).FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);
        }

        public Invoice GetInvoice(string invoiceNumber, int customerId)
        {
            return _context.Invoices.Include(i => i.Customer.User).Include(i => i.InvoiceItems).Include(i => i.DiscountCode).FirstOrDefault(i => i.InvoiceNumber == invoiceNumber && i.CustomerId == customerId);
        }

        public Invoice GetLatestInvoice(int customerId)
        {
            Invoice invoice = null;
            try
            {
                invoice = _context.Invoices.Include(i => i.Customer.User).Include(i => i.InvoiceItems).Include(i => i.DiscountCode).OrderByDescending(i => i.AddedDate).Where(i => i.CustomerId == customerId).ToList()[0];
            }
            catch
            {

            }
            return invoice;
        }

        public Invoice GetLatestPayedInvoice(int customerId)
        {
            Invoice invoice = null;
            try
            {
                invoice = _context.Invoices.Include(i => i.Customer.User).Include(i => i.InvoiceItems).Include(i => i.DiscountCode).OrderByDescending(i => i.AddedDate).Where(i => i.CustomerId == customerId && i.IsPayed == true).ToList()[0];
            }
            catch
            {

            }
            return invoice;
        }

        public Invoice GetInvoiceByPaymentId(int paymentId)
        {
            var payment = _context.EPayments.FirstOrDefault(p => p.Id == paymentId);
            return _context.Invoices.Include(i => i.Customer.User).Include(i => i.InvoiceItems).Include(i => i.DiscountCode).FirstOrDefault(i => i.Id == payment.InvoiceId);
        }

    }
}
