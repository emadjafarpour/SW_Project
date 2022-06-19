using BitakShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitakShop.Infrastructure.Repositories
{
    public class ProductFeatureValuesRepository : BaseRepository<ProductFeatureValue, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductFeatureValuesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<ProductFeatureValue> GetProductFeatures(int productId)
        {
            return _context.ProductFeatureValues.Include(f => f.Feature).Include(f => f.SubFeature)
                .Where(f => f.IsDeleted == false && f.ProductId == productId).ToList();
        }
    }
}
