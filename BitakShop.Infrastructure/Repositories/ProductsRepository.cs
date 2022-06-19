using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitakShop.Core.Models;

namespace BitakShop.Infrastructure.Repositories
{
    public class ProductsRepository : BaseRepository<Product, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductsRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Product> GetProducts()
        {
            return _context.Products.Where(p => p.IsDeleted == false).Include(a => a.ProductGroup).OrderByDescending(a=>a.InsertDate).ToList();
        }

        public Product GetProduct(int id)
        {
            var product = _context.Products.Include(p => p.ProductMainFeatures).Include(p => p.ProductFeatureValues).Include(p=>p.Brand).Include(p=>p.ProductGroup)
                .FirstOrDefault(p => p.Id == id);
            product.ProductMainFeatures = product.ProductMainFeatures.Where(f => f.IsDeleted == false).ToList();
            product.ProductFeatureValues = product.ProductFeatureValues.Where(f => f.IsDeleted == false).ToList();
            return product;
        }

        public List<ProductMainFeature> GetProductMainFeatures(int id)
        {
            return _context.ProductMainFeatures.Where(p=>p.ProductId == id && p.IsDeleted == false).ToList();
        }
        public List<ProductFeatureValue> GetProductFeatures(int id)
        {
            return _context.ProductFeatureValues.Where(p => p.ProductId == id && p.IsDeleted == false).ToList();
        }
        public List<SubFeature> GetSubFeaturesByFeatureId(int id)
        {
            return _context.SubFeatures.Where(p => p.IsDeleted == false && p.FeatureId == id).ToList();
        }

        public ProductMainFeature AddProductMainFeature(ProductMainFeature mainFeature)
        {
            var user = GetCurrentUser();
            mainFeature.InsertDate = DateTime.Now;
            mainFeature.InsertUser = user.UserName;
            _context.ProductMainFeatures.Add(mainFeature);
            _context.SaveChanges();

            _logger.LogEvent(mainFeature.GetType().Name, mainFeature.Id, "Add");
            return mainFeature;
        }
        public ProductFeatureValue AddProductFeature(ProductFeatureValue feature)
        {
            var user = GetCurrentUser();
            feature.InsertDate = DateTime.Now;
            feature.InsertUser = user.UserName;
            _context.ProductFeatureValues.Add(feature);
            _context.SaveChanges();

            _logger.LogEvent(feature.GetType().Name, feature.Id, "Add");
            return feature;
        }
        public List<Product> GetNewestProducts(int take)
        {
            return _context.Products.Where(p=>p.IsDeleted == false).OrderByDescending(p => p.InsertDate).Take(take).ToList();
        }
    }
}
