using BitakShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Infrastructure.Repositories
{
    public class ProductGalleriesRepository : BaseRepository<ProductGallery, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductGalleriesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<ProductGallery> GetProductGalleries(int productId)
        {
            return _context.ProductGalleries.Where(h => h.ProductId == productId & h.IsDeleted == false).ToList();
        }
        public string GetProductName(int productId)
        {
            return _context.Products.Find(productId).Title;
        }
    }
}
